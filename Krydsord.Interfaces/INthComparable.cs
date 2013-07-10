using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface INthComparable : IComparable
    {
        int CompareTo(object obj, int index);
        int CompareToPrefix(object obj, int index);
    }
}
