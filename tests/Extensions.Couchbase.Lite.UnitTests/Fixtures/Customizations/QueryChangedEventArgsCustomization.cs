using AutoFixture;
using NSubstitute;
using System.Reflection;
using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Customizations;

/// <summary>
/// Builds a QueryChangedEventArgs such that it contains an empty resultSet with an exception.
/// </summary>
public class FailedQueryChangedEventArgsCustomization : ICustomization
{
    public FailedQueryChangedEventArgsCustomization(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<QueryChangedEventArgs>(builder => {
            var resultSet = Substitute.For<IResultSet>();
            resultSet.AllResults().Returns(new List<Result>());
            return builder.FromFactory(() => Create(resultSet, Exception));
        });
    }

    private QueryChangedEventArgs Create(IResultSet resultSet, Exception exception)
    {
        var ctor = typeof(QueryChangedEventArgs).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
        return (QueryChangedEventArgs)ctor.Invoke(new object[] { resultSet, exception });
    }
}