using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CamBotButHesFullOfDumbShite.PlayerLevelsDatabase
{
    public partial class PlayerLevelsEntities : DbContext
    {
        public virtual DbSet<PlayerLevelsModel> playerLevelsModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = @"/home/pi/CamBot/CamBotButHesFullOfDumbShite/PlayerLevels.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
    }
}
