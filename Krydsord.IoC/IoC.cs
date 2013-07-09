using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Krydsord.Generator;
using Krydsord.Interfaces;
using Krydsord.Views;

namespace Krydsord.IoC
{
    public class IoC
    {
        public static IWindsorContainer BootstrapContainer()
        {
            //return new WindsorContainer().Install(Configuration.FromAppConfig(), FromAssembly.This());
            var container = new WindsorContainer();
            container.Register(Component.For<IKrydsordGenerator>().ImplementedBy<StartsWithKrydsordGenerator>());
            container.Register(Component.For<IOrdliste>().ImplementedBy<SortedOrdliste>());
            container.Register(Component.For<IRepository<List<string>>>().ImplementedBy<OrdlisteFileRepository>());
            container.Register(Component.For<IView>().ImplementedBy<ConsoleView>());
            return container;
        }
    }
}
