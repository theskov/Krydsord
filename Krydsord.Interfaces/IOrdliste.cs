using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface IOrdliste : IList<string>
    {
        void Initialize(string sti, int maxLength, int skipEvery = 1);
        IList<string> GetOrd(int length, int sortedByNthBogstav);
        IList<string> GetOrd(int length);
        bool OrdDerStarterMed(string s);
        bool OrdDerStarterMed(string s, int length);
        IList<string> GetOrdDerStarterMed(string s, int length);
    }
}
