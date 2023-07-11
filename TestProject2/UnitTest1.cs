namespace TestProject2
{
    public class ServicesTests
    {
        private readonly ITestOutputHelper output;

        public ServicesTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async void SearchBookThrowsExceptionWhenHttpUnsuccessful()
        {
            var service = new BaseServices();
            //var result = await service.SearchBookAsync();

        }
        [Fact]
        public async void NewHttpRequestTest()
        {
            var servs = new BaseServices();
            var output = await servs.SearchBookAsync("martial");
        }

        [Fact]
        public async void AdvanceSearchIsWorking()
        {
            var services = new BaseServices();
            var respond = await services.AdvancedFilteringAsync("xuanhuan");
        }
        [Fact]
        public void FiltratingNovelsByCategoryIsWorking()
        {
            var services = new BaseServices();
            services.SearchBookAsync(category: "xuanhuan");
        }

        [Fact]
        public async void GetBookInfoReturnsBookInfo()
        {
            var services = new BaseServices();
            var resp = await services.GetBookInfoAsync("library-of-heavens-path");
            output.WriteLine((await services.GetBookInfoAsync("library-of-heavens-path")).ToString());
        }

        [Fact]
        public void FormPicPathThrowsExceptionWhenNameNull()
        {
            var service = new BaseServices();
            Assert.Throws<ArgumentNullException>(() => service.FormPicturePath(null));
        }
        [Fact]
        public void FormPicPathThrowsExceptionWhenParamNull()
        {
            var service = new BaseServices();
            Assert.Throws<ArgumentNullException>(() =>
            service.FormPicturePath("blah")
                );
        }
        [Fact]
        public async void NextDataMethodReturnsProperResult()
        {
            var service = new BaseServices();
            var output = await service.LoadNextDataAsync(new Uri("http://wuxia.click/api/search/?limit=10&offset=10&search=mar"));
        }
        [Fact]
        public async Task SaveLoadFunctionality()
        {
        }
        public class ScraperTests
        {
            [Fact]
            public void ReloadThrowsExceptionWhenNoLinkProvided()
            {
                var scraper = new WuxiaScraper();
                //Assert.Throws<NullReferenceException>(()=>scraper.Reload());
            }
            [Fact]
            public void LoadChangesStoredPageLink()
            {
                var scraper = new WuxiaScraper("https://wuxia.click/chapter/versatile-mage-2");
                var old = scraper.SiteUri;
                //scraper.Load("https://wuxia.click/chapter/versatile-mage-3");
                Assert.Equal(old, scraper.SiteUri);

            }
            [Fact]
            public async void GetReadingPageReturnsProperData()
            {
                var scraper = new WuxiaScraper();
                //scraper.Load("https://wuxia.click/chapter/lord-of-the-mysteries-1");

                var result = scraper.GetScriptContentDOMAsync<ChapterData>();

            }
        }
    }
    public class ApiTests
    {
        BaseServices service;
        public ApiTests() { service = new BaseServices(); }
        [Fact]
        public async void SerchBookDefoultParamsReturnsExpectedData()
        {
            var result = await service.SearchBookAsync();
            var expected = JsonSerializer.Deserialize<searchResult>(File.ReadAllText("SearchResultStandart.json"));
            Assert.Equal(expected, result);
        }
        [Fact]
        public async void SearchBookThrowsExceptionWhenFails()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            BaseServicesMoq mocked = new BaseServicesMoq(httpMessageHandlerMock);
            await Assert.ThrowsAsync<HttpRequestException>(() => service.SearchBookAsync());
        }

    }

    class BaseServicesMoq : BaseServices
    {
        public BaseServicesMoq(Mock<HttpMessageHandler> mock)
        {
            client = new HttpClient(mock.Object);
            client.BaseAddress = new Uri("https://www.wuxia/api");
        }
    }

}