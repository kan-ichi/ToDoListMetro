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
        private DataBaseAccessor DataBaseManager;

        public ReactiveCollection<TodoTask> DataGridItemsSource { get; private set; }
        public ReactiveCommand DataGridCurrentCellChanged { get; private set; }

        public MainViewModel()
        {
            this.DataBaseManager = new DataBaseAccessor();
            if (!this.DataBaseManager.IsExistDataBaseFile()) this.GenerateDataBase();

            this.DataGridItemsSource = new ReactiveCollection<TodoTask>();
            this.ReadDatabaseAndShow();

            this.DataGridCurrentCellChanged = new ReactiveCommand();
            this.DataGridCurrentCellChanged.Subscribe(x => DataGridCurrentCellChangedExecute());
        }

        private void DataGridCurrentCellChangedExecute()
        {
            // 未実装
        }

        private void ReadDatabaseAndShow()
        {
            SqlBuilder.MainViewSearchConditions sc = new SqlBuilder.MainViewSearchConditions();
            sc.StatusCodeNotEqual = StatusCode.FINISHED;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = DataBaseManager.TodoTaskSelect(sql);

            this.DataGridItemsSource.Clear();
            foreach (var task in tasks) this.DataGridItemsSource.Add(task);
        }

        private void GenerateDataBase()
        {
            this.DataBaseManager.CreateDataBase();
            List<TodoTask> tasks = new List<TodoTask>();
            tasks.Add(new TodoTask() { Subject = "get up early ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make breakfast ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "take my dog for a walk ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make lunch ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "pick up my child from a nursery school ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "cook dinner ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "sleep early ", DueDate = DateTime.Now, StatusCode = new StatusCode(StatusCode.NOT_YET) });
            foreach (var task in tasks) this.DataBaseManager.TodoTaskInsert(task);
        }

    }
}
