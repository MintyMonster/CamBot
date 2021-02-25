using System;
using System.Collections.Generic;
using System.Text;

namespace CamBotButHesFullOfDumbShite.Wordnik
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Citation
    {
        public string source { get; set; }
        public string cite { get; set; }
    }

    public class ExampleUs
    {
        public string text { get; set; }
        public int? position { get; set; }
    }

    public class WordnikRoot
    {
        public string id { get; set; }
        public string partOfSpeech { get; set; }
        public string attributionText { get; set; }
        public string sourceDictionary { get; set; }
        public string text { get; set; }
        public string sequence { get; set; }
        public int score { get; set; }
        public List<object> labels { get; set; }
        public List<Citation> citations { get; set; }
        public string word { get; set; }
        public List<object> relatedWords { get; set; }
        public List<ExampleUs> exampleUses { get; set; }
        public List<object> textProns { get; set; }
        public List<object> notes { get; set; }
        public string attributionUrl { get; set; }
        public string wordnikUrl { get; set; }
    }


}
