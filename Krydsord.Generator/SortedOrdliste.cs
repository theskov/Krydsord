using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Helpers;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public class SortedOrdliste : List<string>, IOrdliste
    {
        private int maxLength;
        private int skipEvery;
        private readonly IRepository<List<string>> repository;
        private readonly IView view;
        private Dictionary<int, Dictionary<int, List<string>>> ordByLength;

       public SortedOrdliste(IRepository<List<string>> repository, IView view)
        {
            this.repository = repository;
            this.view = view;
        }

        public SortedOrdliste(List<string> ord) : base(ord)
        {
            InitializeOrdByLength();
        }

        public void Initialize(string sti, int maxLength, int skipEvery = 1)
        {
            this.maxLength = maxLength;
            this.skipEvery = skipEvery;
            Clear();
            view.Text("Henter ordliste fra fil:", 1, 20, true);
            var ord = repository.Get(sti).Select(o => o.ToLower()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            for (int i = 0; i < ord.Count(); i+=skipEvery)
            {
                Add(ord[i]);
            }
            view.TextLine("Antal ord: " + Count, true);
            view.TextLine("Danner ordlister pr længde:", true);
            InitializeOrdByLength();
        }

        private void InitializeOrdByLength()
        {
            ordByLength = new Dictionary<int, Dictionary<int, List<string>>>(maxLength);
            for (int length = 1; length <= maxLength; ++length)
            {
                var innerLength = length;
                ordByLength[innerLength] = new Dictionary<int, List<string>>(innerLength);
                List<string> ordCurrentLength = new List<string>();
                foreach (var ord in this.Where(ord => ord.Length == innerLength))
                {
                    ordCurrentLength.Add(ord);
                }
                ordCurrentLength.Sort();
                ordByLength[innerLength].Add(1, ordCurrentLength);
                view.TextLine(innerLength.ToString() + ": " + ordCurrentLength.Count, true);
                for (int currentLetter = 2; currentLetter <= length; ++currentLetter)
                {
                    ordByLength[innerLength].Add(currentLetter, new List<string>());
                    foreach (var ord in ordCurrentLength)
                    {
                        ordByLength[innerLength][currentLetter].Add(ord.Rotate(currentLetter - 1));
                    }
                    ordByLength[innerLength][currentLetter].Sort();
                }
            }
        }

        public IList<string> GetOrd(int length, int sortedByNthBogstav)
        {
            return ordByLength[length][sortedByNthBogstav];
        }

        public IList<string> GetOrd(int length)
        {
            return ordByLength[length][1];
        }

        public bool OrdDerStarterMed(string s)
        {
            return this.AnyStartsWith(s);
        }

        public bool OrdDerStarterMed(string s, int length)
        {
            return ordByLength[length][1].AnyStartsWith(s);
        }

        public IList<string> GetOrdDerStarterMed(string s, int length)
        {
            return ordByLength[length][1].StartsWith(s);
        }
    }
}
