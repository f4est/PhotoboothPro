using System;
using System.Windows;
using System.Windows.Controls;
using UnifiedPhotoBooth;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using NAudio.Wave;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для VideoBoothSettingsPage.xaml
    /// </summary>
    public partial class VideoBoothSettingsPage : Page
    {
        private AppSettings _appSettings;

        public VideoBoothSettingsPage(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            
            // Инициализация UI элементов
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Заполняем список микрофонов
            RefreshMicrophoneList();
            
            // Устанавливаем выбранный микрофон
            if (cbMicrophones.Items.Count > _appSettings.MicrophoneIndex && _appSettings.MicrophoneIndex >= 0)
            {
                cbMicrophones.SelectedIndex = _appSettings.MicrophoneIndex;
            }
            else if (cbMicrophones.Items.Count > 0)
            {
                cbMicrophones.SelectedIndex = 0;
            }
            
            // Устанавливаем длительность записи видео
            string recordingDuration = _appSettings.RecordingDuration.ToString();
            bool durationSet = false;
            foreach (ComboBoxItem item in cbRecordingDuration.Items)
            {
                if (item.Content.ToString() == recordingDuration)
                {
                    cbRecordingDuration.SelectedItem = item;
                    durationSet = true;
                    break;
                }
            }
            
            // Если не нашли подходящее значение в списке, устанавливаем значение по умолчанию
            if (!durationSet && cbRecordingDuration.Items.Count > 0)
            {
                cbRecordingDuration.SelectedIndex = 1; // По умолчанию 15 секунд (второй элемент)
            }
            
            // Устанавливаем время отсчета для видео
            string videoCountdownTime = _appSettings.VideoCountdownTime.ToString();
            bool countdownSet = false;
            foreach (ComboBoxItem item in cbVideoCountdownTime.Items)
            {
                if (item.Content.ToString() == videoCountdownTime)
                {
                    cbVideoCountdownTime.SelectedItem = item;
                    countdownSet = true;
                    break;
                }
            }
            
            // Если не нашли подходящее значение в списке, устанавливаем значение по умолчанию
            if (!countdownSet && cbVideoCountdownTime.Items.Count > 0)
            {
                cbVideoCountdownTime.SelectedIndex = 0; // По умолчанию 3 секунды (первый элемент)
            }
            
            // Если путь к оверлею существует, отображаем его
            if (!string.IsNullOrEmpty(_appSettings.OverlayImagePath))
            {
                txtOverlayPath.Text = _appSettings.OverlayImagePath;
                LoadOverlayPreview(_appSettings.OverlayImagePath);
            }
        }

        private void RefreshMicrophoneList()
        {
            cbMicrophones.Items.Clear();
            
            // Получаем список доступных устройств ввода
            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                var capabilities = NAudio.Wave.WaveIn.GetCapabilities(i);
                cbMicrophones.Items.Add($"{capabilities.ProductName}");
            }
            
            if (cbMicrophones.Items.Count == 0)
            {
                cbMicrophones.Items.Add("Микрофоны не найдены");
            }
        }

        private void CbMicrophones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMicrophones.SelectedIndex >= 0 && cbMicrophones.SelectedItem.ToString() != "Микрофоны не найдены")
            {
                _appSettings.MicrophoneIndex = cbMicrophones.SelectedIndex;
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void CbRecordingDuration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbRecordingDuration.SelectedItem != null)
                {
                    ComboBoxItem selectedItem = cbRecordingDuration.SelectedItem as ComboBoxItem;
                    if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int duration))
                    {
                        _appSettings.RecordingDuration = duration;
                        SettingsManager.SaveSettings(_appSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении длительности записи: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbVideoCountdownTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbVideoCountdownTime.SelectedItem != null)
                {
                    ComboBoxItem selectedItem = cbVideoCountdownTime.SelectedItem as ComboBoxItem;
                    if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int time))
                    {
                        _appSettings.VideoCountdownTime = time;
                        SettingsManager.SaveSettings(_appSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении времени отсчета: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLoadOverlay_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp|Все файлы|*.*",
                Title = "Выберите изображение оверлея"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                // Сохраняем путь к выбранному файлу
                _appSettings.OverlayImagePath = openFileDialog.FileName;
                txtOverlayPath.Text = openFileDialog.FileName;
                
                // Сохраняем настройки
                SettingsManager.SaveSettings(_appSettings);
                
                // Загружаем предпросмотр оверлея
                LoadOverlayPreview(openFileDialog.FileName);
                
                MessageBox.Show("Оверлей загружен успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void LoadOverlayPreview(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(path);
                    image.EndInit();
                    imgOverlayPreview.Source = image;
                }
                else
                {
                    imgOverlayPreview.Source = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предпросмотра оверлея: {ex.Message}", "Ошибка", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                imgOverlayPreview.Source = null;
            }
        }

        private void BtnClearOverlay_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.OverlayImagePath = null;
            txtOverlayPath.Text = "Не выбрано";
            imgOverlayPreview.Source = null;
            
            // Сохраняем настройки
            SettingsManager.SaveSettings(_appSettings);
            
            MessageBox.Show("Оверлей очищен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}