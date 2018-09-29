using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using System.Linq;
using System.Windows;
using ToDoList.Views;

namespace ToDoList
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            ((Window)this.Shell).Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterTypes(
                AllClasses.FromLoadedAssemblies()
                    .Where(x => x.Namespace.EndsWith(".Views")),
                getFromTypes: _ => new[] { typeof(object) },
                getName: WithName.TypeName);

            var catalog = (ModuleCatalog)this.ModuleCatalog;
            catalog.AddModule(typeof(InitializeModule));

        }
    }
}
