using System;
using System.Collections.Generic;
using System.Text;

namespace CamBotButHesFullOfDumbShite.TrefleApi
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TreflePageLinks
    {
        public string self { get; set; }
        public string plant { get; set; }
        public string genus { get; set; }
        public string first { get; set; }
        public string prev { get; set; }
        public string next { get; set; }
        public string last { get; set; }
    }

    public class TreflePageDatum
    {
        public int id { get; set; }
        public object common_name { get; set; }
        public string slug { get; set; }
        public string scientific_name { get; set; }
        public int? year { get; set; }
        public string bibliography { get; set; }
        public string author { get; set; }
        public string status { get; set; }
        public string rank { get; set; }
        public string family_common_name { get; set; }
        public int genus_id { get; set; }
        public string image_url { get; set; }
        public List<string> synonyms { get; set; }
        public string genus { get; set; }
        public string family { get; set; }
        public TreflePageLinks links { get; set; }
    }

    public class TreflePageMeta
    {
        public int total { get; set; }
    }

    public class TreflePageRoot
    {
        public List<TreflePageDatum> data { get; set; }
        public TreflePageLinks links { get; set; }
        public TreflePageMeta meta { get; set; }
    }


}
