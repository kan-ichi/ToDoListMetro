﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.BusinessLogic;
using ToDoList.Models.Codes;
using ToDoList.Models.DataAccess;
using ToDoList.Models.Entities;
using ToDoList.Models.Utilities;
using ToDoList.Models.Validators;

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

        #region クラス内変数・コンストラクタ

        public MetroWindow MainWindow { get; private set; }
        private DataBaseAccessor _dbAccessor_;

        public FileViewModel()
        {
            this.InitializeBindings();
            this.MainWindow = System.Windows.Application.Current.MainWindow as MetroWindow;
            _dbAccessor_ = new DataBaseAccessor();
        }

        #endregion

        #region イベント処理

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

            this.MainWindow.ShowMessageAsync("データのエクスポート", Path.GetFileName(exportPathAndFileName) + " にデータを出力しました。");
            this.ExportPathAndFileName.Value = string.Empty;
        }

        /// <summary>
        /// ボタン〔インポート〕押下処理
        /// </summary>
        private async void ImportCommandExecute()
        {
            string importPathAndFileName = this.ImportPathAndFileName.Value;

            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "はい",
                NegativeButtonText = "いいえ",
                AnimateHide = true,
                AnimateShow = true,
                ColorScheme = MetroDialogColorScheme.Theme,
            };

            var diagResult = await this.MainWindow.ShowMessageAsync("データのインポート", "ファイル " + Path.GetFileName(importPathAndFileName) + " のインポート処理を行います。よろしいですか？", MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings);
            if (diagResult != MessageDialogResult.Affirmative) return;

            // インポートデータを取得
            DataSet importSheets = XlsxReader.GetXLSheets(importPathAndFileName);

            // インポートデータを検証
            var checkImportSheetsResult = FileViewImportValidator.CheckImportSheets(importSheets);
            if (checkImportSheetsResult.Count > 0)
            {
                await this.MainWindow.ShowMessageAsync("データのインポートを中止しました", "ファイルの形式が正しくありません。");
                return;
            }

            // タスクテーブルインポート処理
            FileViewImportLogic importLogic = new FileViewImportLogic(_dbAccessor_);
            importLogic.ImportTodoTask(importSheets);

            await this.MainWindow.ShowMessageAsync("データのインポートが完了しました", "ファイル " + Path.GetFileName(importPathAndFileName) + " からデータをインポートしました。");
            this.ImportPathAndFileName.Value = string.Empty;
        }

        /// <summary>
        /// ボタン〔バックアップ〕押下処理
        /// </summary>
        private void BackupCommandExecute()
        {
            DataSet backupTables = new DataSet();
            foreach(var tableName in _dbAccessor_.GetTableNameList())
            {
                DataTable recordList = _dbAccessor_.SelectAll(tableName);
                backupTables.Tables.Add(recordList);
            }

            string backupPathAndFileName = this.BackupPathAndFileName.Value;
            XlsxWriter.FileViewBackup(backupPathAndFileName, backupTables);

            this.MainWindow.ShowMessageAsync("データのバックアップ", Path.GetFileName(backupPathAndFileName) + " にデータを出力しました。");
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

            // 復旧用データを取得
            DataSet restoreSheets = XlsxReader.GetXLSheets(restorePathAndFileName);

            // 復旧用データを検証
            var checkRestoreSheetsResult = FileViewRestoreValidator.CheckRestoreSheets(restoreSheets);
            if(checkRestoreSheetsResult.Count > 0)
            {
                await this.MainWindow.ShowMessageAsync("データ復旧処理を中止しました", "バックアップファイルの形式が正しくありません。");
                return;
            }

            // 各テーブルのリストア処理
            FileViewRestoreLogic restoreLogic = new FileViewRestoreLogic(_dbAccessor_);
            restoreLogic.RestoreTables(restoreSheets);

            await this.MainWindow.ShowMessageAsync("データ復旧処理が完了しました", "バックアップファイル " + Path.GetFileName(restorePathAndFileName) + " からデータを復旧しました。");
            this.RestorePathAndFileName.Value = string.Empty;
        }

        #endregion

        #region 各種メソッド

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

        #endregion
    }
}
