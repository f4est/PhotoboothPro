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
                // Создаем директорию, если она не существует
                if (!Directory.Exists(SettingsFolder))
                {
                    Directory.CreateDirectory(SettingsFolder);
                }
                
                // Настройки сериализации JSON
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                
                // Сериализуем настройки в JSON
                string json = JsonSerializer.Serialize(settings, options);
                
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
                    }
                    File.Move(tempPath, SettingsFilePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        
        // Метод для загрузки настроек
        public static AppSettings LoadSettings()
        {
            try
            {
                // Проверяем существование файла настроек
                if (!File.Exists(SettingsFilePath))
                {
                    // Если файла нет, создаем настройки по умолчанию
                    var defaultSettings = new AppSettings();
                    SaveSettings(defaultSettings); // Сохраняем настройки по умолчанию
                    return defaultSettings;
                }
                
                // Читаем JSON из файла
                string json = File.ReadAllText(SettingsFilePath, System.Text.Encoding.UTF8);
                
                // Проверяем, что JSON не пустой
                if (string.IsNullOrWhiteSpace(json))
                {
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
                    return new AppSettings();
                }
                
                // Валидируем настройки
                ValidateSettings(settings);
                
                return settings;
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем настройки по умолчанию
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}\nБудут использованы настройки по умолчанию.", 
                              "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return new AppSettings();
            }
        }
        
        // Метод для получения пути к файлу настроек
        public static string GetSettingsFilePath()
        {
            return SettingsFilePath;
        }
        
        // Валидация настроек
        private static void ValidateSettings(AppSettings settings)
        {
            // Валидируем количество фотографий
            if (settings.PhotoCount < 1 || settings.PhotoCount > 10)
                settings.PhotoCount = 4;
                
            // Валидируем время обратного отсчета
            if (settings.PhotoCountdownTime < 1 || settings.PhotoCountdownTime > 10)
                settings.PhotoCountdownTime = 3;
                
            if (settings.VideoCountdownTime < 1 || settings.VideoCountdownTime > 10)
                settings.VideoCountdownTime = 3;
                
            // Валидируем продолжительность записи видео
            if (settings.RecordingDuration < 5 || settings.RecordingDuration > 300)
                settings.RecordingDuration = 15;
                
            // Валидируем индексы устройств
            if (settings.CameraIndex < 0)
                settings.CameraIndex = 0;
                
            if (settings.MicrophoneIndex < 0)
                settings.MicrophoneIndex = 0;
        }
    }
}