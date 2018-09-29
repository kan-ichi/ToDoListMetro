using Microsoft.Practices.Unity;
using ToDoList.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Collections.ObjectModel;

namespace ToDoList.ViewModels
{
    class ShellViewModel
    {
        [Dependency]
        public IRegionManager RegionManager { get; set; }

        public class ScreenItem
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        public ReadOnlyCollection<ScreenItem> ListBoxItemsSource { get; private set; }

        public ReactiveProperty<ScreenItem> ListBoxSelectedItem { get; private set; }

        public ReactiveCommand ListBoxSelectionChanged { get; private set; }

        public ShellViewModel()
        {
            this.ListBoxSelectedItem = new ReactiveProperty<ScreenItem>();
            this.ListBoxItemsSource = new ReadOnlyCollection<ScreenItem>(CreateScreenItemList());

            this.ListBoxSelectionChanged = new ReactiveCommand();
            this.ListBoxSelectionChanged.Subscribe((_) => this.ListBoxSelectionChangedExecute());
        }

        private void ListBoxSelectionChangedExecute()
        {
            var selected = this.ListBoxSelectedItem.Value;
            var selectedScreenType = selected.Type.Name;
            this.RegionManager.RequestNavigate("MainRegion", selectedScreenType);
        }

        private List<ScreenItem> CreateScreenItemList()
        {
            return new List<ScreenItem>
            {
                new ScreenItem { Name = "メイン  ", Type = typeof(MainView) },
                new ScreenItem { Name = "編集  ", Type = typeof(EditView) }
            };
        }

    }
}
