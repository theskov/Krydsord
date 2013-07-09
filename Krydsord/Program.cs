using System;
using System.Linq;

using Krydsord.Interfaces;

using Castle.Windsor;


namespace Krydsord
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IWindsorContainer container = IoC.IoC.BootstrapContainer())
            {
                var ordlisteSti = args.FirstOrDefault() ?? @"C:\Users\Skov\Documents\infl.txt";
                var generator = container.Resolve<IKrydsordGenerator>();
                generator.Initialize(8, 8, ordlisteSti);
                generator.Generate(123);
                Console.WriteLine("Eksekvering afsluttet - tryk en tast for at forsætte");
                Console.ReadKey();
            }
        }
    }
}
