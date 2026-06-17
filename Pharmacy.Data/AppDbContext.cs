using Microsoft.EntityFrameworkCore;
using Pharmacy.Data.Models;

namespace Pharmacy.Data;

public class AppDbContext : DbContext
{
    public DbSet<Category>  Categories { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=medicine.db");
    }
}