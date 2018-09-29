using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Codes;

namespace ToDoList.Models.Entities
{
    public class TodoTask : BindableBase
    {
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        private string _ID;
        public string ID
        {
            get { return _ID; }
            set { this.SetProperty(ref this._ID, value); }
        }

        private DateTime _CreatedAt;
        public DateTime CreatedAt
        {
            get { return _CreatedAt; }
            set { this.SetProperty(ref this._CreatedAt, value); }
        }

        private DateTime _UpdatedAt;
        public DateTime UpdatedAt
        {
            get { return _UpdatedAt; }
            set { this.SetProperty(ref this._UpdatedAt, value); }
        }

        private string _Subject;
        public string Subject
        {
            get { return _Subject; }
            set { this.SetProperty(ref this._Subject, value); }
        }

        private DateTime? _DueDate;
        public DateTime? DueDate
        {
            get { return _DueDate; }
            set { this.SetProperty(ref this._DueDate, value); }
        }

        private StatusCode _StatusCode;
        public StatusCode StatusCode
        {
            get { return _StatusCode; }
            set { this.SetProperty(ref this._StatusCode, value); }
        }
    }
}
