namespace General.DataModels;

public  class PopularBooksModel
{
    public string Genre { get; set; }
    public List<Book> Books { get; set; }
    public bool isLoading { get; set; }

    public PopularBooksModel() 
    {
        isLoading= true;
    }
    public PopularBooksModel(string gernre, List<Book> books)
    {
        isLoading = true;
        Genre = gernre;
        Books = books;
    }
}

