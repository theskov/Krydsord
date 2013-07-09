using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface IStartsWith
    {
        bool StartsWith(IStartsWith prefix);
        bool StartsWith(IStartsWith prefix, int bogstavNummer);
        string GetValue();
        string GetValue(int rotation);
    }
}
