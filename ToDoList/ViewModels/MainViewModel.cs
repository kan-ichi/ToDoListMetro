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
    public class MainViewModel
    {
        #region view binding items
        public ReactiveCollection<TodoTask> DataGridItemsSource { get; private set; }
        public ReactiveCommand DataGridCurrentCellChanged { get; private set; }
        #endregion

        private DataBaseAccessor dbAccessor;

        public MainViewModel()
        {
            this.InitializeBindings();

            this.dbAccessor = new DataBaseAccessor();
            if (!this.dbAccessor.IsExistDataBaseFile()) this.GenerateDataBase();

            this.ReadDatabaseAndShow();
        }

        private void DataGridCurrentCellChangedExecute()
        {
            // under construction
        }

        private void ReadDatabaseAndShow()
        {
            SqlBuilder.MainViewSearchConditions sc = new SqlBuilder.MainViewSearchConditions();
            sc.StatusCodeNotEqual = StatusCode.FINISHED;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = dbAccessor.TodoTaskSelect(sql);

            this.DataGridItemsSource.Clear();
            foreach (var task in tasks) this.DataGridItemsSource.Add(task);
        }

        /// <summary>
        /// ビューにバインドされている項目を初期化します
        /// </summary>
        private void InitializeBindings()
        {
            this.DataGridItemsSource = new ReactiveCollection<TodoTask>();

            this.DataGridCurrentCellChanged = new ReactiveCommand();
            this.DataGridCurrentCellChanged.Subscribe(x => DataGridCurrentCellChangedExecute());
        }

        /// <summary>
        /// データベースを作成し、サンプルデータを追加します
        /// </summary>
        private void GenerateDataBase()
        {
            this.dbAccessor.CreateDataBase();
            List<TodoTask> tasks = new List<TodoTask>();
            tasks.Add(new TodoTask() { Subject = "get up early ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make breakfast ", DueDate = DateTime.Now.AddHours(4), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "take my dog for a walk ", DueDate = DateTime.Now.AddHours(8), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make lunch ", DueDate = DateTime.Now.AddHours(12), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "pick up my child from a nursery school ", DueDate = DateTime.Now.AddHours(16), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "cook dinner ", DueDate = DateTime.Now.AddHours(20), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "sleep early ", DueDate = DateTime.Now.AddHours(24), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            foreach (var task in tasks) this.dbAccessor.TodoTaskInsert(task);
        }

    }
}
