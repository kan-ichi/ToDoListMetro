using Prism.Interactivity.InteractionRequest;
using Prism.InteractivityExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoList.ViewModels;

namespace ToDoList.Views
{
    class MainViewPopupEditViewAction : PopupWindowActionBase
    {
        protected override Window CreateWindow(INotification notification)
        {
            return new PopupEditView();
        }

        protected override void ApplyNotificationToWindow(Window window, INotification notification)
        {
            PopupEditViewModel dataContext = (PopupEditViewModel)window.DataContext;
            dataContext.AssociatedWindow = window;
            dataContext.Notification = notification;
        }
    }
}
