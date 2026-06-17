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
                    // UseMedicine(); 
                    break;
                case "5":
                    // RestockMedicine(); // Твоя функція для поповнення
                    break;
                case "6":
                    // RemoveMedicine(); // Твоя функція для видалення
                    break;
                case "7":
                    // GenerateReports(); // Твоя функція для звітів
                    break;
                case "0":
                    return; // Вихід з циклу та програми
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


    private void ViewMedicineCabinet()
    {
        Console.Clear();

        // Завантажуємо всі ліки разом з їхніми категоріями
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

        // Шукаємо збіги в назві ліків АБО в назві категорії
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
            // Логіка для бонусного завдання (кольори)
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
}