using Prism.Interactivity.InteractionRequest;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ToDoList.Models.Codes;
using ToDoList.Models.Entities;

namespace ToDoList.ViewModels
{
    class PopupEditViewModel
    {
        #region view binding items

        public Window AssociatedWindow { get; set; }

        private INotification _notification_;
        public INotification Notification
        {
            get { return this._notification_; }
            set
            {
                bool isNoValueExist = (_notification_ == null);
                this._notification_ = value;
                if (isNoValueExist)
                {
                    InputValueNotification inputValueNotification = (InputValueNotification)Notification;
                    this._editingTodoTask_ = (TodoTask)inputValueNotification.RequestValue;
                    this.DisplayEditControls(_editingTodoTask_);
                }
            }
        }

        public ReactiveProperty<DateTime?> DueDate { get; private set; }
        public ReadOnlyCollection<string> DueDateHourItemsSource { get; private set; }
        public ReadOnlyCollection<string> DueDateMinuteItemsSource { get; private set; }
        public ReactiveProperty<string> DueDateHour { get; private set; }
        public ReactiveProperty<string> DueDateMinute { get; private set; }
        public ReactiveProperty<bool> Status { get; private set; }
        public ReactiveProperty<string> Subject { get; private set; }
        public ReactiveCommand UpdateCommand { get; private set; }
        public ReactiveCommand CancelCommand { get; private set; }
        public ReactiveCommand AddNewCommand { get; private set; }

        #endregion

        #region クラス内変数・コンストラクタ

        private TodoTask _editingTodoTask_;

        public PopupEditViewModel()
        {
            this.InitializeBindings();
        }

        #endregion

        #region 各種メソッド

        /// <summary>
        /// 編集する項目を画面に表示します
        /// </summary>
        private void DisplayEditControls(TodoTask _task)
        {
            this.DueDate.Value = null;
            this.DueDateHour.Value = 0.ToString("00");
            this.DueDateMinute.Value = 0.ToString("00");
            this.Status.Value = false;
            this.Subject.Value = string.Empty;

            if (_task != null)
            {
                if (_task.DueDate.HasValue)
                {
                    this.DueDate.Value = _task.DueDate.Value;
                    this.DueDateHour.Value = _task.DueDate.Value.Hour.ToString("00");
                    this.DueDateMinute.Value = _task.DueDate.Value.Minute.ToString("00");
                }
                this.Status.Value = _task.StatusCode.IsFinished;
                this.Subject.Value = _task.Subject;
            }
        }

        /// <summary>
        /// 画面項目を引数で指定されたオブジェクトにセットします
        /// </summary>
        private void CollectEditControls(ref TodoTask _task)
        {
            if (this.DueDate.Value.HasValue)
            {
                _task.DueDate = this.DueDate.Value.Value.Date.AddHours(Convert.ToInt32(this.DueDateHour.Value)).AddMinutes(Convert.ToInt32(this.DueDateMinute.Value));
            }
            else
            {
                _task.DueDate = null;

            }

            if (this.Status.Value)
            {
                _task.StatusCode = new StatusCode(StatusCode.CODE_FINISHED);
            }
            else
            {
                _task.StatusCode = new StatusCode(StatusCode.CODE_NOT_YET);
            }

            _task.Subject = this.Subject.Value;
        }

        /// <summary>
        /// ビューにバインドされている項目を初期化します
        /// </summary>
        private void InitializeBindings()
        {
            this.DueDate = new ReactiveProperty<DateTime?>();
            // 良い初期化方法があるはず
            this.DueDateHourItemsSource = new ReadOnlyCollection<string>(new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" });
            this.DueDateMinuteItemsSource = new ReadOnlyCollection<string>(new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" });
            this.DueDateHour = new ReactiveProperty<string>();
            this.DueDateMinute = new ReactiveProperty<string>();
            this.Status = new ReactiveProperty<bool>();
            this.Subject = new ReactiveProperty<string>();

            this.UpdateCommand = new ReactiveCommand();
            this.CancelCommand = new ReactiveCommand();
            this.AddNewCommand = new ReactiveCommand();

            this.UpdateCommand.Subscribe(() =>
            {
                this.CollectEditControls(ref _editingTodoTask_);
                InputValueNotification inputValueNotification = (InputValueNotification)Notification;
                inputValueNotification.ReturnValue = this._editingTodoTask_;

                // IInteractionRequestAware を継承しても FinishInteraction が null のままで使用することができないため、この方法を用いる
                // FinishInteraction();
                this.AssociatedWindow.Close();
                //
            });

            this.CancelCommand.Subscribe(() =>
            {
                this.AssociatedWindow.Close();
            });

            this.AddNewCommand.Subscribe(() =>
            {
                this.CollectEditControls(ref _editingTodoTask_);
                this._editingTodoTask_.ID = null;
                InputValueNotification inputValueNotification = (InputValueNotification)Notification;
                inputValueNotification.ReturnValue = this._editingTodoTask_;
                this.AssociatedWindow.Close();
            });
        }

        #endregion
    }
}
