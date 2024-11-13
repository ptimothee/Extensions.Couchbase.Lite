using System.Collections;
using Couchbase.Lite;
using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite;

public class CouchbaseLiteLiveResults<T> : IEnumerable<T>, IDisposable
{
    private readonly IQuery _query;
    private IDisposable? _subscription;
    private Lazy<IEnumerable<T>> _lazyResult;

    internal CouchbaseLiteLiveResults(IQuery query)
    {
        _query = query;
        var result = GetResults();
        _lazyResult = new Lazy<IEnumerable<T>>(() => result);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _lazyResult.Value.GetEnumerator();
    }

    public void SubscribeToLiveQuery()
    {
        if(_subscription is not null)
        {
            return;
        }

        _subscription = SubscribeForNotifications(ResetResults, FailResults);
    }

    public IDisposable SubscribeForNotifications(Action<object?, IEnumerable<T>> onResultChanged, Action<object?, Exception>? onError = null)
    {
        var taskScheduler = GetTaskScheduler();

        var listenerToken = _query.AddChangeListener(taskScheduler, (sender, args) =>
        {
            if (args.Error is not null)
            {
                onError?.Invoke(sender, args.Error);
                return;
            }

            using(var results = args.Results)
            {
                var updatedResults = results.AllResults().Select(x => x.ToObject<T>()).ToList();
                onResultChanged(sender, updatedResults);
            }
        });

        return new DisposableAction(() => _query.RemoveChangeListener(listenerToken));
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _query.Dispose();
    }

    private void ResetResults(object? sender, IEnumerable<T> results)
    {
        _lazyResult = new Lazy<IEnumerable<T>>(() => results);
    }

    private void FailResults(object? sender, Exception error)
    {
        _lazyResult = new Lazy<IEnumerable<T>>(() => throw error);
    }

    private IEnumerable<T> GetResults()
    {
        using (var result = _query.Execute())
        {
            return result.AllResults()
                            .Select(x => x.ToObject<T>())
                            .ToList();
        }
    }

    private TaskScheduler GetTaskScheduler()
    {
        var syncContext = SynchronizationContext.Current;
        if (syncContext is null)
        {
            return TaskScheduler.Current;
        }

        return TaskScheduler.FromCurrentSynchronizationContext();
    }
}
