﻿// <auto-generated />
using CamBotButHesFullOfDumbShite.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CamBotButHesFullOfDumbShite.Migrations
{
    [DbContext(typeof(ServerConfigEntities))]
    [Migration("20210305165246_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("CamBotButHesFullOfDumbShite.Database.ServerConfigModel", b =>
                {
                    b.Property<string>("guildid")
                        .HasColumnType("TEXT");

                    b.Property<string>("color")
                        .HasColumnType("TEXT");

                    b.Property<string>("prefix")
                        .HasColumnType("TEXT");

                    b.HasKey("guildid");

                    b.ToTable("serverConfigModel");
                });
#pragma warning restore 612, 618
        }
    }
}
