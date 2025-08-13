using System;
using System.Windows;
using System.Windows.Controls;
using UnifiedPhotoBooth;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для PhotoBoothSettingsPage.xaml
    /// </summary>
    public partial class PhotoBoothSettingsPage : Page
    {
        private AppSettings _appSettings;

        public PhotoBoothSettingsPage(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            
            // Инициализация UI элементов
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Устанавливаем количество фотографий
            string photoCount = _appSettings.PhotoCount.ToString();
            foreach (ComboBoxItem item in cbPhotoCount.Items)
            {
                if (item.Content.ToString() == photoCount)
                {
                    cbPhotoCount.SelectedItem = item;
                    break;
                }
            }
            
            // Устанавливаем время отсчета для фото
            string photoCountdownTime = _appSettings.PhotoCountdownTime.ToString();
            foreach (ComboBoxItem item in cbPhotoCountdownTime.Items)
            {
                if (item.Content.ToString() == photoCountdownTime)
                {
                    cbPhotoCountdownTime.SelectedItem = item;
                    break;
                }
            }
            
            // Режим обработки фотографий
            int photoProcessingModeIndex = (int)_appSettings.PhotoProcessingMode;
            if (photoProcessingModeIndex >= 0 && photoProcessingModeIndex < cbPhotoProcessingMode.Items.Count)
            {
                cbPhotoProcessingMode.SelectedIndex = photoProcessingModeIndex;
            }
            else
            {
                cbPhotoProcessingMode.SelectedIndex = 0; // По умолчанию растягивание
            }
            
            // Если путь к рамке существует, отображаем его
            if (!string.IsNullOrEmpty(_appSettings.FrameTemplatePath))
            {
                txtFrameTemplatePath.Text = _appSettings.FrameTemplatePath;
                LoadFramePreview(_appSettings.FrameTemplatePath);
            }
        }

        private void CbPhotoCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhotoCount.SelectedItem != null)
            {
                _appSettings.PhotoCount = int.Parse(((ComboBoxItem)cbPhotoCount.SelectedItem).Content.ToString());
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void CbPhotoCountdownTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhotoCountdownTime.SelectedItem != null)
            {
                _appSettings.PhotoCountdownTime = int.Parse(((ComboBoxItem)cbPhotoCountdownTime.SelectedItem).Content.ToString());
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void CbPhotoProcessingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhotoProcessingMode.SelectedIndex >= 0)
            {
                _appSettings.PhotoProcessingMode = (ImageProcessingMode)cbPhotoProcessingMode.SelectedIndex;
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void BtnLoadFrameTemplate_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp|Все файлы|*.*",
                Title = "Выберите изображение рамки"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                // Сохраняем путь к выбранному файлу
                _appSettings.FrameTemplatePath = openFileDialog.FileName;
                txtFrameTemplatePath.Text = openFileDialog.FileName;
                
                // Сохраняем настройки
                SettingsManager.SaveSettings(_appSettings);
                
                // Загружаем предпросмотр рамки
                LoadFramePreview(openFileDialog.FileName);
                
                MessageBox.Show("Рамка загружена успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void LoadFramePreview(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(path);
                    image.EndInit();
                    imgFramePreview.Source = image;
                }
                else
                {
                    imgFramePreview.Source = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предпросмотра рамки: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                imgFramePreview.Source = null;
            }
        }

        private void BtnClearFrameTemplate_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.FrameTemplatePath = null;
            txtFrameTemplatePath.Text = "Не выбрано";
            imgFramePreview.Source = null;
            
            // Сохраняем настройки
            SettingsManager.SaveSettings(_appSettings);
            
            MessageBox.Show("Шаблон рамки очищен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSetupPositions_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли шаблон рамки
            if (string.IsNullOrEmpty(_appSettings.FrameTemplatePath) || !File.Exists(_appSettings.FrameTemplatePath))
            {
                MessageBox.Show("Сначала загрузите шаблон рамки.", "Необходим шаблон", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            // В связи с тем, что метод ShowPositionsSetupWindow отсутствует,
            // выводим сообщение о том, что функция будет доступна в следующей версии
            MessageBox.Show("Настройка позиций будет доступна в следующей версии приложения.", 
                           "В разработке", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}