using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using General.DataModels;
using WuxiaClassLib.DataModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using Scraper;
using System.Text;

namespace WuxiaApp.Servs;
public class Services
{
    List<Book> bookList;
    readonly string api;
    readonly string hostSite;
    static HttpClient client;
    WuxiaScraper scraper;
    string _userFont;
    double _userFontSize;
    Color _userColor;

    readonly Dictionary<string, string> ImageParams = new()
    {
        ["preview"] = ".webp?width=150&quality=60",
        ["bigpic"] = ".webp",
        ["source"] = "https://wuxiaworldeu.b-cdn.net/original/"
    };
    public bool UserProfileSet { get; set; }
    public string UserFont { get => _userFont;  }
    public double UserFontSize { get => _userFontSize;  }
    public Color UserColor { get => _userColor;  }

    public Services()
    {
        api = "https://wuxia.click/api/";
        hostSite = "https://wuxia.click/";
        client = new HttpClient() { BaseAddress = new Uri(api) };
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        scraper = new WuxiaScraper();
        bookList = new List<Book>();
        UserProfileSet = false;
    }

    public async Task<List<Book>> GetBooksLocalAsync(IFileSystem fileSystem)
    {

        if (bookList?.Count > 0)
            return bookList;
        try
        {

            var contents = await File.ReadAllTextAsync(Path.Combine(fileSystem.AppDataDirectory, "library.dat"));
            bookList = JsonSerializer.Deserialize<List<Book>>(contents);
            var path = Path.Combine(fileSystem.AppDataDirectory, "user_profile");
            if (File.Exists(path))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    var reader = new BinaryReader(stream);
                    _userFont = reader.ReadString();
                    _userFontSize = reader.ReadDouble();
                    _userColor = Color.FromUint(reader.ReadUInt32());
                }
                UserProfileSet = true;
            }
            
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return bookList;
    }

    public async Task<searchResult> SearchBookAsync(string searchPattern = "", string ordering = "-total_views", string category = "", string limit = "10")
    {
        var query = new Dictionary<string, string>()
        {
            ["search"] = searchPattern,
            ["limit"] = limit,
            ["order"] = ordering,
            ["category_name"] = category
        };
        var querysrting = QueryHelpers.AddQueryString("search/", query);
        using var request = new HttpRequestMessage(HttpMethod.Get, querysrting);
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var jsonObject = await response.Content.ReadFromJsonAsync<searchResult>();


        return jsonObject;
    }

    public async Task<searchResult> AdvancedFilteringAsync(string categories = "", string ordering = "-total_views")
    {

        var query = new Dictionary<string, string>()
        {
            ["itemsPerPage"] = "10",
            ["category_include"] = categories,
            ["order"] = ordering
        };
        var querysrting = QueryHelpers.AddQueryString("novel-filter/", query);
        using var request = new HttpRequestMessage(HttpMethod.Get, querysrting);
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<searchResult>();
    }

    public async Task CopyLibraryFileAsync(string sourceFile, string targetFileName)
    {
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(sourceFile);
        using StreamReader reader = new(fileStream);

        string content = await reader.ReadToEndAsync();

        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);

        using FileStream outputStream = File.OpenWrite(targetFile);
        using StreamWriter streamWriter = new(outputStream);

        await streamWriter.WriteAsync(content);
    }

    public void DeleteBook(Book book)
    {
        if (!bookList.Contains(book))
            throw new ArgumentException($"There is no book {book.Title} by {book.Author.name} in the library");
        bookList?.Remove(book);

    }

    public void Save(IFileSystem fileSystem)
    {
        var content = JsonSerializer.Serialize(bookList);
        try
        {
            
            using (var stream = File.Open(
            Path.Combine(fileSystem.AppDataDirectory, "library.dat"),
            FileMode.Create))
            {
                using var writer = new StreamWriter(stream);
                writer.WriteLine(content);
            }
            using (var stream = File.Open(
            Path.Combine(fileSystem.AppDataDirectory, "user_profile"),
            FileMode.Create))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(_userFont);
                writer.Write(_userFontSize);
                writer.Write(_userColor.ToUint());

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    /// <summary>
    /// Adds a new book to the library
    /// </summary>
    /// <param name="book"> Book object to be added</param>
    /// <exception cref="ArgumentException"></exception>
    public void AddNewBook(Book book)
    {
        if (bookList.Contains(book))
            throw new ArgumentException($"Error book {book.Title} by {book.Author.name} is already present is the library");
        bookList.Add(book);
    }
    public async Task<BookInfo> GetBookInfoAsync(string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "novels/" + name);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BookInfo>();
        return result;

    }
    /// <summary>
    ///  Forms path for picture
    ///  
    /// </summary>
    /// <param name="slugName">slug name of the book</param>
    /// <param name="picParam">picture quality parametr(preview or bigpic)</param>
    /// <returns> A string representing uri path for picture</returns>
    /// <exception cref="ArgumentException"></exception>
    public string FormPicturePath(string slugName, string picParam = "preview")
    {
        ArgumentNullException.ThrowIfNull(slugName);
        return ImageParams["source"] + slugName + ImageParams[picParam];
    }
    public bool CheckBookInLib(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        return bookList.Contains(book);
    }
    public async Task<searchResult> LoadNextDataAsync(Uri path)
    {

        using var response = await client.GetAsync("search/" + path.Query);

        response.EnsureSuccessStatusCode();

        var jsonObject = await response.Content.ReadFromJsonAsync<searchResult>();


        return jsonObject;
    }
    public async Task<ChapterData> FetchChapterAsync(string chapSlug)
    {
        //scraper.Load(hostSite + "chapter/" + chapSlug);
        //return scraper.GetReadingPage();
        scraper.SiteUri = new Uri(hostSite + "chapter/" + chapSlug);
        return await scraper.GetScriptContentDOMAsync<ChapterData>();
    }

    public void SaveReadedPoint(Book readedBook)
    {
        var result = bookList.Find(book => book.Slug == readedBook.Slug);
        result.Readed = readedBook.Readed;

    }
    public void SetUserPreferences(string font, double fontsize, Color color)
    {
        _userFont = font;
        _userFontSize = fontsize;
        _userColor = color;
        UserProfileSet = true;
    }
}

