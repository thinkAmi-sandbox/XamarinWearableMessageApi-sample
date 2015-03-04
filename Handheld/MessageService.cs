using System;

using Android.Widget; // toastまわり
using Android.Gms.Wearable; 

namespace Handheld
{
    [Android.App.Service()]
    [Android.App.IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" })] 
    public class MessageService : 
        Android.Gms.Wearable.WearableListenerService
    {
        private const string MessageTag = "hoge";

        // WearableListenerServiceのをオーバーライド
        public override void OnMessageReceived(IMessageEvent e)
        {
            base.OnMessageReceived(e);

            // メッセージタグが一致している場合のみ反応する
            // See: https://github.com/xamarin/monodroid-samples/blob/master/wear%2FQuiz%2FWearable%2FQuizListenerService.cs#L134
            if (MessageTag.Equals(e.Path))
            {
                var msg = System.Text.Encoding.UTF8.GetString(e.GetData());

                Android.Widget.Toast.MakeText(this, msg, ToastLength.Long).Show();
            }
        } 
    }
}

