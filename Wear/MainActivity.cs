using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.Wearable.Views;
using Android.Views;
using Android.Widget;

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Gms.Wearable;

namespace Wear
{
	[Activity(Label = "Wear", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity,
		Android.Gms.Common.Apis.IResultCallback,
		Android.Gms.Common.Apis.IGoogleApiClientConnectionCallbacks, 
		Android.Gms.Common.Apis.IGoogleApiClientOnConnectionFailedListener
	{
		Android.Gms.Common.Apis.IGoogleApiClient client;
		private const string MessageTag = "hoge";


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


		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			client = new GoogleApiClientBuilder(this)
				.AddApi(Android.Gms.Wearable.WearableClass.Api)
				.AddConnectionCallbacks(this) // Xamarinの場合はクラスがないので、このクラスのOnConnect系を使う
				.AddOnConnectionFailedListener(this) // 同上
				.Build();
			client.Connect();

			var v = FindViewById<WatchViewStub>(Resource.Id.watch_view_stub);
			v.LayoutInflated += delegate
			{
				// AndroidのAsyncTaskを使う方法
				var asyncTask = FindViewById<Button>(Resource.Id.AsyncTask);
				asyncTask.Click += (sender, e) =>
				{
					var task = new SendMessageAsyncTask(){ Activity = this };
					task.Execute();
				};

				// C#のasync/awaitを使う方法
				var asyncAwait = FindViewById<Button>(Resource.Id.AsyncAwait);
				asyncAwait.Click += (sender, e) => SendMessageAsync();
			};
		}


		public ICollection<string> NodeIds
		{
			get
			{
				var results = new HashSet<string>();
				var nodes =
					Android.Gms.Wearable.WearableClass.NodeApi.GetConnectedNodes(client)
						.Await()
						.JavaCast<Android.Gms.Wearable.INodeApiGetConnectedNodesResult>();

				foreach (var node in nodes.Nodes)
				{
					results.Add(node.Id);
				}
				return results;
			}
		}


		public void SendMessage(String node)
		{
			WearableClass.MessageApi.SendMessage(client, node, MessageTag, System.Text.Encoding.UTF8.GetBytes("async_task"));
		}


		public Task<ICollection<string>> GetNodeIdsAsync()
		{
			return Task.Run(() => NodeIds);
		}

		public async void SendMessageAsync()
		{
			var nodeIds = await GetNodeIdsAsync();

			foreach (var nodeId in nodeIds)
			{
				WearableClass.MessageApi.SendMessage(client, nodeId, MessageTag, Encoding.UTF8.GetBytes("async_await"));
			}
		} 
	}
}



