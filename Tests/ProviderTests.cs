using Enamdict;

namespace Tests;

public class ProviderTests
{
    [Test]
    public async Task ParserTest()
    {
        var (info, result) = await Provider.ParseAsync();
        Assert.That(info.Date, Is.GreaterThan(DateOnly.Parse("2023-01-01")));
        Assert.That(result.Count, Is.GreaterThan(300_000));
    }
}