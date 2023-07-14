
namespace General.DataModels;


public class BookInfo
{
    public string slug { get; set; }
    public Author author { get; set; }
    public Category[] category { get; set; }
    public string views { get; set; }
    public Tag[] tag { get; set; }
    public int chapters { get; set; }
    public int review_count { get; set; }
    public string image { get; set; }
    public string first_chapter { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string name { get; set; }
    public string linkNU { get; set; }
    public string description { get; set; }
    public int numOfChaps { get; set; }
    public string novelStatus { get; set; }
    public DateTime last_chap_updated { get; set; }
    public string rating { get; set; }
    public string original_image { get; set; }
    public bool dmca { get; set; }
    //public string[] other_names { get; set; }
    public int ranking { get; set; }
    public string human_views { get; set; }
}

public class Author
{
    public string name { get; set; }
    public string slug { get; set; }
}

public class Other_Names
{
}
public class Category
{
    public string name { get; set; }
    public string slug { get; set; }
}

public class Tag
{
    public int id { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
}
