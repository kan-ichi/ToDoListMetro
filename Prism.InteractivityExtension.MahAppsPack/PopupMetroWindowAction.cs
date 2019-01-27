using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Interactivity.InteractionRequest;
using Prism.InteractivityExtension.MahAppsPack.DefaultPopupMetroWindows;

namespace Prism.InteractivityExtension.MahAppsPack
{
    public class PopupMetroWindowAction : PopupWindowActionBase
    {
        #region MetroStyle
        public ResourceDictionary MetroStyle
        {
            get { return (ResourceDictionary)GetValue(MetroStyleProperty); }
            set { SetValue(MetroStyleProperty, value); }
        }
        public static readonly DependencyProperty MetroStyleProperty =
            DependencyProperty.Register("MetroStyle", typeof(ResourceDictionary), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion
        #region Accent
        public Accents? Accent
        {
            get { return (Accents?)GetValue(AccentProperty); }
            set { SetValue(AccentProperty, value); }
        }
        public static readonly DependencyProperty AccentProperty =
            DependencyProperty.Register("Accent", typeof(Accents?), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion
        #region Theme
        public Themes? Theme
        {
            get { return (Themes?)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Themes?), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion
        /// <summary>
        /// MetroWindowを生成して返す
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        protected override Window CreateWindow(INotification notification)
        {
            Window window = null;

            if (notification == null)
            {
                window = new DefaultMetroWindow() { Title = notification.Title,Content = notification.Content };
            }
            else if (notification is IConfirmation)
            {
                window = new DefaultConfirmationMetroWindow() { Confirmation = (IConfirmation)notification };
            }
            else
            {
                window = new DefaultNotificationMetroWindow() { Notification = notification };
            }
            return window;
        }
        /// <summary>
        /// MetroWindowのオーナーをセットする
        /// <para>MetroStyleが設定されていればWindowに適用する</para>
        /// <para>MetroStyleが設定されておらず、AccentとThemeが設定されていればこれらを適用する</para>
        /// <para>MetroStyleが設定されておらず、AccentとThemeの両方が設定されておらず、OwnerがMetroWindoであれば、Ownerと同様のStyleを適用する</para>
        /// <para>複数有効ならMetroStyleが優先される</para>複数有効ならMetroStyleが優先される
        /// </summary>
        /// <param name="window"></param>
        protected override void SetWindowOwner(Window window)
        {
            base.SetWindowOwner(window);
            if(this.MetroStyle != null)
            {
                window.Resources.MergedDictionaries.Add(this.MetroStyle);
            }
            else
            {
                if (this.Accent.HasValue && this.Theme.HasValue)
                {
                    MahApps.Metro.ThemeManager.ChangeAppStyle(
                        window,
                        MahApps.Metro.ThemeManager.GetAccent(this.Accent.Value.ToStringFromEnum()),
                        MahApps.Metro.ThemeManager.GetAppTheme(this.Theme.Value.ToStringFromEnum()));
                }
                else if (window.Owner is MahApps.Metro.Controls.MetroWindow) window.Resources.MergedDictionaries.AddRange(window.Owner.Resources.MergedDictionaries);
            }
        }

        protected override void ApplyNotificationToWindow(Window window, INotification notification)
        {
        }
    }
}

