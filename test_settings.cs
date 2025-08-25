using System;
using System.Text.Json;

// Простая копия класса AppSettings для тестирования
public class TestAppSettings
{
    public double VideoFps { get; set; } = 30.0;
    public bool UseAutoFps { get; set; } = true;
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Тестирование логики настроек FPS");
        
        // Тест 1: Ручной FPS 20
        var settings1 = new TestAppSettings();
        settings1.UseAutoFps = false;
        settings1.VideoFps = 20.0;
        
        Console.WriteLine($"Тест 1: UseAutoFps = {settings1.UseAutoFps}, VideoFps = {settings1.VideoFps}");
        Console.WriteLine($"VideoFps.ToString() = '{settings1.VideoFps.ToString()}'");
        Console.WriteLine($"VideoFps.ToString('F1') = '{settings1.VideoFps.ToString("F1")}'");
        
        // Тест 2: Сравнение строк
        string[] tags = { "15", "24", "25", "30", "50", "60", "Manual", "Auto" };
        string videoFps = settings1.VideoFps.ToString();
        
        Console.WriteLine($"\nТест 2: Ищем '{videoFps}' среди тегов:");
        foreach (string tag in tags)
        {
            bool exactMatch = tag == videoFps;
            bool numericMatch = false;
            
            if (double.TryParse(tag, out double tagFps) && double.TryParse(videoFps, out double settingFps))
            {
                numericMatch = Math.Abs(tagFps - settingFps) < 0.01;
            }
            
            Console.WriteLine($"  Тег '{tag}': точное совпадение = {exactMatch}, числовое совпадение = {numericMatch}");
        }
        
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}

