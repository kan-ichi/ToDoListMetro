using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.DataAccess;
using ToDoList.Models.Entities;

namespace ToDoList.ViewModels
{
    class EditViewModel
    {
        #region view binding items
        public ReactiveCommand SearchCommand { get; private set; }
        public ReactiveCommand DataGridCurrentCellChanged { get; private set; }
        public ReactiveCommand ClearCommand { get; private set; }
        public ReactiveCollection<TodoTask> DataGridItemsSource { get; private set; }
        public ReactiveProperty<string> SearchConditionsText { get; private set; }
        public ReactiveProperty<DateTime?> SearchConditionsTextDueDateFrom { get; private set; }
        public ReactiveProperty<DateTime?> SearchConditionsTextDueDateTo { get; private set; }
        public ReactiveProperty<DateTime?> DueDate { get; private set; }
        public ReactiveProperty<bool> Status { get; private set; }
        public ReactiveProperty<string> Subject { get; private set; }
        #endregion

        private DataBaseAccessor dbAccessor;

        public EditViewModel()
        {
            this.InitializeBindings();

            this.dbAccessor = new DataBaseAccessor();
        }

        private void DataGridCurrentCellChangedExecute()
        {
            // under construction
        }

        private void SearchCommandExecute()
        {
            SqlBuilder.EditViewSearchConditions sc = new SqlBuilder.EditViewSearchConditions();
            sc.Text = this.SearchConditionsText.Value;
            sc.DueDateFrom = this.SearchConditionsTextDueDateFrom.Value;
            sc.DueDateTo = this.SearchConditionsTextDueDateTo.Value;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = dbAccessor.TodoTaskSelect(sql);

            this.DataGridItemsSource.Clear();
            foreach (var task in tasks) this.DataGridItemsSource.Add(task);
        }

        private void ClearCommandExecute()
        {
            this.SearchConditionsText.Value = string.Empty;
            this.SearchConditionsTextDueDateFrom.Value = null;
            this.SearchConditionsTextDueDateTo.Value = null;
        }

        /// <summary>
        /// ビューにバインドされている項目を初期化します
        /// </summary>
        private void InitializeBindings()
        {
            this.SearchCommand = new ReactiveCommand();
            this.SearchCommand.Subscribe(x => SearchCommandExecute());

            this.ClearCommand = new ReactiveCommand();
            this.ClearCommand.Subscribe(x => ClearCommandExecute());

            this.DataGridCurrentCellChanged = new ReactiveCommand();
            this.DataGridCurrentCellChanged.Subscribe(x => DataGridCurrentCellChangedExecute());

            this.DataGridItemsSource = new ReactiveCollection<TodoTask>();

            this.SearchConditionsText = new ReactiveProperty<string>();
            this.SearchConditionsTextDueDateFrom = new ReactiveProperty<DateTime?>();
            this.SearchConditionsTextDueDateTo = new ReactiveProperty<DateTime?>();
            this.DueDate = new ReactiveProperty<DateTime?>();
            this.Status = new ReactiveProperty<bool>();
            this.Subject = new ReactiveProperty<string>();
        }
    }
}
