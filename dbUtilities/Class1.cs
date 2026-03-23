using Microsoft.Data.Sqlite;

namespace dbUtilities;

public static class SqliteDb
{
	public static SqliteConnection CreateConnection(string databasePath)
	{
		var connectionString = new SqliteConnectionStringBuilder
		{
			DataSource = databasePath,
			Mode = SqliteOpenMode.ReadWriteCreate,
		}.ToString();

		return new SqliteConnection(connectionString);
	}

	public static void EnsureCreated(string databasePath, string? createSchemaSql = null)
	{
		using var connection = CreateConnection(databasePath);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = "PRAGMA foreign_keys = ON;";
		command.ExecuteNonQuery();

		if (!string.IsNullOrWhiteSpace(createSchemaSql))
		{
			command.CommandText = createSchemaSql;
			command.ExecuteNonQuery();
		}
	}

    // Add a method to create a table in the database named urls with an id auto incremented and two text columns named originlal_url and shortened_url
    public static void CreateUrlsTable(string databasePath)
    {
        using var connection = CreateConnection(databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS urls (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                original_url TEXT NOT NULL,
                shortened_url TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
    } 

// create a function that accepts a url and shortens it by generating a random string of 6 characters and inserting the original url and the shortened url into the urls table
    public static string ShortenUrl(string databasePath, string originalUrl)
    {
        var shortenedUrl = GenerateRandomString(6);

        using var connection = CreateConnection(databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO urls (original_url, shortened_url)
            VALUES (@originalUrl, @shortenedUrl);
        ";
        command.Parameters.AddWithValue("@originalUrl", originalUrl);
        command.Parameters.AddWithValue("@shortenedUrl", shortenedUrl);
        command.ExecuteNonQuery();

        return shortenedUrl;
    }


    //create a function that returns the original url given a shortened url
    public static string? GetOriginalUrl(string databasePath, string shortenedUrl)
    {
        using var connection = CreateConnection(databasePath);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT original_url
            FROM urls
            WHERE shortened_url = @shortenedUrl;
        ";
        command.Parameters.AddWithValue("@shortenedUrl", shortenedUrl);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetString(0);
        }
        else
        {
            return null;
        }
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }   
 
}
