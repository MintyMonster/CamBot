using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CamBotButHesFullOfDumbShite.PlayerLevelsDatabase
{
    public partial class PlayerLevelsModel
    {
        [Key]
        public string playerId { get; set; }
        public string playerUsername { get; set; }
        public string points { get; set; }
    }
}
