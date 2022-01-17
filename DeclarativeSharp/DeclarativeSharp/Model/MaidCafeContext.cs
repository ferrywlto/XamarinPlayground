using System.IO;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace DeclarativeSharp.Model {
    public class MaidCafeContext : Microsoft.EntityFrameworkCore.DbContext{
        public DbSet<Cafe> Cafes { get; set; }
        public DbSet<Maid> Maids { get; set; }

        public MaidCafeContext() {
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "cafes.db3");

            optionsBuilder
                .UseSqlite($"Filename={dbPath}");
        }
    }
}
