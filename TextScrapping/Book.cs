namespace General.DataModels;



public class Book
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PicturePath { get; set; }
    public string Uri { get; set; }
    public string Ratings { get; set; }
    public int Readed { get; set; } 
    public int Chapters { get; set; }
    public string Views { get; set; }
    public string Slug { get; set; }    
    public Author Author { get; set; }
    public Category[] Categories { get; set; }
    public string FirstChapter { get; set; }
    public DateTime LastUpdate { get; set; }
    public string Status { get; set; }
    public int Ranking { get; set; }



}




