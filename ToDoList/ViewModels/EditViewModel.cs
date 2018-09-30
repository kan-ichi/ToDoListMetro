using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Codes;
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
        public ReactiveCommand RegisterCommand { get; private set; }
        public ReactiveCollection<TodoTask> DataGridItemsSource { get; private set; }
        public ReactiveProperty<string> SearchConditionsText { get; private set; }
        public ReactiveProperty<DateTime?> SearchConditionsTextDueDateFrom { get; private set; }
        public ReactiveProperty<DateTime?> SearchConditionsTextDueDateTo { get; private set; }
        public ReactiveProperty<DateTime?> DueDate { get; private set; }
        public ReactiveProperty<bool> Status { get; private set; }
        public ReactiveProperty<string> Subject { get; private set; }
        #endregion

        private DataBaseAccessor dbAccessor;
        private TodoTask editingTodoTask;

        public EditViewModel()
        {
            this.InitializeBindings();

            this.dbAccessor = new DataBaseAccessor();
        }

        /// <summary>
        /// ボタン〔検索〕押下処理
        /// </summary>
        private void SearchCommandExecute()
        {
            SqlBuilder.EditViewSearchConditions sc = new SqlBuilder.EditViewSearchConditions();
            sc.Text = this.SearchConditionsText.Value;
            sc.DueDateFrom = this.SearchConditionsTextDueDateFrom.Value;
            sc.DueDateTo = this.SearchConditionsTextDueDateTo.Value;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = this.dbAccessor.TodoTaskSelect(sql);

            this.DataGridItemsSource.Clear();
            foreach (var task in tasks) this.DataGridItemsSource.Add(task);
        }

        /// <summary>
        /// ボタン〔クリア〕押下処理
        /// </summary>
        private void ClearCommandExecute()
        {
            this.SearchConditionsText.Value = string.Empty;
            this.SearchConditionsTextDueDateFrom.Value = null;
            this.SearchConditionsTextDueDateTo.Value = null;
        }

        /// <summary>
        /// データグリッドの行選択時処理
        /// </summary>
        private void DataGridCurrentCellChangedExecute()
        {
            this.editingTodoTask = null;
            foreach (var task in this.DataGridItemsSource) if (task.IsSelected) this.editingTodoTask = task;

            if (this.editingTodoTask != null)
            {
                this.DueDate.Value = this.editingTodoTask.DueDate;
                this.Status.Value = this.editingTodoTask.StatusCode.IsFinished;
                this.Subject.Value = this.editingTodoTask.Subject;
            }
        }

        /// <summary>
        /// ボタン〔登録〕押下処理
        /// </summary>
        private void RegisterCommandExecute()
        {
            this.editingTodoTask.DueDate = this.DueDate.Value;
            if (this.Status.Value)
            {
                this.editingTodoTask.StatusCode.Code = StatusCode.FINISHED;
            }
            else
            {
                this.editingTodoTask.StatusCode.Code = StatusCode.NOT_YET;
            }
            this.editingTodoTask.Subject = this.Subject.Value;

            this.dbAccessor.TodoTaskUpdate(this.editingTodoTask);
            this.SearchCommandExecute();
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

            this.RegisterCommand = new ReactiveCommand();
            this.RegisterCommand.Subscribe(x => RegisterCommandExecute());

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
