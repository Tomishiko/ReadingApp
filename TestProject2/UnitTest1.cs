using WuxiaApp.Servs;
using Xunit.Abstractions;
using WuxiaClassLib.DataModels;

namespace TestProject2
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
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
    }
}