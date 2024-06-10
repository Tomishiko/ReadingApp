using System.ComponentModel;
namespace General.DataModels;



public partial class Book: IEquatable<Book>,INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string Title { get; set; }
    public string Description { get; set; }
    public string PicturePath { get; set; }
    public string Uri { get; set; }
    public string Ratings { get; set; }
    int readed;
    public int Readed
    {
        get { return readed; }
        set
        {
            if(readed != value)
            {
                readed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Readed)));
            }

        }
    }
    public int Chapters { get; set; }
    public string Views { get; set; }
    public string Slug { get; set; }    
    public Author Author { get; set; }
    public Category[] Categories { get; set; }
    public string FirstChapter { get; set; }
    public DateTime LastUpdate { get; set; }
    public string Status { get; set; }
    public int Ranking { get; set; }

    public bool Equals(Book? other)
    {
        return this.Slug == other?.Slug;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Book);
    }
    public override string ToString()
    {
        return Title;
    }
    public override int GetHashCode()
    {
        return Slug.GetHashCode();
    }
}




