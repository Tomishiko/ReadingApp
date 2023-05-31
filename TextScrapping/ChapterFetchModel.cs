using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.DataModels;

public class ChapterData
{
    public int index { get; set; }
    public string title { get; set; }
    public string text { get; set; }
    public object nextChap { get; set; }
    public string novelParent { get; set; }
    public string novelParentName { get; set; }
    public object prevChap { get; set; }
    public int id { get; set; }
    public bool is_eighteen { get; set; }
}





