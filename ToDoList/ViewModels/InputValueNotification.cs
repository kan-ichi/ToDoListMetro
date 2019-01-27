using Prism.Interactivity.InteractionRequest;

namespace ToDoList.ViewModels
{
    public class InputValueNotification : Notification
    {
        public object RequestValue { get; set; }
        public object ReturnValue { get; set; }
    }
}
