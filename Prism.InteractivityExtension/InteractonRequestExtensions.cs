using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.InteractivityExtension
{
    public static class InteractonRequestExtensions
    {
        /// <summary>
        /// 確認を求めるダイアログを出すリクエストを送信する
        /// </summary>
        /// <param name="interactionRequest"></param>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="content">ダイアログの内容</param>
        public static void RaiseEx(this InteractionRequest<Notification> interactionRequest, string title, string content)
        {
            interactionRequest.Raise(new Notification
            {
                Title = title,
                Content = content
            });
        }
        /// <summary>
        /// 確認を求めるダイアログを出すリクエストを送信する
        /// </summary>
        /// <param name="interactionRequest"></param>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="content">ダイアログの内容</param>
        /// <param name="callback">確認した時のコールバックメソッド</param>
        public static void RaiseEx(this InteractionRequest<Notification> interactionRequest, string title, string content, Action<Notification> callback)
        {
            interactionRequest.Raise(new Notification
            {
                Title = title,
                Content = content
            },
            callback);
        }

        /// <summary>
        /// Yes,Noを求めるダイアログを出すリクエストを送信し、結果を返す
        /// </summary>
        /// <param name="interactionRequest"></param>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="content">ダイアログの内容</param>
        /// <returns></returns>
        public static bool RaiseEx(this InteractionRequest<Confirmation> interactionRequest, string title, string content)
        {
            bool res = false;
            interactionRequest.Raise(new Confirmation
            {
                Title = title,
                Content = content
            },
            n =>
            {
                res = n.Confirmed;
            });
            return res;
        }
        /// <summary>
        /// Yes,Noを求めるダイアログを出すリクエストを送信する
        /// </summary>
        /// <param name="interactionRequest"></param>
        /// <param name="title">ダイアログのタイトル</param>
        /// <param name="content">ダイアログの内容</param>
        /// <param name="callback">Yes,Noが選択された時のコールバックメソッド</param>
        public static void RaiseEx(this InteractionRequest<Confirmation> interactionRequest, string title, string content, Action<Confirmation> callback)
        {
            interactionRequest.Raise(new Confirmation
            {
                Title = title,
                Content = content
            },
            callback);
        }
    }
}
