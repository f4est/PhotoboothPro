using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace UnifiedPhotoBooth
{
    public static class SettingsManager
    {
        // Путь к файлу настроек в папке AppData
        private static readonly string SettingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UnifiedPhotoBooth");
            
        private static readonly string SettingsFilePath = Path.Combine(SettingsFolder, "settings.json");
        
        // Метод для сохранения настроек
        public static bool SaveSettings(AppSettings settings)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Сохранение настроек. Индекс камеры: {settings.CameraIndex}");
                
                // Создаем директорию, если она не существует
                if (!Directory.Exists(SettingsFolder))
                {
                    Directory.CreateDirectory(SettingsFolder);
                    System.Diagnostics.Debug.WriteLine($"Создана директория для настроек: {SettingsFolder}");
                }
                
                // Настройки сериализации JSON
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                
                // Сериализуем настройки в JSON
                string json = JsonSerializer.Serialize(settings, options);
                System.Diagnostics.Debug.WriteLine($"Настройки сериализованы в JSON, размер: {json.Length} байт");
                
                // Используем атомарную запись через временный файл
                string tempPath = SettingsFilePath + ".tmp";
                File.WriteAllText(tempPath, json, System.Text.Encoding.UTF8);
                
                // Проверяем что временный файл создан успешно
                if (File.Exists(tempPath) && new FileInfo(tempPath).Length > 0)
                {
                    // Заменяем старый файл новым
                    if (File.Exists(SettingsFilePath))
                    {
                        File.Delete(SettingsFilePath);
                        System.Diagnostics.Debug.WriteLine("Удален старый файл настроек");
                    }
                    
                    File.Move(tempPath, SettingsFilePath);
                    System.Diagnostics.Debug.WriteLine($"Настройки успешно сохранены в {SettingsFilePath}");
                    return true;
                }
                
                System.Diagnostics.Debug.WriteLine("Ошибка: временный файл настроек не создан или пуст");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение при сохранении настроек: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        
        // Метод для загрузки настроек
        public static AppSettings LoadSettings()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Загрузка настроек из {SettingsFilePath}");
                
                // Проверяем существование файла настроек
                if (!File.Exists(SettingsFilePath))
                {
                    System.Diagnostics.Debug.WriteLine("Файл настроек не существует, создаем настройки по умолчанию");
                    // Если файла нет, создаем настройки по умолчанию
                    var defaultSettings = new AppSettings();
                    SaveSettings(defaultSettings); // Сохраняем настройки по умолчанию
                    return defaultSettings;
                }
                
                // Читаем JSON из файла
                string json = File.ReadAllText(SettingsFilePath, System.Text.Encoding.UTF8);
                System.Diagnostics.Debug.WriteLine($"Прочитан JSON файл настроек, размер: {json.Length} байт");
                
                // Проверяем, что JSON не пустой
                if (string.IsNullOrWhiteSpace(json))
                {
                    System.Diagnostics.Debug.WriteLine("JSON файл настроек пуст, создаем настройки по умолчанию");
                    return new AppSettings();
                }
                
                // Настройки десериализации
                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNameCaseInsensitive = true
                };
                
                // Десериализуем JSON в объект настроек
                var settings = JsonSerializer.Deserialize<AppSettings>(json, options);
                
                // Проверяем результат десериализации
                if (settings == null)
                {
                    System.Diagnostics.Debug.WriteLine("Ошибка десериализации настроек, создаем настройки по умолчанию");
                    return new AppSettings();
                }
                
                // Валидируем настройки
                ValidateSettings(settings);
                
                System.Diagnostics.Debug.WriteLine($"Настройки успешно загружены. Индекс камеры: {settings.CameraIndex}");
                return settings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение при загрузке настроек: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}. Будут использованы настройки по умолчанию.", 
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return new AppSettings();
            }
        }
        
        // Метод для валидации настроек
        private static void ValidateSettings(AppSettings settings)
        {
            // Проверяем наличие списка позиций
            if (settings.PhotoPositions == null)
            {
                settings.PhotoPositions = new System.Collections.Generic.List<PhotoPosition>();
            }
            
            // Проверяем корректность числовых параметров
            if (settings.PhotoCount <= 0) settings.PhotoCount = 4;
            if (settings.PhotoCountdownTime <= 0) settings.PhotoCountdownTime = 3;
            if (settings.VideoCountdownTime <= 0) settings.VideoCountdownTime = 3;
            if (settings.RecordingDuration <= 0) settings.RecordingDuration = 15;
            
            // Проверяем настройки принтера
            if (settings.PrintWidth <= 0) settings.PrintWidth = 10.16; // 4 дюйма
            if (settings.PrintHeight <= 0) settings.PrintHeight = 15.24; // 6 дюймов
            if (settings.PrintCopies <= 0) settings.PrintCopies = 1;
            if (settings.PrintDpi <= 0) settings.PrintDpi = 300;
            
            // Проверка согласованности настроек принтера
            // Если заполнено поле PrinterName, но не заполнено SelectedPrinter, копируем значение
            if (!string.IsNullOrEmpty(settings.PrinterName) && string.IsNullOrEmpty(settings.SelectedPrinter))
            {
                settings.SelectedPrinter = settings.PrinterName;
            }
            // И наоборот
            else if (string.IsNullOrEmpty(settings.PrinterName) && !string.IsNullOrEmpty(settings.SelectedPrinter))
            {
                settings.PrinterName = settings.SelectedPrinter;
            }
            
            // Проверяем существование файлов
            if (!string.IsNullOrEmpty(settings.FrameTemplatePath) && !File.Exists(settings.FrameTemplatePath))
            {
                settings.FrameTemplatePath = null;
            }
            
            if (!string.IsNullOrEmpty(settings.OverlayImagePath) && !File.Exists(settings.OverlayImagePath))
            {
                settings.OverlayImagePath = null;
            }
        }
        
        // Метод для получения пути к файлу настроек (для диагностики)
        public static string GetSettingsFilePath()
        {
            return SettingsFilePath;
        }
    }
} 