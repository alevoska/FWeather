using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace FWeather.Models
{

    public class WeatherContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Location> Locations { get; set; }

        public string DbPath { get; }

        public WeatherContext()
        {
            DbPath = System.IO.Path.Join(Directory.GetCurrentDirectory(), "Database", "weather.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
            .HasKey(i => new { i.LocationId, i.Date });

            modelBuilder.Entity<Item>()
            .HasOne(i => i.Location)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.LocationId);

            modelBuilder.Entity<Location>()
            .HasMany(l => l.Items)
            .WithOne(i => i.Location);
        }
    }
}