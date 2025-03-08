using Microsoft.EntityFrameworkCore;
using TimeSheet.ApiService.Domain;

namespace TimeSheet.ApiService
{
    public class TimeSheetContext : DbContext
    {
        public DbSet<TimeSheetEntry> TimeSheetEntries { get; set; }

        public string DbPath { get; }

        public TimeSheetContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "timsheet.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
