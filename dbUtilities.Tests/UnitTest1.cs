namespace dbUtilities.Tests;

using Microsoft.Data.Sqlite;

public class UrlShorteningTests
{
    [Fact]
    public void ShortenUrl_ReturnsSixCharacterCode()
    {
        var databasePath = GetTempDbPath();

        SqliteDb.CreateUrlsTable(databasePath);

        var shortCode = SqliteDb.ShortenUrl(databasePath, "https://example.com/very/long/url");

        Assert.Equal(6, shortCode.Length);
        Assert.Matches("^[A-Za-z0-9]{6}$", shortCode);

        Cleanup(databasePath);
    }

    [Fact]
    public void ShortenUrl_StoresAndReturnsOriginalUrl()
    {
        var databasePath = GetTempDbPath();
        const string originalUrl = "https://learn.microsoft.com/dotnet/csharp/";

        SqliteDb.CreateUrlsTable(databasePath);
        var shortCode = SqliteDb.ShortenUrl(databasePath, originalUrl);

        var savedOriginalUrl = SqliteDb.GetOriginalUrl(databasePath, shortCode);

        Assert.Equal(originalUrl, savedOriginalUrl);

        Cleanup(databasePath);
    }

    [Fact]
    public void GetOriginalUrl_ReturnsNull_WhenShortCodeDoesNotExist()
    {
        var databasePath = GetTempDbPath();

        SqliteDb.CreateUrlsTable(databasePath);

        var result = SqliteDb.GetOriginalUrl(databasePath, "ABC123");

        Assert.Null(result);

        Cleanup(databasePath);
    }

    private static string GetTempDbPath()
    {
        return Path.Combine(Path.GetTempPath(), $"shortly-tests-{Guid.NewGuid():N}.db");
    }

    private static void Cleanup(string databasePath)
    {
        if (!File.Exists(databasePath))
        {
            return;
        }

        SqliteConnection.ClearAllPools();

        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                File.Delete(databasePath);
                return;
            }
            catch (IOException) when (attempt < 4)
            {
                Thread.Sleep(50);
            }
        }
    }
}
