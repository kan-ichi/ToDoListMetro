using Prism.Regions;
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
    public class MainViewModel : INavigationAware
    {
        #region view binding items

        public ReactiveCommand FinishButtonCommand { get; private set; }
        public ReactiveCollection<TodoTask> DataGridItemsSource { get; private set; }

        #endregion

        #region 画面遷移時の処理

        public string ScreenId { get; private set; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.ScreenId = navigationContext.Parameters["ScreenId"] as string;
        }

        /// <summary>
        /// この画面に遷移して来たタイミングで処理を行います
        /// </summary>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            this.SearchExecute();
            return this.ScreenId == navigationContext.Parameters["ScreenId"] as string;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // nothing to do
        }

        #endregion

        #region クラス内変数・コンストラクタ

        private DataBaseAccessor _dbAccessor_;

        public MainViewModel()
        {
            this.InitializeBindings();

            _dbAccessor_ = new DataBaseAccessor();
            if (!_dbAccessor_.IsExistDataBaseFile()) this.GenerateDataBase();

            this.SearchExecute();
        }

        #endregion

        #region イベント処理

        /// <summary>
        /// ボタン〔完了〕押下処理
        /// </summary>
        private void FinishButtonCommandExecute()
        {
            TodoTask clickedTask = null;
            foreach (var task in this.DataGridItemsSource) if (task.IsSelected) clickedTask = task;

            clickedTask.StatusCode = new StatusCode(StatusCode.FINISHED);
            _dbAccessor_.TodoTaskUpdate(clickedTask);
            this.SearchExecute();
        }

        #endregion

        #region 各種メソッド

        /// <summary>
        /// データグリッドに検索結果を表示します
        /// </summary>
        private void SearchExecute()
        {
            SqlBuilder.MainViewSearchConditions sc = new SqlBuilder.MainViewSearchConditions();
            sc.StatusCodeNotEqual = StatusCode.FINISHED;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = _dbAccessor_.TodoTaskSelect(sql);

            this.DataGridItemsSource.Clear();
            foreach (var task in tasks) this.DataGridItemsSource.Add(task);
        }

        /// <summary>
        /// ビューにバインドされている項目を初期化します
        /// </summary>
        private void InitializeBindings()
        {
            this.DataGridItemsSource = new ReactiveCollection<TodoTask>();

            this.FinishButtonCommand = new ReactiveCommand();
            this.FinishButtonCommand.Subscribe(x => FinishButtonCommandExecute());
        }

        /// <summary>
        /// データベースを作成し、サンプルデータを追加します
        /// </summary>
        private void GenerateDataBase()
        {
            _dbAccessor_.CreateDataBase();
            List<TodoTask> tasks = new List<TodoTask>();
            tasks.Add(new TodoTask() { Subject = "get up early", DueDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make breakfast", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "take my dog for a walk", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "make lunch", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.FINISHED) });
            tasks.Add(new TodoTask() { Subject = "pick up my child from a nursery school", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(16).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "cook dinner", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(20).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            tasks.Add(new TodoTask() { Subject = "sleep early", DueDate = Convert.ToDateTime(DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm")), StatusCode = new StatusCode(StatusCode.NOT_YET) });
            foreach (var task in tasks) _dbAccessor_.TodoTaskInsert(task);
        }

        #endregion
    }
}
