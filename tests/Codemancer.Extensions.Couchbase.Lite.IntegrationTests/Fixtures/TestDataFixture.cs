using Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures.Models;

namespace Codemancer.Extensions.Couchbase.Lite.IntegrationTests.Fixtures;

public static class TestDataFixture
{
    public static IEnumerable<object[]> GetPeople()
    {
        yield return new object[] { new Person { Name = "John", Age = 44, Dob = new DateTime(1980, 1, 15), DigitalProfiles = [new DigitalProfile { SiteName = "Facebook", WebAddress = "https://fb.com/profile/john" }, new DigitalProfile { SiteName = "Youtube", WebAddress = "https://yt.com/profile/john"}] } };
        yield return new object[] { new Person { Name = "Jane", Age = 40, Pet = new Pet { Name = "Waffle", Type = "Cat", Photo = [0x01, 0x02, 0x03, 0x04] } } };
        yield return new object[] { new Person { Name = "Jerry", Age = 40, Photo = [0x01, 0x02, 0x03, 0x04] } };
    }
}
