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
        private async void RestoreCommandExecute()
        {
            string restorePathAndFileName = this.RestorePathAndFileName.Value;

            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "はい",
                NegativeButtonText = "いいえ",
                AnimateHide = true,
                AnimateShow = true,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            var diagResult = await this.MainWindow.ShowMessageAsync("データ復旧の確認", "現在のデータを全て消去し、バックアップファイル " + Path.GetFileName(restorePathAndFileName) + " で上書きしますがよろしいですか？", MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings);
            if (diagResult != MessageDialogResult.Affirmative) return;

            Dictionary<string, List<List<object>>> restoreSheets = XlsxReader.GetXLSheets(restorePathAndFileName);

            var checkRestoreSheetsResult = this.CheckRestoreSheets(restoreSheets);
            if(checkRestoreSheetsResult.Count > 0)
            {
                await this.MainWindow.ShowMessageAsync("データ復旧処理を中止しました", "バックアップファイルの形式が正しくありません");
                return;
            }

            // under construction

            this.RestorePathAndFileName.Value = string.Empty;
        }

        /// <summary>
        /// 復旧用データのチェックを行います
        /// </summary>
        private List<CheckRestoreSheetsResult> CheckRestoreSheets(Dictionary<string, List<List<object>>> _restoreSheets)
        {
            List<CheckRestoreSheetsResult> ret = new List<CheckRestoreSheetsResult>();

            // 復旧用データに存在するべきシート名
            List<string> properSheetNames = new List<string>()
            {
                "todo_task"
            };

            // 復旧用データ todo_task に存在するべき列名
            List<string> properColumnNamesOfTodoTask = new List<string>()
            {
                "id",
                "created_at",
                "updated_at",
                "subject",
                "due_date",
                "status_code"
            };

            // 復旧用データに存在するシート名をチェック
            {
                List<string> restoreSheetNames = new List<string>();
                foreach (var restoreSheet in _restoreSheets) restoreSheetNames.Add(restoreSheet.Key);

                if (this.IsMatched(properSheetNames, restoreSheetNames) == false)
                {
                    ret.Add(CheckRestoreSheetsResult.SHEET_NAMES_INVALID);
                    return ret;
                }
            }

            // 復旧用データ todo_task のチェック
            List<List<object>> sheet = _restoreSheets["todo_task"];

            // シートにデータが存在すること
            if (sheet.Count == 0)
            {
                ret.Add(CheckRestoreSheetsResult.SHEET_TODO_TASK_ROW_NOT_FOUND);
            }
            else
            {
                List<string> restoreColumnNames = new List<string>();
                foreach (var firstColumn in sheet[0]) restoreColumnNames.Add(firstColumn.ToString());

                // シートの列の過不足をチェック
                if (this.IsMatched(properColumnNamesOfTodoTask, restoreColumnNames) == false)
                {
                    ret.Add(CheckRestoreSheetsResult.SHEET_TODO_TASK_COLUMN_INVALID);
                }
            }

            return ret;
        }

        /// <summary>
        /// 復旧用データのチェック結果
        /// </summary>
        private enum CheckRestoreSheetsResult
        {
            /// <summary>
            /// シート名が不正
            /// </summary>
            SHEET_NAMES_INVALID,

            /// <summary>
            /// todo_task シートに行が存在しない
            /// </summary>
            SHEET_TODO_TASK_ROW_NOT_FOUND,

            /// <summary>
            /// todo_task シートの列に過不足がある
            /// </summary>
            SHEET_TODO_TASK_COLUMN_INVALID
        }

        /// <summary>
        /// 引数リスト同士が一致しているかを判定します
        /// </summary>
        private bool IsMatched(List<string> _firstList, List<string> _secondList)
        {
            bool unMatched = false;
            foreach(var first in _firstList)
            {
                if (_secondList.Contains(first) == false) unMatched = true; 
            }
            foreach(var second in _secondList)
            {
                if (_firstList.Contains(second) == false) unMatched = true; 
            }
            return !(unMatched);
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
