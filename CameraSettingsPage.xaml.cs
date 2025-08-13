using System;
using System.Windows;
using System.Windows.Controls;
using UnifiedPhotoBooth;
using System.Windows.Threading;
using OpenCvSharp;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для CameraSettingsPage.xaml
    /// </summary>
    public partial class CameraSettingsPage : Page
    {
        private AppSettings _appSettings;
        private VideoCapture _capture;
        private DispatcherTimer _previewTimer;
        private bool _isPreviewActive = false;

        public CameraSettingsPage(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            
            // Инициализация UI элементов
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Заполняем список камер
            RefreshCameraList();
            
            // Устанавливаем выбранную камеру
            if (cbCameras.Items.Count > _appSettings.CameraIndex && _appSettings.CameraIndex >= 0)
            {
                cbCameras.SelectedIndex = _appSettings.CameraIndex;
            }
            else if (cbCameras.Items.Count > 0)
            {
                cbCameras.SelectedIndex = 0;
            }
            
            // Устанавливаем режим поворота
            if (!string.IsNullOrEmpty(_appSettings.RotationMode))
            {
                foreach (ComboBoxItem item in cbCameraRotation.Items)
                {
                    if (item.Content.ToString() == _appSettings.RotationMode)
                    {
                        cbCameraRotation.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                cbCameraRotation.SelectedIndex = 0;
            }
            
            // Устанавливаем зеркальное отображение
            chkMirrorMode.IsChecked = _appSettings.MirrorMode;
        }

        private void RefreshCameraList()
        {
            cbCameras.Items.Clear();
            
            // Добавляем доступные камеры
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    // Пытаемся открыть камеру
                    using (var capture = new VideoCapture(i))
                    {
                        if (capture.IsOpened())
                        {
                            cbCameras.Items.Add($"Камера {i}");
                        }
                    }
                }
                catch { }
            }
            
            if (cbCameras.Items.Count == 0)
            {
                cbCameras.Items.Add("Камеры не найдены");
            }
        }

        private void CbCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCameras.SelectedIndex >= 0)
            {
                _appSettings.CameraIndex = cbCameras.SelectedIndex;
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void CbCameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика выбора типа камеры
        }

        private void CbCameraRotation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCameraRotation.SelectedItem is ComboBoxItem selectedItem)
            {
                _appSettings.CameraRotation = int.Parse(selectedItem.Content.ToString().Replace(" градусов", ""));
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void ChkMirrorMode_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _appSettings.MirrorMode = chkMirrorMode.IsChecked == true;
            SettingsManager.SaveSettings(_appSettings);
        }

        private void BtnTestCamera_Click(object sender, RoutedEventArgs e)
        {
            if (_isPreviewActive)
            {
                StopPreview();
                return;
            }

            int selectedIndex = cbCameras.SelectedIndex;
            if (selectedIndex < 0 || cbCameras.Items[selectedIndex].ToString() == "Камеры не найдены") 
            {
                MessageBox.Show("Выберите камеру для тестирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                // Открываем камеру
                _capture = new VideoCapture(selectedIndex);
                if (!_capture.IsOpened())
                {
                    MessageBox.Show("Не удалось открыть выбранную камеру", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Отображаем область предпросмотра
                previewBorder.Visibility = Visibility.Visible;
                
                // Настраиваем таймер для обновления изображения
                _previewTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(33) // ~30 fps
                };
                
                // Режим поворота
                string rotationMode = ((ComboBoxItem)cbCameraRotation.SelectedItem).Content.ToString();
                
                // Зеркальное отображение
                bool mirrorMode = chkMirrorMode.IsChecked == true;
                
                _previewTimer.Tick += (s, args) =>
                {
                    using (var frame = new Mat())
                    {
                        if (_capture.Read(frame))
                        {
                            // Применяем поворот
                            RotateFlags? rotateFlags = null;
                            switch (rotationMode)
                            {
                                case "90° вправо (вертикально)":
                                    rotateFlags = RotateFlags.Rotate90Clockwise;
                                    break;
                                case "90° влево (вертикально)":
                                    rotateFlags = RotateFlags.Rotate90Counterclockwise;
                                    break;
                                case "180°":
                                    rotateFlags = RotateFlags.Rotate180;
                                    break;
                            }
                            
                            if (rotateFlags.HasValue)
                            {
                                Cv2.Rotate(frame, frame, rotateFlags.Value);
                            }
                            
                            // Применяем зеркальное отображение
                            if (mirrorMode)
                            {
                                Cv2.Flip(frame, frame, FlipMode.Y);
                            }
                            
                            // Отображаем кадр
                            imgPreview.Source = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource(frame);
                        }
                    }
                };
                
                _previewTimer.Start();
                _isPreviewActive = true;
                btnTestCamera.Content = "Остановить предпросмотр";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при тестировании камеры: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClosePreview_Click(object sender, RoutedEventArgs e)
        {
            StopPreview();
        }
        
        private void StopPreview()
        {
            if (_previewTimer != null)
            {
                _previewTimer.Stop();
                _previewTimer = null;
            }
            
            if (_capture != null)
            {
                _capture.Dispose();
                _capture = null;
            }
            
            previewBorder.Visibility = Visibility.Collapsed;
            _isPreviewActive = false;
            btnTestCamera.Content = "Проверить камеру";
        }
        
        public void Cleanup()
        {
            StopPreview();
        }
    }
}