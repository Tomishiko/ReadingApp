using General;
using General.DataModels;
using Scraper;
using System.Text.RegularExpressions;
namespace TestProject
{
    public class UnitTest1
    {

        [Fact]
        public async void GetBookOverviewReturnsProperData()
        {

            WuxiaScraper scraper = new("https://wuxia.click/");
            //WuxiaScraper scraper = new("https://wuxia.click/search/alchemy");

            //Book book = await scraper.GetBookOverview();
            var result = await scraper.GetScriptContentAsync<TopBooks>();
            Assert.Fail("fick u");

        }
    }
}