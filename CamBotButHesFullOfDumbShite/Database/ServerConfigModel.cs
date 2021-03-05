using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CamBotButHesFullOfDumbShite.Database
{
    public partial class ServerConfigModel
    {
        [Key]
        public string guildid { get; set; }
        public string prefix { get; set; }
        public string color { get; set; }
    }
}
