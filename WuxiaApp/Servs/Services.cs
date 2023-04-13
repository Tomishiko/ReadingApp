using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using General.DataModels;
using WuxiaClassLib.DataModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace WuxiaApp.Servs;
public class Services
{
    List<Book> bookList;
    string api;
    static HttpClient client;
    
    public Services()
    {
        api = "https://wuxia.click/api/";
        client = new HttpClient() { BaseAddress = new Uri(api) };
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        
    }

    public async Task<List<Book>> GetBooksLocalAsync()
    {

        if (bookList?.Count > 0)
            return bookList;
        try
        {
            var contents = await File.ReadAllTextAsync(Path.Combine(FileSystem.Current.AppDataDirectory, "library.dat"));
            bookList = JsonSerializer.Deserialize<List<Book>>(contents);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return bookList;
    }

    public async Task<searchResult> SearchBookAsync(string searchPattern="",string ordering= "-total_views",string category="",string limit="10")
    {
        var query = new Dictionary<string, string>()
        {
            ["search"] = searchPattern,
            ["limit"] = "10",
            ["order"] = ordering,
            ["category_name"] = category,
            ["limit"]=limit
        };
        var querysrting = QueryHelpers.AddQueryString("search/", query);
        using var request = new HttpRequestMessage(HttpMethod.Get, querysrting);
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var jsonObject = await response.Content.ReadFromJsonAsync<searchResult>();


        return jsonObject;
    }

    public async Task<searchResult> AdvancedFilteringAsync(string categories="",string ordering="-total_views")
    {

        var query = new Dictionary<string, string>()
        {
            ["itemsPerPage"]="10",
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
        using StreamReader reader = new StreamReader(fileStream);

        string content = await reader.ReadToEndAsync();

        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);

        using FileStream outputStream = File.OpenWrite(targetFile);
        using StreamWriter streamWriter = new StreamWriter(outputStream);

        await streamWriter.WriteAsync(content);
    }

    public void DeleteBook(Book book)
    {
        bookList?.Remove(book);
       
    }

    public async Task Save()
    {
        var content = JsonSerializer.Serialize(bookList);
        using var stream = File.Open(
            Path.Combine(FileSystem.Current.AppDataDirectory, "library.dat"), FileMode.Create);
        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(content);

    }
    public void AddNewBook(Book book)
    {
        bookList.Add(book);
    }
    public async Task<BookInfo> GetBookInfoAsync(string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "novels/"+name);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BookInfo>();
        return result;

    }
}

