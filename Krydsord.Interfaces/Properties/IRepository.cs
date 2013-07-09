using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface IRepository<T>
    {
        T Get(string sti);
    }
}
