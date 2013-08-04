using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Interfaces;

namespace Krydsord.Views
{
    public class ConsoleView : IView
    {
        private int updateInterval = 197;
        private int updateCounter = 0;
        public void Initialize(int updateInterval)
        {
            this.updateInterval = updateInterval;
        }

        public void Display(char[][] krydsord, bool force = false)
        {
            if (updateCounter++%updateInterval == 0 || force)
            {
                Console.SetCursorPosition(0, 1);
                foreach (var chars in krydsord)
                {
                    Console.WriteLine(new string(chars));
                }
            }
        }

        public void Display(string[] krydsord, int x, int y, bool force = false)
        {
            if (updateCounter++ % updateInterval == 0 || force)
            {
                foreach (var text in krydsord)
                {
                    Console.SetCursorPosition(x, y++);
                    Console.WriteLine(text);
                }
            }
        }

        public void TextLine(string text, bool force = false)
        {
            if (updateCounter++ % updateInterval == 0 || force)
            {
                Console.WriteLine(text);

            }
        }

        public void Text(string text, int x, int y, bool force = false)
        {
            if (updateCounter++ % updateInterval == 0 || force)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine(text);
                
            }
        }
    }
}
