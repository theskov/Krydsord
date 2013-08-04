using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Helpers;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public abstract class DanVandretCheckLodretKrydsordGenerator : BaseKrydsordGenerator
    {
        
        protected string[] lodretKrydsord;

        protected List<List<OrdlisteOrd>> ordlisteOrdPrLinje;
        protected List<OrdlisteOrd> ordlisteOrd;

        public DanVandretCheckLodretKrydsordGenerator(IOrdliste ordliste, IView view) : base(ordliste, view) { }

        protected override void Initialize()
        {
            krydsord = new char[height][];
            lodretKrydsord = new string[width];

            for (int krydsordLinje = 0; krydsordLinje < height; ++krydsordLinje)
            {
                krydsord[krydsordLinje] = new char[width];
            }

            ordlisteOrd = new List<OrdlisteOrd>();
            ordlisteOrdPrLinje = new List<List<OrdlisteOrd>>();
            for (var i = 0; i < height; ++i)
            {
                ordlisteOrdPrLinje.Add(new List<OrdlisteOrd>());
            }
        }

        public override char[][] Generate(int seed)
        {
            this.seed = seed;
            randomHelper = new RandomHelper(seed);
            int iterations = 0;
            InitializeKrydsord();
            view.Display(krydsord, true);
            while (!CheckKrydsord())
            {
                view.Text(++iterations + " iterationer", 1, height + 3);
                view.Display(krydsord);
                FixKrydsord();
            }
            view.Display(krydsord, true);
            return krydsord;
        }

        protected abstract bool CheckKrydsord();

        protected abstract void FixKrydsord();


        protected void DanLodretKrydsord(bool force = false)
        {
            DanLodretKrydsord(int.MaxValue, int.MaxValue, force);
        }

        // Very naïve implementation
        protected void DanLodretKrydsord(int maxX, int maxY, bool force = false)
        {
            for (var currentColumn = 0; currentColumn < height; ++currentColumn)
            {
                char[] ColumnArray = new char[height];
                for (var currentRow = 0; currentRow < width; ++currentRow)
                {
                    if ((currentColumn > maxX - 1 && currentRow >= maxY) || currentRow > maxY) 
                        ColumnArray[currentRow] = krydsord[currentRow][currentColumn] == '#' ? '#' : '.'; // Replace chars with dots if the words haven't been placed.
                    else ColumnArray[currentRow] = krydsord[currentRow][currentColumn];
                }
                lodretKrydsord[currentColumn] = new string(ColumnArray);
            }
            view.Display(lodretKrydsord, width + 3, 1, force);
        }

       

        protected abstract void InitializeKrydsord();

        protected void ConvertToResult()
        {
            for (var currentLine = 0; currentLine < height; ++currentLine)
            {
                krydsord[currentLine] = String.Join("#", ordlisteOrdPrLinje[currentLine].Select(olo => olo.Ord)).PadRight(width, '#').ToCharArray();
            }
        }

        // Class that enables us to keep track, of where in the ordliste a word comes from
        // Thus, when iterating over possible words, we can pick up at the right place

        protected class OrdlisteOrd
        {
            public OrdlisteOrd(int index, string ord, int wordcountOfThisLength)
            {
                initialIndex = index;
                this.index = index;
                Ord = ord;
                wordCountOfThisLength = wordcountOfThisLength;
                length = ord.Length;
            }

            public bool RollIndex(IOrdliste ordliste)
            {
                index = ++index % wordCountOfThisLength;
                Ord = ordliste.GetOrd(length)[index];
                return index == initialIndex;
            }

            public void ResetIndex()
            {
                this.index = initialIndex;
            }

            public int index;
            private int initialIndex;
            private int wordCountOfThisLength;
            public int length;
            private string ord;
            public virtual string Ord
            {
                get { return ord; }
                protected set { ord = value; }
            }
        }
    }
}
