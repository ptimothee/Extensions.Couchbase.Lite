namespace Codemancer.Extensions.Couchbase.Lite;

internal class DisposableAction : IDisposable
{
    private readonly Action _action;

    internal DisposableAction(Action onDispose)
    {
        _action = onDispose;
    }

    public void Dispose()
    {
        _action();
    }
}
