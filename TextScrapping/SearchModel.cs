using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuxiaClassLib.DataModels
{

    public class searchResult
    {
        public int count { get; set; }
        public string next { get; set; }
        public object previous { get; set; }
        public Result[] results { get; set; }


    }
    public class Result
    {
        public string name { get; set; }
        public string image { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public string rating { get; set; }
        public int ranking { get; set; }
        public string views { get; set; }
        public int chapters { get; set; }
        public Category[] category { get; set; }
        public Tag[] tag { get; set; }
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




}
