namespace Pharmacy.Data.Models;

public class Medicine
{
    public int Id { get; set; }
    public string name { get; set; }
    public MedicineType type { get; set; }
    public int quantity { get; set; }
    public int categoryId { get; set; }
    public DateTime expirationDate { get; set; }
    public Category category { get; set; }
    
    public Medicine() {}
}