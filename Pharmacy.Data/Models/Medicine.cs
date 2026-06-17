namespace Pharmacy.Data.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MedicineType Type { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Category Category { get; set; }
    
    public Medicine() {}
}