using System;
using System.Linq;
using System.Collections.Generic;
using Krydsord.Helpers;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public class StartsWithKrydsordGenerator : DanVandretCheckLodretKrydsordGenerator
    {
        private Dictionary<int, List<char>> fejlOrd;
        private KeyValuePair<int, int> affectedItem;
        private int wordCounter = 1;
        private StatefulOrdlisteOrd lastAddedWord;
        public StartsWithKrydsordGenerator(IOrdliste ordliste, IView view)
            : base(ordliste, view)
        {

        }

        protected override void FixKrydsord()
        {
            var latestAffectedOrdlisteOrd = (StatefulOrdlisteOrd)ordlisteOrdPrLinje[affectedItem.Key][affectedItem.Value];
            // We know all lists are non-empty, or the previous words could not have been added
            foreach (KeyValuePair<int, List<char>> keyValuePair in fejlOrd)
            {
                latestAffectedOrdlisteOrd.FilterOrdliste(keyValuePair.Key,keyValuePair.Value);
                if (!lastAddedWord.CurrentOrdliste.Any())
                {
                    --wordCounter;
                    if (wordCounter == 0)
                        throw new Exception("Krydsordslayoutet der resulterer fra seed " + seed +
                                            " kunne ikke indeholde en lovlig krydsord - alle mulige kombinationer forsøgt uden held.");
                }
            }

        }

        protected override bool CheckKrydsord()
        {
            fejlOrd = new Dictionary<int, List<char>>();
            bool fejlOrdFundet;
            do
            {
                fejlOrdFundet = false;
                ConvertToResult();
                lastAddedWord = (StatefulOrdlisteOrd)ordlisteOrd[wordCounter - 1];
                
                // Initialize fejlord
                for (int i = 0; i < lastAddedWord.length; ++i)
                {
                    fejlOrd.Add(i, new List<char>());
                }
                DanLodretKrydsord(lastAddedWord.column + lastAddedWord.length, lastAddedWord.row);

                for (var currentColumn = lastAddedWord.column; currentColumn < lastAddedWord.column + lastAddedWord.length; ++currentColumn)
                {
                    string currentColumnString = lodretKrydsord[currentColumn];
                    int startOfAffectedWord = currentColumnString.Substring(0, lastAddedWord.row).LastIndexOf('#') + 1;
                    var currentColumnWord = currentColumnString.Substring(startOfAffectedWord).Split('#').First();

                    var fixedPart = currentColumnWord.Trim('.');

                    if (!ordliste.OrdDerStarterMed(fixedPart, currentColumnWord.Length))
                    {
                        if(!fejlOrdFundet) affectedItem = new KeyValuePair<int, int>(lastAddedWord.row, ordlisteOrdPrLinje[lastAddedWord.row].IndexOf(lastAddedWord));
                        fejlOrdFundet = true;
                        fejlOrd[currentColumn-lastAddedWord.column].AddRange(FindMuligeBogstaver(fixedPart.Substring(0, fixedPart.Length - 1), currentColumnWord.Length));
                    }
                }
                // The last word added caused no errors, so add another
                if (!fejlOrdFundet) ++wordCounter;
            } while (!fejlOrdFundet && wordCounter <= ordlisteOrd.Count);
            if (fejlOrdFundet) return false;
            DanLodretKrydsord(true);
            return true;
        }

        // If we know the list of possible letters in ordlisten, we could make this faster for large lists
        // by iterating over the possible letters and checking for AnyStartsWith for each
        private IEnumerable<char> FindMuligeBogstaver(string substring, int wordLength)
        {
            return ordliste.GetOrdDerStarterMed(substring, wordLength).Select(ord => ord.First()).Distinct();
        }

        protected override void InitializeKrydsord()
        {
            for (var currentLine = 0; currentLine < height; ++currentLine)
            {
                int currentChar = 0;
                while (currentChar < width)
                {
                    var remaining = width - currentChar;
                    var length = Math.Min(randomHelper.randomInInterval(width, true), remaining);
                    var ordindex = randomHelper.randomInInterval(ordliste.GetOrd(length).Count, false);
                    var nextOrd = ordliste.GetOrd(length)[ordindex];
                    var nytOrd = new StatefulOrdlisteOrd(ordindex, nextOrd, ordliste.GetOrd(length).Count, currentLine, currentChar, ordliste);
                    ordlisteOrdPrLinje[currentLine].Add(nytOrd);
                    ordlisteOrd.Add(nytOrd);
                    KeyValuePair<int, int> dummy;
                    currentChar += length + 1;
                }
            }
            ConvertToResult();
        }

        protected class StatefulOrdlisteOrd : OrdlisteOrd
        {
            public int row;
            public int column;
            private IOrdliste ordliste;
            //private IList<string> currentOrdliste;
            private List<KompletOrd> kompletteOrd;
            private bool initialOrdlisteState;


            public StatefulOrdlisteOrd(int index, string ord, int wordcountOfThisLength, int row, int column, IOrdliste ordliste)
                : base(index, ord, wordcountOfThisLength)
            {
                this.row = row;
                this.column = column;
                this.ordliste = ordliste;
                ResetOrdliste();
            }

            public void ResetOrdliste()
            {
                //currentOrdliste = ordliste.GetOrd(ord.Length);
                kompletteOrd = ordliste.GetOrd(ord.Length).Select(currentOrd => new KompletOrd(currentOrd)).ToList();
                initialOrdlisteState = true;
            }

            //public void FilterOrdliste(IList<string> filterList)
            //{
            //    if (initialOrdlisteState) currentOrdliste = filterList;
            //    else currentOrdliste = currentOrdliste.Intersect(filterList).ToList();
            //}

            public void FilterOrdliste(int index, List<char> letters)
            {
                //kompletteOrd = kompletteOrd.Where(kompletOrd => kompletOrd.Rotation(index).First() == letter);
                List<KompletOrd> lovligeOrd = new List<KompletOrd>();
                foreach (char letter in letters)
                {
                    lovligeOrd = lovligeOrd.Union(
                        SortedComparableListHelper<KompletOrd>.StartsWith(
                            kompletteOrd,
                            new KompletOrd(letter.ToString()),
                            index)
                        ).ToList();
                }
                kompletteOrd = lovligeOrd;
            }

            public IEnumerable<string> CurrentOrdliste
            {
                get { return kompletteOrd.Select(kompletOrd => kompletOrd.Ord); }
            }
        }

        // Class that contains all rotations of a word and supports comparison and StartsWith on any rotation
        protected class KompletOrd : INthComparable, IStartsWith
        {
            private int length;
            private List<string> rotationer;
            public KompletOrd(string ord)
            {
                rotationer = new List<string>();
                length = ord.Length;
                for (int i = 0; i < length; ++i)
                {
                    rotationer.Add(ord.Rotate(i));
                }
            }

            public string Ord
            {
                get { return rotationer[0]; }
            }

            public string Rotation(int antaltegn)
            {
                if (antaltegn >= length) throw new ArgumentException();
                return rotationer[antaltegn];
            }

            public int CompareTo(object obj)
            {
                return Ord.CompareTo(((KompletOrd) obj).GetValue());
            }

            public int CompareTo(object obj, int index)
            {
                return rotationer[index].CompareTo(((KompletOrd) obj).GetValue(index));
            }

            public bool StartsWith(IStartsWith prefix)
            {
                return Ord.StartsWith(prefix.GetValue());
            }

            public bool StartsWith(IStartsWith prefix, int bogstavNummer)
            {
                if (bogstavNummer > length) throw new ArgumentException();
                return rotationer[bogstavNummer].StartsWith(prefix.GetValue());
            }

            public string GetValue()
            {
                return Ord;
            }

            public string GetValue(int rotation)
            {
                if (rotation > length) throw new ArgumentException();
                return rotationer[rotation];
            }
        }
    }
}
