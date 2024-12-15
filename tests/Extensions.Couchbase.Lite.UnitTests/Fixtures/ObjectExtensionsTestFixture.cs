using Couchbase.Lite;
using Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures.Models;

namespace Codemancer.Extensions.Couchbase.Lite.UnitTests.Fixtures;

public static class ObjectExtensionsTestFixture
{
    public static IEnumerable<object[]> GetSupportedMutableObjects()
    {
        /** Given an instance **/

        // When instance conatains a null property,
        // Should omit the key-value pair
        yield return new object[]
        {
            new { Object = (string?)null },
            new Dictionary<string, object?>
            {

            }
        };

        // When instance contains a string property,
        // Should include matching key-value pair
        yield return new object[]
        {
            new { Name = "John" },
            new Dictionary<string, object?>
            {
                { "Name", "John" },
            }
        };

        // When instance contains a enum property,
        // Should include matching key-value pair with enum value as string
        yield return new object[]
        {
            new { WeekDay = DayOfWeek.Monday },
            new Dictionary<string, object?>
            {
                { "WeekDay", DayOfWeek.Monday.ToString() }
            }
        };

        // When instance contains a enum property (with EnumMemberAttribute),
        // Should include matching key-value pair with enum value as string
        yield return new object[]
        {
            new { Size = Sizes.Small },
            new Dictionary<string, object?>
            {
                { "Size", "small" }
            }
        };

        // When instance contains a Datetime property,
        // Should include matching key-value pair with datetime as a UTC string
        var now = DateTime.Now;
        yield return new object[]
        {
            new { Now = now },
            new Dictionary<string, object?>
            {
                { "Now", now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK") },
            }
        };


        yield return new object[]
        {
            new { Name = "John", Age = 44 },
            new Dictionary<string, object?>
            {
                { "Name", "John" },
                { "Age", 44 }
            }
        };


        // When instance contains a nested type,
        yield return new object[]
        {
            new { Name = "John", Pet = new { Name = "Oreo", Type = "Dog" } },
            new Dictionary<string, object?>
            {
                { "Name", "John" },
                { "Pet", new Dictionary<string, object?>
                    {
                        { "Name", "Oreo" },
                        { "Type", "Dog" }
                    }
                }
            }
        };



        var bytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        yield return new object[]
        {
            new { Data = bytes },
            new Dictionary<string, object?>
            {
                { "Data", new Blob(string.Empty, bytes) }
            }
        };

        var stream = new MemoryStream([0x01, 0x02, 0x03, 0x04]);
        yield return new object[]
        {
            new { Data = stream },
            new Dictionary<string, object?>
            {
                { "Data", new Blob(string.Empty, stream) }
            }
        };

        var blob = new Blob(string.Empty, new MemoryStream([0x01, 0x02, 0x03, 0x04 ]));
        yield return new object[]
        {
            new { Data = blob },
            new Dictionary<string, object?>
            {
                { "Data", blob }
            }
        };


    }
}
