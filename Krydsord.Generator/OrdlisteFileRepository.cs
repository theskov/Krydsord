using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public class OrdlisteFileRepository : IRepository<List<string>>
    {
        private const string firstWordPattern = @"^(\w+)\s+\S+\s+(.*)";
        private const string restOfWordsPattern = @"\w+";
        private bool outputToDisplay = false;

        public List<string> Get(string sti)
        {
            var result = new List<string>();
            string[] lines = System.IO.File.ReadAllLines(sti);
            foreach (var line in lines)
            {
                parseAndAddLine(line, result);
            }
            return result;
        }

        private void parseAndAddLine(string line, List<string> ordliste)
        {
            var firstWordMatch = Regex.Match(line, firstWordPattern);
            if (firstWordMatch.Success)
            {
                Add(ordliste, firstWordMatch.Groups[1].Value);
                var restOfWordsMatches = Regex.Matches(firstWordMatch.Groups[2].Value, restOfWordsPattern);
                foreach (Match match in restOfWordsMatches)
                {
                    Add(ordliste, match.Value);
                }
            }
        }

        private void Add(List<string> ordliste, string ord)
        {
            if (outputToDisplay) Console.WriteLine(ord);
            ordliste.Add(ord);
        }
    }
}
