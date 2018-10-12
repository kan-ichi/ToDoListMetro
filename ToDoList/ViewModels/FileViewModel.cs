using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.DataAccess;
using ToDoList.Models.Entities;

namespace ToDoList.ViewModels
{
    class FileViewModel
    {
        #region view binding items
        public ReactiveProperty<string> ExportPathAndFileName { get; private set; }
        public ReactiveProperty<bool> ExportStatusExceptFinished { get; private set; }
        public ReactiveProperty<DateTime?> ExportDueDateFrom { get; private set; }
        public ReactiveProperty<DateTime?> ExportDueDateTo { get; private set; }
        public ReactiveProperty<string> ImportPathAndFileName { get; private set; }
        public ReactiveProperty<string> BackupPathAndFileName { get; private set; }
        public ReactiveProperty<string> RestorePathAndFileName { get; private set; }
        public ReactiveCommand ExportCommand { get; private set; }
        public ReactiveCommand ImportCommand { get; private set; }
        public ReactiveCommand BackupCommand { get; private set; }
        public ReactiveCommand RestoreCommand { get; private set; }
        #endregion

        public MetroWindow MainWindow { get; private set; }
        private DataBaseAccessor _dbAccessor_;

        public FileViewModel()
        {
            this.InitializeBindings();
            this.MainWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            _dbAccessor_ = new DataBaseAccessor();
        }

        /// <summary>
        /// ボタン〔エクスポート〕押下処理
        /// </summary>
        private void ExportCommandExecute()
        {
            SqlBuilder.FileViewExportSearchConditions sc = new SqlBuilder.FileViewExportSearchConditions();
            sc.ExportStatusExceptFinished = this.ExportStatusExceptFinished.Value;
            sc.ExportDueDateFrom = this.ExportDueDateFrom.Value;
            sc.ExportDueDateTo = this.ExportDueDateTo.Value;

            SqlBuilder sql = new SqlBuilder(sc);
            List<TodoTask> tasks = _dbAccessor_.TodoTaskSelect(sql);

            string exportPathAndFileName = this.ExportPathAndFileName.Value;
            XlsxWriter.FileViewExport(exportPathAndFileName, tasks);

            this.MainWindow.ShowMessageAsync("ファイルのエクスポート", Path.GetFileName(exportPathAndFileName) + " にデータを出力しました");
            this.ExportPathAndFileName.Value = string.Empty;
        }

        /// <summary>
        /// ボタン〔インポート〕押下処理
        /// </summary>
        private void ImportCommandExecute()
        {
            // under construction
             string importPathAndFileName = this.ImportPathAndFileName.Value;
        }

        /// <summary>
        /// ボタン〔バックアップ〕押下処理
        /// </summary>
        private void BackupCommandExecute()
        {
            Dictionary<string, List<List<object>>> backupTables = new Dictionary<string, List<List<object>>>();
            foreach(var tableName in _dbAccessor_.GetTableNameList())
            {
                List<List<object>> recordList = _dbAccessor_.SelectAllWithHeader(tableName);
                backupTables.Add(tableName, recordList);
            }

            string backupPathAndFileName = this.BackupPathAndFileName.Value;
            XlsxWriter.FileViewBackup(backupPathAndFileName, backupTables);

            this.MainWindow.ShowMessageAsync("ファイルのバックアップ", Path.GetFileName(backupPathAndFileName) + " にデータを出力しました");
            this.BackupPathAndFileName.Value = string.Empty;
        }

        /// <summary>
        /// ボタン〔データ復旧〕押下処理
        /// </summary>
        private void RestoreCommandExecute()
        {
            // under construction
             string restorePathAndFileName = this.RestorePathAndFileName.Value;
        }

        /// <summary>
        /// ビューにバインドされている項目を初期化します
        /// </summary>
        private void InitializeBindings()
        {
            this.ExportPathAndFileName = new ReactiveProperty<string>();
            this.ExportStatusExceptFinished = new ReactiveProperty<bool>();
            this.ExportDueDateFrom = new ReactiveProperty<DateTime?>();
            this.ExportDueDateTo = new ReactiveProperty<DateTime?>();
            this.ImportPathAndFileName = new ReactiveProperty<string>();
            this.BackupPathAndFileName = new ReactiveProperty<string>();
            this.RestorePathAndFileName = new ReactiveProperty<string>();

            this.ExportCommand = new ReactiveCommand();
            this.ExportCommand = ExportPathAndFileName.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
            this.ExportCommand.Subscribe(x => ExportCommandExecute());

            this.ImportCommand = new ReactiveCommand();
            this.ImportCommand = ImportPathAndFileName.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
            this.ImportCommand.Subscribe(x => ImportCommandExecute());

            this.BackupCommand = new ReactiveCommand();
            this.BackupCommand = BackupPathAndFileName.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
            this.BackupCommand.Subscribe(x => BackupCommandExecute());

            this.RestoreCommand = new ReactiveCommand();
            this.RestoreCommand = RestorePathAndFileName.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
            this.RestoreCommand.Subscribe(x => RestoreCommandExecute());
        }
    }
}
