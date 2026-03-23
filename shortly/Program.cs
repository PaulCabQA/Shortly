using dbUtilities;

var databasePath = Path.Combine(AppContext.BaseDirectory, "shortly.db");
var originalUrl = args.Length > 0 ? args[0] : "https://example.com/some/long/path";

SqliteDb.CreateUrlsTable(databasePath);


//prompt the user to enter a url to shorten
Console.WriteLine("Enter a URL to shorten:");
var inputUrl = Console.ReadLine();
var shortCode = SqliteDb.ShortenUrl(databasePath, inputUrl ?? originalUrl);




Console.WriteLine($"Original URL: {originalUrl}");
Console.WriteLine($"Short code: {shortCode}");
Console.WriteLine($"Database: {databasePath}");


// prompt the user to enter a short code to retrieve the original url
Console.WriteLine("Enter a short code to retrieve the original URL:");  
var inputShortCode = Console.ReadLine();
var originalUrlRetrieved = SqliteDb.GetOriginalUrl(databasePath, inputShortCode ?? shortCode);

if (originalUrlRetrieved != null)
{
    Console.WriteLine($"Original URL: {originalUrlRetrieved}");
}
else
{
    Console.WriteLine("Short code not found.");
}

