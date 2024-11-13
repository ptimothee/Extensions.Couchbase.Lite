using NSubstitute;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Customizations;

/// <summary>
/// Customize an IQuery such that it returns amd Empty resultSet when Execute is called
/// and receives QueryChangedEventArgs notification containing an error.
/// </summary>
public class LiveQueryDispatchingFailedQueryResultChangedCustomization : ICustomization
{
    public LiveQueryDispatchingFailedQueryResultChangedCustomization(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize(new AutoNSubstituteCustomization());
        fixture.Customize(new FailedQueryChangedEventArgsCustomization(Exception)); // When the query change notification occures the notification event should contain specified exception 

        var query = fixture.Freeze<IQuery>();
        query.Execute()
                .Returns(x =>
                {
                    var resultSet = Substitute.For<IResultSet>();
                    resultSet.AllResults().Returns(new List<Result>());
                    return resultSet;
                });

        query.When(query => query.AddChangeListener(Arg.Any<TaskScheduler>(), Arg.Any<EventHandler<QueryChangedEventArgs>>()))
                .Do(methodInvocation =>
                {
                    var args = fixture.Create<QueryChangedEventArgs>();
                    var eventHandler = methodInvocation.ArgAt<EventHandler<QueryChangedEventArgs>>(1);
                    eventHandler.Invoke(this, args);
                });
    }
}