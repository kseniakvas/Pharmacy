using Microsoft.EntityFrameworkCore;
using Pharmacy.Data.Models;

namespace Pharmacy.Data;

public class AppDbContext : DbContext
{
    public DbSet<Category>  Contacts { get; set; }
    public DbSet<Medicine> ContactGroups { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=contacts.db");
    }
}