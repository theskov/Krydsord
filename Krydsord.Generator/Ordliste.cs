using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Interfaces;
using Krydsord.Helpers;

namespace Krydsord
{
    public class Ordliste : List<string>, IOrdliste
    {
        private IView view;
        private readonly IRepository<List<string>> repository;
        private Dictionary<int, Dictionary<int, List<string>>> ordByLength;
        private int maxLength;
        private int skipEvery;
        public IList<string> GetOrd(int length, int sortedByNthBogstav)
        {
            if (sortedByNthBogstav > length) throw new ArgumentException();
            return ordByLength[length][sortedByNthBogstav];
        }

        public IList<string> GetOrd(int length)
        {
            return GetOrd(length, 1);
        }

        public Ordliste(IRepository<List<string>> repository, IView view)
        {
            this.repository = repository;
            this.view = view;
        }

        // NOTE: With this constructor, we don't have a repository so calls to Initialize will fail
        private Ordliste(IEnumerable<string> ord, int maxlength, IView view)
        {
            maxLength = maxlength;
            this.view = view;
            AddRange(ord);
            InitializeOrdByLength();
        }

        public void Initialize(string sti, int maxlength, int skipEvery = 1)
        {
            maxLength = maxlength;
            this.skipEvery = skipEvery;
            Clear();
            view.Text("Henter ordliste fra fil:", 1, 20, true);
            if (skipEvery != 1)
            {
                AddRange(repository.Get(sti).Where((ord, index) => index % skipEvery == 0).Distinct());
            }
            else AddRange(repository.Get(sti).Select(ord => ord.ToLower()).Distinct(StringComparer.InvariantCultureIgnoreCase));
            view.TextLine("Antal Ord: " + Count , true);
            view.TextLine("Danner ordlister pr længde:", true);
            InitializeOrdByLength();
        }

        private void InitializeOrdByLength()
        {
            ordByLength = new Dictionary<int, Dictionary<int, List<string>>>(maxLength);
            for (int length = 1; length <= maxLength; ++length)
            {
                ordByLength[length] = new Dictionary<int, List<string>>(length);
                List<string> ordCurrentLength = this.Where(ord => ord.Length == length).ToList();
                ordByLength[length].Add(1, ordCurrentLength);
                view.TextLine(length.ToString() + ": " + ordCurrentLength.Count, true);
                for (int currentLetter = 2; currentLetter <= length; ++currentLetter)
                {
                    ordByLength[length].Add(currentLetter, ordCurrentLength.OrderBy(s => s.Rotate(currentLetter - 1), StringComparer.InvariantCultureIgnoreCase).ToList());
                }
            }
        }

        public bool OrdDerStarterMed(string s)
        {
            return this.Any(ord => ord.StartsWith(s));
        }

        public bool OrdDerStarterMed(string s, int length)
        {
            return this.Where(ord => ord.Length == length).Any(ord => ord.StartsWith(s));
        }

        public IList<string> GetOrdDerStarterMed(string s, int length)
        {
            throw new NotImplementedException();
        }

        public List<char> UniqueLetters { get; private set; }
    }
}
