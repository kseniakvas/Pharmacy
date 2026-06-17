namespace Pharmacy;

public static class InputValidations
{
    static public string ReadRequiredInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }
        }
    }
    
    static public DateTime? ReadOptionalDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
        
            if (string.IsNullOrEmpty(input)) 
                return null;

            if (DateTime.TryParse(input, out DateTime parsedDate))
            {
                return parsedDate;
            }
        
            Console.WriteLine("Invalid date format. Please try again (e.g., 01-12-26).");
        }
    }
    
    static public void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, currentLineCursor);
    }
}