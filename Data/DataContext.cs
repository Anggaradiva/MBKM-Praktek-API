using Microsoft.EntityFrameworkCore;
using Praktek_API.Models;
using Praktek_API.Models;

namespace Praktek_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Karyawan> Karyawan { get; set; }

    }
}