using System;

namespace Wear
{
    public class SendMessageAsyncTask : Android.OS.AsyncTask
    {
        public MainActivity Activity { get; set; }

        protected override Java.Lang.Object DoInBackground (params Java.Lang.Object[] parameters)
        {
            if (Activity != null) {
                var nodes = Activity.NodeIds;
                foreach (var node in nodes) {
                    Activity.SendMessage (node);
                }
            }
            return null;
        }
    }
}

