using System.ComponentModel;
using System.Runtime.Serialization;

namespace General.DataModels;



public partial class Book: IEquatable<Book>,INotifyPropertyChanged,ISerializable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public Book() { }
    #region Propertys
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Readed"));
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
    #endregion
    #region Methods

    
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
    #endregion

    #region SerializationDeserialization
    
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Title), Title,typeof(string));
        info.AddValue(nameof(PicturePath), PicturePath, typeof(string));
        info.AddValue(nameof(Uri), Uri, typeof(string));
        info.AddValue(nameof(Ratings), Ratings, typeof(string));
        info.AddValue(nameof(readed), readed, typeof(int));
        info.AddValue(nameof(Chapters), Chapters, typeof(int));
        info.AddValue(nameof(Views), Views, typeof(string));
        info.AddValue(nameof(Slug), Slug, typeof(string));
        info.AddValue(nameof(Author.name), Author.name, typeof(string));
        info.AddValue(nameof(LastUpdate), LastUpdate, typeof(DateTime));
        info.AddValue(nameof(Ranking), Ranking, typeof(int));
    }
    public Book(SerializationInfo info, StreamingContext context)
    {
        Title = info.GetValue(nameof(Title), typeof(string)) as string;
        PicturePath = info.GetValue(nameof(PicturePath), typeof(string)) as string;
        Uri = info.GetValue(nameof(Uri), typeof(string)) as string;
        Ratings = info.GetValue(nameof(Ratings), typeof(string)) as string;
        readed = (int)info.GetValue(nameof(readed), typeof(int));
        Chapters = (int)info.GetValue(nameof(Chapters), typeof(int));
        Views = info.GetValue(nameof(Views), typeof(string)) as string;
        Slug = info.GetValue(nameof(Slug), typeof(string)) as string;
        Author.name = info.GetValue(nameof(Author.name), typeof(string)) as string;
        LastUpdate = (DateTime)info.GetValue(nameof(LastUpdate), typeof(DateTime));
        Ranking = (int)info.GetValue(nameof(Ranking), typeof(int));
    }
    #endregion
}




