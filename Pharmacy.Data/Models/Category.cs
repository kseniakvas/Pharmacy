namespace Pharmacy.Data.Models;

public sealed class Category
{
    public int Id { get; set; }
    public string CategoryName  { get; set; }
    public List<Medicine> MedicineList  { get; set; }
    public Category() { }
}