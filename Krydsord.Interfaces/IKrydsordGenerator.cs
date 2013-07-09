using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface IKrydsordGenerator
    {
        void Initialize(int width, int height, string sti);
        char[][] Generate(int seed);
    }
}
