namespace Scraper;
using HtmlAgilityPack;
using General.DataModels;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

public class WuxiaScraper
{
    private  HtmlWeb _web;
    private  HtmlDocument _currentPage;
    private  Uri _siteUri;

    public Uri SiteUri { get => _siteUri; set => _siteUri = value; }

    /// <summary>
    /// Creates an instance of WuxiaScraper and 
    /// immediately loads page
    /// </summary>
    /// <param name="link"> Link to the page to be scraped</param>
    public WuxiaScraper(string link)
    {
        _web = new HtmlWeb();
        _currentPage = _web.Load(link);
        _siteUri = new Uri(link);
       
    }

    /// <summary>
    /// Creates an empty instance of WuxiaScraper
    /// </summary>
    public WuxiaScraper()
    {
        _web = new HtmlWeb();
    }



    /// <summary>
    /// Gets text from reading page uri
    /// </summary>
    /// <returns>Tuple contains text and chapter name</returns>
    
    public (StringBuilder,string) GetReadingPage()
    {
        if(_currentPage is null)
            _currentPage = _web.Load(_siteUri);
        var Nodes = _currentPage.DocumentNode.SelectNodes("//div[@id='chapterText']");
        var Text = new StringBuilder();
        var chapName = Nodes[0].InnerText;
        for(int i=1;i<Nodes.Count; i++)
            Text.AppendLine(Nodes[i].InnerText);
        

        return (Text, chapName);
    }

    public async Task<Book> GetBookOverview()
    {
        _currentPage = await _web.LoadFromWebAsync(_siteUri.OriginalString);
        Book book = new();
        var temp = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[1]/div[1]/div/figure/div/img");// Picture html node
        book.PicturePath = Regex.Match(temp.Attributes["src"].Value, @".*\?").Value;

        temp = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[2]/div/div[1]");// Chapters number html node
        try
        {

            book.Chapters = Int32.Parse(Regex.Match(temp.InnerHtml, @"\d+").Value);

        }catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            book.Chapters = 0;
        }
        

        temp = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[2]/div/div[2]");// Ratings html node
        book.Ratings = Regex.Match(temp.InnerHtml, @"\d.\d*").Value;

        book.Title = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[1]/div[2]/div[1]/h5").InnerText;
        book.Description = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[3]/div[2]/div/div/div/div/div").InnerText;
        temp = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div/div/div[1]/div[2]/a");
        //book.Uri = "https://" + _siteUri.Host + temp.Attributes["href"].Value;

       

        return book;
    }

    

    private string GetNextPageLink()
    {
        var node = _currentPage.DocumentNode.SelectSingleNode("//button[@id='nextChapter']");
        if (node == null)
            throw new NullReferenceException("No next page with given xpath");
        node = node.ParentNode;
        var atribs = node.Attributes;
        string path = atribs["href"].Value;
        //if (link == null)
        //throw new NullReferenceException("There is no href atrib");

        return "https://" + _siteUri.Host + path;


    }

    private string GetPreviousPageLink()
    {
        var node = _currentPage.DocumentNode.SelectSingleNode("//button[@id='previousChapter']");
        if (node == null)
            throw new NullReferenceException("No previous page link with given xpath");
        node = node.ParentNode;
        var atribs = node.Attributes;
        string path = atribs["href"].Value;
        //if (link == null)
        //throw new NullReferenceException("There is no href atrib");

        return "https://" + _siteUri.Host + path;
    }
    private string GetChapterName()
    {

        var node = _currentPage.DocumentNode.SelectSingleNode("/html/body/div[1]/div/div[3]/div[1]/div[1]/div/div[2]/h1");
        if (node == null)
            throw new NullReferenceException("No chapter with given xpath");
        return node.InnerText;
    }

    public async Task<T> GetScriptContentAsync<T>()
    {
        _currentPage = await _web.LoadFromWebAsync(_siteUri.OriginalString);
        if(_web.StatusCode != HttpStatusCode.OK)
        {
            Debug.WriteLine($"Unable to get script content, http status code was {_web.StatusCode}");
            throw new HttpRequestException($"Unable to get script content, http status code was {_web.StatusCode}");
        }
        var scripttext = _currentPage.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']")?.InnerText;

        if(scripttext == null)
            throw new NullReferenceException($"Cant get next data {nameof(scripttext)} is null");

          var  deserialized = JsonSerializer.Deserialize<T>(scripttext);

        if(deserialized == null)
            throw new NullReferenceException($"{nameof(deserialized)} cant be null here");

        return deserialized;    
        //[JSON].props.pageProps.dehydratedState.queries.[0].state.data


    }


    public async  Task<T> GetScriptContentDOMAsync<T>()
    {
         _currentPage = await _web.LoadFromWebAsync(_siteUri.AbsoluteUri);
            
        if (_web.StatusCode != HttpStatusCode.OK)
        {
            Debug.WriteLine($"Unable to get script content, http status code was {_web.StatusCode}");
            throw new HttpRequestException($"Unable to get script content, http status code was {_web.StatusCode}");
        }
        var scripttext = _currentPage.DocumentNode.SelectSingleNode("//script[@id='__NEXT_DATA__']")?.InnerText;

        if (scripttext is null)
            throw new NullReferenceException($"Cant get next data {nameof(scripttext)} is null");

        //var deserialized = JsonSerializer.Deserialize<T>(scripttext);

        JsonNode pageDOM = JsonNode.Parse(scripttext);
        var dataNode = pageDOM["props"]["pageProps"]["dehydratedState"]["queries"][0]["state"]["data"];
        var deserialized = dataNode.Deserialize<T>();
        if (deserialized == null)
            throw new NullReferenceException($"{nameof(deserialized)} cant be null here");

        return deserialized;
        ////[JSON].props.pageProps.dehydratedState.queries.[0].state.data
        //0001000

    }
}
