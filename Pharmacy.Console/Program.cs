using Pharmacy.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace Pharmacy;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        AppDbContext db = new AppDbContext();
        
        db.Database.Migrate();
        Medkit medkit = new Medkit(db);
        medkit.MainMenu();
    }
}