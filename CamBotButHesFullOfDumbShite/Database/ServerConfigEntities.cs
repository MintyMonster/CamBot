using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CamBotButHesFullOfDumbShite.Database
{
    public partial class ServerConfigEntities : DbContext
    {
        public virtual DbSet<ServerConfigModel> serverConfigModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = @"/home/pi/CamBot/CamBot/CamBotButHesFullOfDumbShite/ServerConfig.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
    }
}
