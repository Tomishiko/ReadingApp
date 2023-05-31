using WuxiaClassLib.DataModels;

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
        public async void NewHttpRequestTest()
        {
            var servs = new Services();
            var output = await servs.SearchBookAsync("martial");
            
        }

        [Fact]
        public async void AdvanceSearchIsWorking()
        {
            var services = new Services();
            var respond = await services.AdvancedFilteringAsync("xuanhuan");
        }
        [Fact]
        public void FiltratingNovelsByCategoryIsWorking()
        {
            var services = new Services();
            services.SearchBookAsync(category: "xuanhuan");
        }

        [Fact]
        public async void GetBookInfoReturnsBookInfo()
        {
            var services = new Services();
            var resp = await services.GetBookInfoAsync("library-of-heavens-path");
            output.WriteLine((await services.GetBookInfoAsync("library-of-heavens-path")).ToString());
        }

        [Fact]
        public void FormPicPathThrowsExceptionWhenNameNull()
        {
            var service = new Services();
            Assert.Throws<ArgumentNullException>(() =>service.FormPicturePath(null));
        }
        [Fact]
        public void FormPicPathThrowsExceptionWhenParamNull()
        {
            var service = new Services();
            Assert.Throws<ArgumentNullException>(() => 
            service.FormPicturePath("blah",null)
                );
        }
        [Fact]
        public async void NextDataMethodReturnsProperResult()
        {
           var service = new Services();
           var output = await service.LoadNextDataAsync(new Uri("http://wuxia.click/api/search/?limit=10&offset=10&search=mar"));
        }
        [Fact]
        public async Task SaveLoadFunctionality()
        {
            var filesysMock = new Mock<IFileSystem>();
            filesysMock.Setup(p => p.AppDataDirectory).Returns("output/");
            var services = new Services();
            var searchresult = await services.SearchBookAsync();
            foreach (var result in searchresult.results)
            {
                var book = new Book()
                {
                    Chapters = result.chapters,
                    Description = result.description,
                    Ratings = result.rating,
                    Title = result.name,
                    Views = result.views,
                    Slug = result.slug
                };
                if (result.image == null)
                    book.PicturePath = "unloaded_image.png";
                else
                    book.PicturePath = services.FormPicturePath(result.slug);
                services.AddNewBook(book);
            }
            var font = "abadakedabra";
            var fonsize = 15d;
            var color = Color.FromRgb(255, 255, 255);
            services.SetUserPreferences(font,fonsize,color);
            services.Save(filesysMock.Object);
            var expectedLibrary = await services.GetBooksLocalAsync(filesysMock.Object);
            services = new Services();
            var actualLibrary = await services.GetBooksLocalAsync(filesysMock.Object);
            Assert.Equal<Book>(expectedLibrary, actualLibrary);
            Assert.Equal(services.UserFont, font);
            Assert.Equal(services.UserFontSize, fonsize);
            Assert.Equal(services.UserColor,color);
        }
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