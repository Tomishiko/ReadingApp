using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WuxiaApp.Servs;
public class PreferenceServices : INotifyPropertyChanged
{
    static public readonly List<string> Fonts = new List<string>
        {
            "OpenSansRegular",
            "SegoeRegular",
            "SegoePrint",
            "Arial",
            "Calibri",
            "Roboto",
            "Tahoma",
            "TimesNewRoman",
            "Georgia",
            "Merriweather"
        };

    static public readonly List<Color> Backgrounds = new List<Color>
        {
            Color.FromRgb(255,255,255),
            Color.FromRgb(0,0,0),
            Color.FromRgb(224,219,182),
            Color.FromRgb(190,190,190)
        };

    int _userFont;
    double _userFontSize;
    int _userBackColor;
    IPreferences preferences;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Font
    {
        get => Fonts[_userFont];
        set
        {
            if (Font == value)
                return;
            int index = Fonts.IndexOf(value);
            _userFont = index;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Font)));
        }
    }
    public Color BackColor
    {
        get => Backgrounds[_userBackColor];
        set
        {
            if (value == BackColor)
                return;
            int index = Backgrounds.IndexOf(value);
            _userBackColor = index;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackColor)));
        }
    }
    public double FontSize
    {
        get => _userFontSize;
        set
        {
            if (value == FontSize)
                return;
            _userFontSize = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
        }
    }



    public PreferenceServices(IPreferences preferences)
    {
        this.preferences = preferences;
        BackColor = Backgrounds[preferences.Get("BackColor", 0)];
        Font = Fonts[preferences.Get("Font", 0)];
        FontSize = preferences.Get("FontSize", 18.0);



    }

    /// <summary>
    /// Updates user preferences 
    /// </summary>
    /// <param name="font"></param>
    /// <param name="fontsize"></param>
    /// <param name="color"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetUserPreferences(string font, double fontsize, Color color)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(font);
        ArgumentNullException.ThrowIfNull(fontsize);
        ArgumentNullException.ThrowIfNull(color);

        _userFont = Fonts.IndexOf(font);
        _userFontSize = fontsize;
        _userBackColor = Backgrounds.IndexOf(color);
    }

    public void Save()
    {
        preferences.Set(nameof(FontSize), _userFontSize);
        preferences.Set(nameof(Font), _userFont);
        preferences.Set(nameof(BackColor), _userBackColor);


    }

}