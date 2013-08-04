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
        private int wordCounter = 1;
        private StatefulOrdlisteOrd lastAddedWord;
        public StartsWithKrydsordGenerator(IOrdliste ordliste, IView view)
            : base(ordliste, view)
        {

        }

        protected override void FixKrydsord()
        {
            var latestAffectedOrdlisteOrd = (StatefulOrdlisteOrd) ordlisteOrd[wordCounter - 1];
            if (!latestAffectedOrdlisteOrd.FilterOrdliste(fejlOrd)) // Filtering resulted in no legal words left
            {
                var faultingOrd = latestAffectedOrdlisteOrd;
                bool result = false;
                do
                {
                    --wordCounter;
                    if (wordCounter == 0)
                        throw new Exception("Krydsordslayoutet der resulterer fra seed " + seed +
                                            " kunne ikke indeholde en lovlig krydsord - alle mulige kombinationer forsøgt uden held.");
                    latestAffectedOrdlisteOrd = (StatefulOrdlisteOrd) ordlisteOrd[wordCounter - 1];

                    // This word cannot make the faulting word correct
                    // Reset its word list and move further back
                    if (latestAffectedOrdlisteOrd.row == faultingOrd.row ||
                        (latestAffectedOrdlisteOrd.row == faultingOrd.row - 1 && latestAffectedOrdlisteOrd.column >= faultingOrd.column + faultingOrd.length))
                    {
                        // Commenting out the line below is NOT safe - we might miss possible solutions
                        latestAffectedOrdlisteOrd.ReInitializeOrdliste();
                    }
                    else
                    {
                        result = latestAffectedOrdlisteOrd.Next();
                    }
                } while (!result);
            }
        }

        protected override bool CheckKrydsord()
        {
            bool fejlOrdFundet;
            do
            {
                fejlOrdFundet = false;
                ConvertToResult();
                lastAddedWord = (StatefulOrdlisteOrd)ordlisteOrd[wordCounter - 1];

                fejlOrd = new Dictionary<int, List<char>>();
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
                        fejlOrdFundet = true;
                        break;
                    }
                }
                // The last word added caused no errors, so add another
                if (!fejlOrdFundet) ++wordCounter;
                else
                { // Iterate again to find legal characters for all positions
                    for (var currentColumn = lastAddedWord.column;
                         currentColumn < lastAddedWord.column + lastAddedWord.length;
                         ++currentColumn)
                    {
                        string currentColumnString = lodretKrydsord[currentColumn];
                        int startOfAffectedWord = currentColumnString.Substring(0, lastAddedWord.row).LastIndexOf('#') + 1;
                        var currentColumnWord = currentColumnString.Substring(startOfAffectedWord).Split('#').First();

                        var fixedPart = currentColumnWord.Trim('.');
                        fejlOrd[currentColumn - lastAddedWord.column].AddRange(
                            FindMuligeBogstaver(fixedPart.Substring(0, fixedPart.Length - 1), currentColumnWord.Length));
                    }
                }
            } while (!fejlOrdFundet && wordCounter <= ordlisteOrd.Count);
            if (fejlOrdFundet) return false;
            DanLodretKrydsord(true);
            return true;
        }

        // If we know the list of possible letters in ordlisten, we could make this faster for large lists
        // by iterating over the possible letters and checking for AnyStartsWith for each
        private IEnumerable<char> FindMuligeBogstaver(string substring, int wordLength)
        {
            return ordliste.GetOrdDerStarterMed(substring, wordLength).Select(ord => ord[substring.Length]).Distinct();
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
                    currentChar += length + 1;
                }
            }
            ConvertToResult();
        }

        protected class StatefulOrdlisteOrd : OrdlisteOrd
        {
            private string initialOrd;
            public int row;
            public int column;
            private IOrdliste ordliste;
            private OrdListeTreeStructure ordListeTree;

            public StatefulOrdlisteOrd(int index, string ord, int wordcountOfThisLength, int row, int column, IOrdliste ordliste)
                : base(index, ord, wordcountOfThisLength)
            {
                this.row = row;
                this.column = column;
                this.ordliste = ordliste;
                initialOrd = ord;
                ordListeTree = new OrdListeTreeStructure();
                InitializeOrdliste();
            }

             private void InitializeOrdliste()
             {
                 Ord = initialOrd;
                 ordListeTree.Initialize(ordliste.GetOrd(length));
             }

             public void ReInitializeOrdliste()
             {
                 foreach (TreeStructureElement element in ordListeTree.Values)
                 {
                     element.ForceEnable();
                 }
                 Ord = initialOrd;
             }

            public bool FilterOrdliste(Dictionary<int, List<char>> letters)
            {
                foreach (char letter in ordListeTree.Keys.ToList())
                {
                    if (!letters[0].Contains(letter)) ordListeTree.Disable(letter);
                    else
                    {
                        if (!InnerFilterOrdliste(1, letters, ordListeTree[letter].Tree)) ordListeTree.Disable(letter); // InnerFilter emptied the branch
                    }
                }
                if (!ordListeTree.AnyEnabled)
                {
                    ordListeTree.ReInitialize();
                    return false;
                }
                SetWord();
                return true;
            }

            private bool InnerFilterOrdliste(int level, Dictionary<int, List<char>> letters, OrdListeTreeStructure tree)
            {
                if (!letters.ContainsKey(level)) return true; // Leaf node
                foreach (char letter in tree.Keys.ToList())
                {
                    if (!letters[level].Contains(letter)) tree.Disable(letter);
                    else
                    {
                        if (!InnerFilterOrdliste(level + 1, letters, tree[letter].Tree)) // InnerFilter emptied the branch
                            tree.Disable(letter);
                    }
                }
                return tree.AnyEnabled;
            }

            private void SetWord()
            {
                var chars = new char[length];
                var currentChars = Ord.ToCharArray();
                var currentTree = ordListeTree;
                int counter = 0;
                while (currentTree.AnyEnabled)
                {
                    var largerElements = currentTree.SkipWhile(x => x.Key != currentChars[counter]).ToList();
                    if (largerElements.Any(x => x.Value.Enabled)) {
                        chars[counter++] = largerElements.First(x => x.Value.Enabled).Key;
                        currentTree = largerElements.First(x => x.Value.Enabled).Value.Tree;
                    }
                    else {
                        chars[counter++] = currentTree.First(x => x.Value.Enabled).Key;
                        currentTree = currentTree.First(x => x.Value.Enabled).Value.Tree;
                    }
                }
                Ord = new string(chars);
            }

            public bool Next()
            {
                if (!ordListeTree.RemoveWord(new Queue<char>(Ord)))
                {
                    ordListeTree.ReInitialize();
                    return false;
                }
                SetWord();
                return true;
            }

            private string currentWord;
            public string CurrentWord
            {
                get { return currentWord; }
            }
        }

        [Serializable]
        private class OrdListeTreeStructure : Dictionary<char, TreeStructureElement>
        {
            public void Initialize(IEnumerable<string> ordliste)
            {
                Clear();
                foreach (string ord in ordliste)
                {
                    var currentlevel = this;
                    foreach (char c in ord)
                    {
                        if (!currentlevel.ContainsKey(c)) currentlevel.Add(c, new TreeStructureElement(true, new OrdListeTreeStructure()));
                        currentlevel = currentlevel[c].Tree;
                    }
                }
            }

            // This method assumes all branches below the first level has Enabled set to true
            // Thus it should only be called when a call to RemoveFirst returns false.
            public void ReInitialize()
            {
                foreach (TreeStructureElement element in Values) {
                    element.Enabled = true;
                }
                // if (!CheckInvariant()) throw new Exception("TreeStructure not cleaned up correctly");
            }

            public bool RemoveWord(Queue<char> ord)
            {
                if (!ord.Any()) return false;
                var currentElement = this[ord.Dequeue()];
                if (!currentElement.Tree.RemoveWord(ord))
                {
                    currentElement.Enabled= false;
                    
                    // Reset the Enabled flags, so we can reuse the structure later
                    foreach (TreeStructureElement element in currentElement.Tree.Values)
                    {
                        element.Enabled = true;
                    }
                }
                return this.AnyEnabled;
            }

            public void Disable(char c){
                this[c].Disable();

            }

            public void Enable(char c)
            {
                this[c].Enable();
            }

            public bool AnyEnabled
            {
                get
                {
                    return Values.Any(x => x.Enabled);
                }
            }

            public bool CheckInvariant()
            {
                foreach (TreeStructureElement element in Values)
                {
                    if (!(element.Enabled && element.Tree.CheckInvariant())) return false;
                }
                return true;
            }
        }

        private class TreeStructureElement
        {
            public bool Enabled;
            public OrdListeTreeStructure Tree;

            public TreeStructureElement(bool enabled, OrdListeTreeStructure tree)
            {
                Enabled = enabled;
                Tree = tree;
            }

            public void ForceEnable()
            {
                Enabled = true;
                foreach (TreeStructureElement element in Tree.Values)
                {
                    element.Enable();
                }
            }

            public void Enable()
            {
                if (!Enabled)
                {
                    Enabled = true;
                    foreach (TreeStructureElement element in Tree.Values)
                    {
                        element.Enable();
                    }
                }
            }

            public void Disable()
            {
                if (Enabled)
                {
                    Enabled = false;
                    foreach (TreeStructureElement element in Tree.Values)
                    {
                        element.Enable();
                    }
                }
            }
        }
    }
}
