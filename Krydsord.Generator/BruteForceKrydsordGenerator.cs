using System;
using System.Linq;
using System.Collections.Generic;
using Krydsord.Helpers;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public class BruteForceKrydsordGenerator : DanVandretCheckLodretKrydsordGenerator
    {
        private KeyValuePair<int, int> affectedItem;
        private int wordCounter = 1;
        private PlacedOrdlisteOrd lastAddedWord;
        public BruteForceKrydsordGenerator(IOrdliste ordliste, IView view) : base(ordliste, view)
        {

        }

        protected override void FixKrydsord()
        {
            OrdlisteOrd latestAffectedOrdlisteOrd = ordlisteOrdPrLinje[affectedItem.Key][affectedItem.Value];
            RollIndex(ordlisteOrd.IndexOf(latestAffectedOrdlisteOrd));

        }

        private void RollIndex(int ordIndex)
        {
            for (int ordnr = ordIndex; ordnr >= 0; --ordnr)
            {
                if (!ordlisteOrd[ordnr].RollIndex(ordliste))
                {
                    return;
                }
                --wordCounter;
            }
            throw new Exception("Krydsordslayoutet der resulterer fra seed " + seed + " kunne ikke indeholde en lovlig krydsord - alle mulige kombinationer forsøgt uden held.");
        }

        protected override bool CheckKrydsord()
        {
            do
            {
                ConvertToResult();
                lastAddedWord = (PlacedOrdlisteOrd)ordlisteOrd[wordCounter-1];
                DanLodretKrydsord(lastAddedWord.column + lastAddedWord.length, lastAddedWord.row);

                for (var currentColumn = lastAddedWord.column; currentColumn < lastAddedWord.column + lastAddedWord.length; ++currentColumn)
                {
                    string currentColumnString = lodretKrydsord[currentColumn];
                    int startOfAffectedWord = currentColumnString.Substring(0, lastAddedWord.row).LastIndexOf('#') + 1;
                    var currentColumnWord = currentColumnString.Substring(startOfAffectedWord).Split('#').First();

                    var fixedPart = currentColumnWord.Trim('.');

                    if (!ordliste.OrdDerStarterMed(fixedPart, currentColumnWord.Length))
                    {
                        affectedItem = new KeyValuePair<int, int>(lastAddedWord.row, ordlisteOrdPrLinje[lastAddedWord.row].IndexOf(lastAddedWord));
                        return false;
                    }
                }
                // The last word added caused no errors, so add another
                ++wordCounter;
            } while (wordCounter <= ordlisteOrd.Count);
            affectedItem = new KeyValuePair<int, int>(width, height);
            DanLodretKrydsord(true);
            return true;
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
                    var nytOrd = new PlacedOrdlisteOrd(ordindex, nextOrd, ordliste.GetOrd(length).Count, currentLine, currentChar);
                    ordlisteOrdPrLinje[currentLine].Add(nytOrd);
                    ordlisteOrd.Add(nytOrd);
                    KeyValuePair<int, int> dummy;
                    currentChar += length + 1;
                }
            }
            ConvertToResult();
        }

        protected class PlacedOrdlisteOrd : OrdlisteOrd
        {
            public int row;
            public int column;

            public PlacedOrdlisteOrd(int index, string ord, int wordcountOfThisLength, int row, int column) : base(index, ord, wordcountOfThisLength)
            {
                this.row = row;
                this.column = column;
            }
        }
    }
}
