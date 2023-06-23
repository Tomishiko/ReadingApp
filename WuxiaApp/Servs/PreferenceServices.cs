namespace WuxiaApp.Servs;
public class PreferenceServices : BaseServices
{
    public static bool UserProfileSet { get; set; } = false;

    public readonly List<String> Fonts;
    public readonly List<Color> Backgrounds;


    public string Font { get => Fonts[_userFont]; }
    public int FontIndex
    {
        get => _userFont;
        set
        {
            if (value >= Fonts.Count || value < 0) return;
            else _userFont = value;
        }
    }
    public int BackColorIndex
    {
        get => _userBackColor;
        set
        {
            if (value >= Backgrounds.Count || value < 0) return;
            else _userBackColor = value;
        }
    }
    public Color BackColor { get => Backgrounds[_userBackColor]; }
    public double FontSize { get => _userFontSize; }
    static PreferenceServices()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "user_profile");
        if (File.Exists(path))
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                var reader = new BinaryReader(stream);
                _userFont = reader.ReadInt32();
                _userFontSize = reader.ReadDouble();
                _userBackColor = reader.ReadInt32();
            }
            UserProfileSet = true;
        }

    }

    public PreferenceServices()
    {
        Fonts = new List<string>
        {
            "OpenSansRegular",
            "SegoeRegular",
            "SegoePrint",
            "Arial",
            "Calibri",
            "Roboto",
            "Tahoma",
            "TimesNewRoman"
        };
        Backgrounds = new List<Color>
        {
            Color.FromRgb(255,255,255),
            Color.FromRgb(0,0,0),
            Color.FromRgb(224,219,182),
            Color.FromRgb(190,190,190)
        };
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
        UserProfileSet = true;
    }

}