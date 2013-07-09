using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krydsord.Helpers;
using Krydsord.Interfaces;

namespace Krydsord.Generator
{
    public abstract class BaseKrydsordGenerator : IKrydsordGenerator
    {
        protected IOrdliste ordliste;
        protected IView view;
        protected int width;
        protected int height;
        protected RandomHelper randomHelper;
        protected int seed;
        protected char[][] krydsord;

        protected const int maxOrdLength = 25;

        public BaseKrydsordGenerator(IOrdliste ordliste, IView view)
        {
            this.ordliste = ordliste;
            this.view = view;
        }

        public void Initialize(int width, int height, string sti)
        {
            ordliste.Initialize(sti, maxOrdLength, 1); // DEBUG
            this.width = width;
            this.height = height;
            Initialize();
        }

        protected abstract void Initialize();

        public abstract char[][] Generate(int seed);
    }
}
