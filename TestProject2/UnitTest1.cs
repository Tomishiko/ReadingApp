using WuxiaApp.Servs;
using Xunit.Abstractions;
using WuxiaClassLib.DataModels;
using Scraper;
using General.DataModels;

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
    }
    public class ScraperTests
    {
        [Fact]
        public void ReloadThrowsExceptionWhenNoLinkProvided()
        {
            var scraper = new WuxiaScraper();
            Assert.Throws<NullReferenceException>(()=>scraper.Reload());
        }
        [Fact]
        public void LoadChangesStoredPageLink()
        {
            var scraper = new WuxiaScraper("https://wuxia.click/chapter/versatile-mage-2");
            var old = scraper.SiteUri;
            scraper.Load("https://wuxia.click/chapter/versatile-mage-3");
            Assert.Equal(old, scraper.SiteUri);

        }
        [Fact]
        public async void GetReadingPageReturnsProperData()
        {
            var scraper = new WuxiaScraper();
            scraper.Load("https://wuxia.click/chapter/lord-of-the-mysteries-1");
           
            var result = scraper.GetScriptContentDOMAsync<ChapterData>();
            
        }
        [Fact]
        public void WhyInTheFuckItsNotWorking()
        {
        }
    }
}