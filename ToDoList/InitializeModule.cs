using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using ToDoList.Views;

namespace ToDoList
{
    class InitializeModule : IModule
    {
        [Dependency]
        public IRegionManager RegionManager { get; set; }

        public void Initialize()
        {
            this.RegionManager.RequestNavigate("MainRegion", typeof(MainView).Name);
        }
    }
}
