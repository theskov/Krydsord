using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public class OrdFilterKrydsordGenerator : DanVandretCheckLodretKrydsordGenerator
    {
        private KeyValuePair<int, int> affectedItem;
        public OrdFilterKrydsordGenerator(IOrdliste ordliste, IView view) : base(ordliste, view) { }


        protected override void FixKrydsord()
        {
            for (int currentRow = height - 1; currentRow > affectedItem.Key; --currentRow)
            {
                foreach (var currentOrdlisteOrd in ordlisteOrdPrLinje[currentRow])
                {
                    currentOrdlisteOrd.ResetIndex();
                }
            }
            int columnIndex = 0;
            OrdlisteOrd latestAffectedOrdlisteOrd = null;
            foreach (var currentOrdlisteOrd in ordlisteOrdPrLinje[affectedItem.Key])
            {
                if (columnIndex > affectedItem.Value) currentOrdlisteOrd.ResetIndex();
                else latestAffectedOrdlisteOrd = currentOrdlisteOrd;
                columnIndex += currentOrdlisteOrd.length;
            }

            for (int ordnr = ordlisteOrd.IndexOf(latestAffectedOrdlisteOrd); ordnr >= 0; --ordnr)
            {
                if (!ordlisteOrd[ordnr].RollIndex(ordliste))
                {
                    ConvertToResult();
                    return;
                }
            }
            throw new Exception("Krydsordslayoutet der resulterer fra seed " + this.seed + " kunne ikke indeholde en lovlig krydsord - alle mulige kombinationer forsøgt uden held.");
        }

        protected override bool CheckKrydsord()
        {
            int lowestRowNecessaryToChange = height;
            int lowestColumnNecessaryToChange = width;
            DanLodretKrydsord();
            for (var currentColumn = 0; currentColumn < height; ++currentColumn)
            {
                int rowindex = 0;
                string currentColumnString = lodretKrydsord[currentColumn];
                var currentColumnWords = currentColumnString.Split('#');
                foreach (var currentColumnWord in currentColumnWords)
                {
                    if (currentColumnWord.Length == 0)// Two sequential #'s
                    {
                        ++rowindex;
                        continue;
                    }

                    if (!ordliste.GetOrd(currentColumnWord.Length).Contains(currentColumnWord))
                    {
                        int latestRowInWord = rowindex + currentColumnWord.Length - 1;
                        if (latestRowInWord < lowestRowNecessaryToChange)
                        {
                            lowestRowNecessaryToChange = latestRowInWord;
                            lowestColumnNecessaryToChange = currentColumn;
                        }
                    };
                }
            }
            affectedItem = new KeyValuePair<int, int>(lowestRowNecessaryToChange, lowestColumnNecessaryToChange);
            return lowestRowNecessaryToChange == height && lowestColumnNecessaryToChange == width;
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
                    var nytOrd = new OrdlisteOrd(ordindex, nextOrd, ordliste.GetOrd(length).Count);
                    ordlisteOrdPrLinje[currentLine].Add(nytOrd);
                    ordlisteOrd.Add(nytOrd);
                    currentChar += length + 1;
                }
            }
            ConvertToResult();
        }
    }
}
