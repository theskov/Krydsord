using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krydsord.Interfaces
{
    public interface IView
    {
        void Initialize(int updateInterval);
        void Display(char[][] krydsord, bool force = false);
        void Display(string[] krydsord, int x, int y, bool force = false);
        void TextLine(string text, bool force = false);
        void Text(string text, int x, int y, bool force = false);
    }
}
