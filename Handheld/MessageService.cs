using System;

using System.Collections.Generic;
using System.Text;
using Android.Runtime;  // `JavaCast`を使うために必要
using Android.Widget; // toastまわり
using Android.Gms.Common.Apis;
using Android.Gms.Wearable; 

namespace Handheld
{
    [Android.App.Service()]
    [Android.App.IntentFilter(new string[] { "com.google.android.gms.wearable.BIND_LISTENER" })] 
    public class MessageService : 
        Android.Gms.Wearable.WearableListenerService,
        IResultCallback,
        IGoogleApiClientConnectionCallbacks,
        IGoogleApiClientOnConnectionFailedListener
    {
        IGoogleApiClient client;
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


                // UIスレッドではないため、以下の書き方でOK
                var nodeIds = NodeIds;
                foreach (var nodeId in nodeIds)
                {
                    WearableClass.MessageApi
                        .SendMessage(client, nodeId, MessageTag, Encoding.UTF8.GetBytes("こんにちは"));
                } 
            }
        }

        // IGoogleApiClientConnectionCallbacksインタフェース向け
        public void OnConnected(Android.OS.Bundle connectionHint)
        {
        }

        public void OnConnectionSuspended(int cause)
        {
        }

        // IGoogleApiClientOnConnectionFailedListenerインタフェース向け
        public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result)
        {
        }

        // IResultCallbackインタフェース向け
        public void OnResult(Java.Lang.Object result)
        {
        } 

        public override void OnCreate()
        {
            base.OnCreate();

            client = new Android.Gms.Common.Apis.GoogleApiClientBuilder(this)
                .AddApi(Android.Gms.Wearable.WearableClass.Api)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            client.Connect();
        }

        public ICollection<string> NodeIds
        {
            get
            {
                var results = new HashSet<string>();
                var nodes =
                    WearableClass.NodeApi.GetConnectedNodes(client)
                        .Await().JavaCast<INodeApiGetConnectedNodesResult>();

                foreach (var node in nodes.Nodes)
                {
                    results.Add(node.Id);
                }
                return results;
            }
        } 
    }
}

