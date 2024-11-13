using NSubstitute;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Couchbase.Lite.Query;

namespace Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Customizations;

/// <summary>
/// Customize an IQuery such that it throws an exception when Execute is called.
/// </summary>
public class FailedQueryCustomization : ICustomization
{
    public FailedQueryCustomization(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    public void Customize(IFixture fixture)
    {
        fixture.Customize(new AutoNSubstituteCustomization());

        var query = fixture.Freeze<IQuery>();
        query.When(query => query.Execute())
                .Do(methodInvocation =>
                {
                    throw Exception;
                });
    }
}
