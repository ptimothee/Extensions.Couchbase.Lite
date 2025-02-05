using Couchbase.Lite.Sync;

namespace Codemancer.Extensions.Couchbase.Lite.Sync
{
    public static class ReplicatorExtensions
    {
        public static async Task StartAsync(this Replicator replicator)
        {
            if (replicator.Status.Activity != ReplicatorActivityLevel.Stopped)
            {
                return;
            }

            var tcs = new TaskCompletionSource();
            var eventSubcription = replicator.AddChangeListener((sender, args) =>
            {
                if (args.Status.Error is not null)
                {
                    tcs.SetException(args.Status.Error);
                    return;
                }

                if (args.Status.Activity == ReplicatorActivityLevel.Connecting)
                {
                    tcs.SetResult();
                }
            });

            replicator.Start();    
            await tcs.Task.ConfigureAwait(false);

            eventSubcription.Remove();
        }

        public static async Task StopAsync(this Replicator replicator)
        {
            if (replicator.Status.Activity == ReplicatorActivityLevel.Stopped)
            {
                return;
            }

            var tcs = new TaskCompletionSource();
            var eventSubcription = replicator.AddChangeListener((sender, args) =>
            {
                if(args.Status.Error is not null)
                {
                    tcs.SetException(args.Status.Error);
                    return;
                }

                if (args.Status.Activity == ReplicatorActivityLevel.Stopped)
                {
                    tcs.SetResult();
                    return;
                }

                tcs.SetResult();             
            });

            replicator.Stop();
            await tcs.Task;

            eventSubcription.Remove();
        }
    }
}
