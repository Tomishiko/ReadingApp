using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using General.DataModels;
using WuxiaClassLib.DataModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using Scraper;
using System.Text;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace WuxiaApp.Servs;
public class Services
{
    Dictionary<string,Book> books;
    readonly string api;
    readonly string hostSite;
    static HttpClient client;
    WuxiaScraper scraper;
    int _userFont;
    double _userFontSize;
    int _userBackColor;

    readonly Dictionary<string, string> ImageParams = new()
    {
        ["preview"] = ".webp?width=150&quality=60",
        ["bigpic"] = ".webp",
        ["source"] = "https://wuxiaworldeu.b-cdn.net/original/"
    };
    public bool UserProfileSet { get; set; }
    public string Font { get => Fonts[_userFont]; }
    public int FontIndex
    {
        get => _userFont;
        set
        {
            if (value>=Fonts.Count || value<0) return;
            else _userFont = value;
        }
    }
    public int BackColorIndex
    {
        get => _userBackColor;
        set
        {
            if (value >= Backgrounds.Count || value < 0) return;
            else _userBackColor = value;
        }
    }
    public Color BackColor { get => Backgrounds[_userBackColor]; }
    public double FontSize { get => _userFontSize; }
    public List<string> Fonts;
    public List<Color> Backgrounds;

    public Services()
    {
        api = "https://wuxia.click/api/";
        hostSite = "https://wuxia.click/";
        client = new HttpClient() { BaseAddress = new Uri(api) };
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        scraper = new WuxiaScraper();
        books = new Dictionary<string, Book>();
        UserProfileSet = false;
        Fonts = new List<string>
        {
            "OpenSansRegular",
            "SegoeRegular",
            "SegoePrint",
            "Arial",
            "Calibri",
            "Roboto",
            "Tahoma",
            "TimesNewRoman"
        };
        Backgrounds = new List<Color>
        {
            Color.FromRgb(255,255,255),
            Color.FromRgb(0,0,0),
            Color.FromRgb(224,219,182),
            Color.FromRgb(190,190,190)
        };

    }

    public async Task<Dictionary<string, Book>> GetBooksLocalAsync(IFileSystem fileSystem)
    {

        if (books?.Count > 0)
            return books;
        try
        {
            
            var contents = await File.ReadAllTextAsync(Path.Combine(fileSystem.AppDataDirectory, "library.dat"));
            books = JsonSerializer.Deserialize<Dictionary<string, Book>>(contents);
            var path = Path.Combine(fileSystem.AppDataDirectory, "user_profile");
            if (File.Exists(path))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    var reader = new BinaryReader(stream);
                    _userFont = reader.ReadInt32();
                    _userFontSize = reader.ReadDouble();
                    _userBackColor = reader.ReadInt32();
                }
                UserProfileSet = true;
            }
            
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return books;
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
        var result = books.Remove(book.Slug);
        if (!result)
            throw new ArgumentException("Book is not present in the lib collection", nameof(book));
    }

    public void Save(IFileSystem fileSystem)
    {
        var content = JsonSerializer.Serialize(books);
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
                writer.Write(_userBackColor);

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
        if (books.ContainsKey(book.Slug))
            throw new ArgumentException($"Error book {book.Title} by {book.Author.name} is already present is the library");
        books.Add(book.Slug,book);
        
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
        return books.ContainsKey(book.Slug);
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
        Book result = (Book)books[readedBook.Slug];
        result.Readed = readedBook.Readed;

    }
    public void SetUserPreferences(string font, double fontsize, Color color)
    {
        _userFont = Fonts.IndexOf(font);
        _userFontSize = fontsize;
        _userBackColor = Backgrounds.IndexOf(color);
        UserProfileSet = true;
    }
    public Book GetLocalBookData(Book book)
    {
        ArgumentNullException.ThrowIfNull(book, nameof(book));
        return books[book.Slug];
    }

}

