using WuxiaApp.Servs;
namespace XunitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var serv = new Services();
            serv.SearchBook("martial");
        }
    }
}