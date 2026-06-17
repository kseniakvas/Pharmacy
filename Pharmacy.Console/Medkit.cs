using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Data.Models;

namespace Pharmacy;

public sealed class Medkit
{
    private readonly AppDbContext _db;
    private readonly Category _defaultCategory;

    public Medkit(AppDbContext context)
    {
        _db = context;
        var category = _db.Categories.FirstOrDefault(c => c.CategoryName == "Невизначено");
        if (category == null)
        {
            category = new Category { CategoryName = "Невизначено" };
            _db.Categories.Add(category);
            _db.SaveChanges();
        }

        _defaultCategory = category;
    }

    public void MainMenu()
    {
        while (true)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"ДОМАШНЯ АПТЕЧКА",42}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
            Console.ResetColor();

            Console.WriteLine("1. Додати ліки — назва, тип, кількість, термін, категорія");
            Console.WriteLine("2. Переглянути аптечку — вивід таблицею");
            Console.WriteLine("3. Пошук — за частиною назви або категорією");
            Console.WriteLine("4. Використати ліки — зменшити кількість");
            Console.WriteLine("5. Поповнити запас — збільшити кількість, оновити термін");
            Console.WriteLine("6. Видалити ліки — за ID, з підтвердженням");
            Console.WriteLine("7. Звіти — прострочені, закінчуються, закінчилися");
            Console.WriteLine("0. Вихід");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
            Console.ResetColor();
            Console.Write("\nВаш вибір (введіть номер і натисніть Enter): ");

            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    AddMedicine();
                    break; 
                case "2":
                    ViewMedicineCabinet();
                    break;
                case "3":
                    SearchMedicine();
                    break;
                case "4":
                    UseMedicine(); 
                    break;
                case "5":
                    // RestockMedicine(); 
                    break;
                case "6":
                    // RemoveMedicine(); 
                    break;
                case "7":
                    GenerateReports(); 
                    break;
                case "0":
                    return; 
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nПомилка: Некоректний ввід. Будь ласка, введіть число від 1 до 8.");
                    Console.ResetColor();
                    Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void AddMedicine()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{"ДОДАТИ ЛІКИ",38}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();

        var name = InputValidations.ReadRequiredInput("Введіть назву ліків: ");

        Console.WriteLine("1 - Таблетки, 2 - Сироп, 3 - Мазь, 4 - Інше");
        Console.Write("Оберіть тип (1-4): ");
        var typeInput = Console.ReadLine();
        var type = typeInput switch
        {
            "1" => MedicineType.Таблетки,
            "2" => MedicineType.Сироп,
            "3" => MedicineType.Мазь,
            "4" => MedicineType.Інше,
            _ => MedicineType.Інше
        };

        int quantity;
        while (true)
        {
            Console.Write("Введіть кількість: ");
            if (int.TryParse(Console.ReadLine(), out quantity) && quantity >= 0)
                break;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Помилка: Кількість має бути цілим числом і не може бути від'ємною!");
            Console.ResetColor();
        }

        DateTime expirationDate;
        while (true)
        {
            Console.Write("Введіть термін придатності (РРРР-ММ-ДД): ");
            if (DateTime.TryParse(Console.ReadLine(), out expirationDate))
                break;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Помилка: Некоректний формат дати. Використовуйте РРРР-ММ-ДД.");
            Console.ResetColor();
        }

        var categoryName =
            InputValidations.ReadRequiredInput("\nВведіть категорію (напр., 'Від застуди', 'Знеболювальні'): ");

        var category = _db.Categories.FirstOrDefault(c => c.CategoryName.ToLower() == categoryName.ToLower());

        if (category == null)
        {
            category = new Category { CategoryName = categoryName };
            _db.Categories.Add(category);
        }

        var newMedicine = new Medicine
        {
            Name = name,
            Type = type,
            Quantity = quantity,
            ExpirationDate = expirationDate,
            Category = category
        };

        _db.Medicines.Add(newMedicine);
        _db.SaveChanges();

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("===================================");
        Console.WriteLine("   Ліки успішно додано в аптечку!  ");
        Console.WriteLine("===================================");
        Console.ResetColor();
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

    private void UseMedicine()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{"ВИКОРИСТАТИ ЛІКИ",41}");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
    Console.ResetColor();
    
    var medicines = _db.Medicines.Include(m => m.Category).ToList();

    if (!medicines.Any())
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Ваша аптечка порожня. Немає ліків для використання.");
        Console.ResetColor();
        Console.WriteLine("\nНатисніть Enter для повернення до меню...");
        Console.ReadLine();
        return;
    }

    Console.WriteLine("ДОСТУПНІ ЛІКИ:\n");
    PrintMedicinesTable(medicines);

    Medicine selectedMedicine = null;
    
    while (true)
    {
        Console.Write("Введіть ID ліків (або 0 для скасування): ");
        var input = Console.ReadLine()?.Trim();

        if (input == "0") return; 

        if (int.TryParse(input, out int id))
        {
            selectedMedicine = medicines.FirstOrDefault(m => m.Id == id);
            
            if (selectedMedicine != null)
            {
                break; 
            }
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Ліки з ID {id} не знайдено. Спробуйте ще раз.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Помилка: Введіть коректне числове ID.");
            Console.ResetColor();
        }
    }

    Console.WriteLine($"\nВи обрали: {selectedMedicine.Name}");
    Console.WriteLine($"Поточний залишок: {selectedMedicine.Quantity} шт.");
    
    if (selectedMedicine.Quantity == 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("На жаль, ці ліки вже закінчилися. Спочатку поповніть запас.");
        Console.ResetColor();
        Console.WriteLine("\nНатисніть Enter для повернення до меню...");
        Console.ReadLine();
        return;
    }
    
    while (true)
    {
        Console.Write("\nСкільки одиниць ви хочете використати? ");
        var amountInput = Console.ReadLine()?.Trim();

        if (int.TryParse(amountInput, out int amountToUse))
        {
            if (amountToUse <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: Кількість має бути більшою за нуль.");
                Console.ResetColor();
            }
            else if (amountToUse > selectedMedicine.Quantity)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка: Ви не можете використати більше, ніж є в наявності ({selectedMedicine.Quantity} шт.).");
                Console.ResetColor();
            }
            else
            {
                selectedMedicine.Quantity -= amountToUse;
                _db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=======================================================");
                Console.WriteLine($" Успішно використано {amountToUse} шт. Залишок: {selectedMedicine.Quantity} шт.");
                Console.WriteLine("=======================================================");
                Console.ResetColor();
                break; 
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Помилка: Введіть коректне ціле число.");
            Console.ResetColor();
        }
    }

    Console.WriteLine("\nНатисніть Enter для продовження...");
    Console.ReadLine();
}

    private void ViewMedicineCabinet()
    {
        Console.Clear();
        
        var medicines = _db.Medicines.Include(m => m.Category).ToList();

        if (!medicines.Any())
        {
            Console.WriteLine("Ваша аптечка наразі порожня.");
            Console.WriteLine("\nНатисніть Enter для повернення до меню...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("\nВМІСТ ВАШОЇ АПТЕЧКИ:");
        PrintMedicinesTable(medicines);

        Console.WriteLine("\nНатисніть Enter для повернення до меню...");
        Console.ReadLine();
    }

    private void SearchMedicine()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{"ПОШУК ЛІКІВ",38}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();

        Console.Write("Введіть частину назви або категорію (0 для виходу): ");
        var searchingInput = Console.ReadLine()?.Trim();

        if (searchingInput == "0" || string.IsNullOrEmpty(searchingInput)) return;
        
        var foundMedicines = _db.Medicines.Include(m => m.Category)
            .Where(m => m.Name.ToLower().Contains(searchingInput.ToLower()) ||
                        m.Category.CategoryName.ToLower().Contains(searchingInput.ToLower()))
            .ToList();

        if (foundMedicines.Any())
        {
            Console.Clear();
            Console.WriteLine($"\nРЕЗУЛЬТАТИ ПОШУКУ ДЛЯ: '{searchingInput}'");
            PrintMedicinesTable(foundMedicines);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nЛіки за вашим запитом не знайдено.");
            Console.ResetColor();
        }

        Console.WriteLine("\nНатисніть Enter для продовження...");
        Console.ReadLine();
    }

    private void PrintMedicinesTable(List<Medicine> medicines)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();

        Console.WriteLine($" {"ID",-3} | {"НАЗВА",-18} | {"КАТЕГОРІЯ",-15} | {"ТИП",-10} | {"КІЛ-ТЬ",-6} | {"ТЕРМІН"}");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();

        var today = DateTime.Today;

        foreach (var med in medicines)
        {
            var isExpired = med.ExpirationDate.Date < today;
            var isExpiringSoon = (med.ExpirationDate.Date - today).TotalDays <= 30 && !isExpired;

            if (isExpired)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (isExpiringSoon) Console.ForegroundColor = ConsoleColor.Yellow;

            var categoryName = med.Category?.CategoryName ?? "Без категорії";
            var dateStr = med.ExpirationDate.ToString("yyyy-MM-dd");

            var statusStr = isExpired ? " ПРОСТРОЧЕНО" : isExpiringSoon ? " ЗАКІНЧУЄТЬСЯ" : "";
            Console.WriteLine($"... | {dateStr,-12} | {statusStr,-12}");

            Console.WriteLine(
                $" {med.Id,-3} | {med.Name,-18} | {categoryName,-15} | {med.Type,-10} | {med.Quantity,-6} | {dateStr}{statusStr}");

            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯\n");
        Console.ResetColor();
    }
    
    private void GenerateReports()
{
    while (true)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{"ЗВІТИ ТА АНАЛІТИКА",42}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();

        Console.WriteLine("1. Прострочені ліки");
        Console.WriteLine("2. Ліки, термін яких закінчується (< 30 днів)");
        Console.WriteLine("3. Ліки, що закінчилися (кількість 0)");
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("4. СПИСОК ПОКУПОК (Закінчились або прострочені)");
        Console.ResetColor();
        
        Console.WriteLine("0. Повернутися до головного меню");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯");
        Console.ResetColor();
        Console.Write("\nВаш вибір: ");

        var input = Console.ReadLine()?.Trim();
        var today = DateTime.Today;

        switch (input)
        {
            case "1":
                var expired = _db.Medicines.Include(m => m.Category)
                    .Where(m => m.ExpirationDate.Date < today)
                    .OrderBy(m => m.ExpirationDate)
                    .ToList();
                ShowReport("ПРОСТРОЧЕНІ ЛІКИ", expired);
                break;

            case "2":
                var targetDate = today.AddDays(30);
                var expiringSoon = _db.Medicines.Include(m => m.Category)
                    .Where(m => m.ExpirationDate.Date >= today && m.ExpirationDate.Date <= targetDate)
                    .OrderBy(m => m.ExpirationDate)
                    .ToList();
                ShowReport("ТЕРМІН ЗАКІНЧУЄТЬСЯ (< 30 ДНІВ)", expiringSoon);
                break;

            case "3":
                var outOfStock = _db.Medicines.Include(m => m.Category)
                    .Where(m => m.Quantity == 0)
                    .OrderBy(m => m.Name)
                    .ToList();
                ShowReport("ЛІКИ, ЩО ЗАКІНЧИЛИСЯ (КІЛЬКІСТЬ = 0)", outOfStock);
                break;

            case "4":
                var shoppingList = _db.Medicines.Include(m => m.Category)
                    .Where(m => m.Quantity == 0 || m.ExpirationDate.Date < today)
                    .OrderBy(m => m.Category.CategoryName).ThenBy(m => m.Name)
                    .ToList();
                ShowReport("СПИСОК ПОКУПОК (ЩО ТРЕБА КУПИТИ)", shoppingList);
                break;

            case "0":
                return; 

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nПомилка: Некоректний вибір.");
                Console.ResetColor();
                Console.WriteLine("Натисніть Enter для продовження...");
                Console.ReadLine();
                break;
        }
    }
}
    
private void ShowReport(string title, List<Medicine> medicines)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n--- ЗВІТ: {title} ---");
    Console.ResetColor();

    if (!medicines.Any())
    {
        Console.WriteLine("\nУ цій категорії ліків не знайдено. Все чудово!");
    }
    else
    {
        PrintMedicinesTable(medicines);
    }

    Console.WriteLine("\nНатисніть Enter для повернення до меню звітів...");
    Console.ReadLine();
}
}