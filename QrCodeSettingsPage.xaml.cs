using System;
using System.Windows;
using System.Windows.Controls;
using UnifiedPhotoBooth;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для QrCodeSettingsPage.xaml
    /// </summary>
    public partial class QrCodeSettingsPage : Page
    {
        private AppSettings _appSettings;

        public QrCodeSettingsPage(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            
            // Инициализация UI элементов
            InitializeUI();
        }

        private void InitializeUI()
        {
            try
            {
                // Инициализация элементов настройки QR-кода
                sliderQrSize.Value = _appSettings.QrCodeSize;
                txtQrSize.Text = _appSettings.QrCodeSize.ToString();
                
                rectQrBackColor.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_appSettings.QrBackgroundColor));
                rectQrForeColor.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_appSettings.QrForegroundColor));
                
                txtQrLogoPath.Text = _appSettings.QrLogoPath;
                
                sliderQrLogoSize.Value = _appSettings.QrLogoSize;
                txtQrLogoSize.Text = $"{_appSettings.QrLogoSize}%";
                
                // Обновляем предпросмотр QR-кода
                // Делаем это через диспетчер, чтобы интерфейс успел инициализироваться
                Dispatcher.BeginInvoke(new Action(() => {
                    try {
                        UpdateQrPreview();
                    } catch {
                        // Игнорируем ошибки при предпросмотре при инициализации
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации страницы настроек QR-кода: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SliderQrSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (txtQrSize != null) // Проверяем, инициализирован ли элемент UI
                {
                    int size = (int)sliderQrSize.Value;
                    txtQrSize.Text = size.ToString();
                    
                    _appSettings.QrCodeSize = size;
                    SettingsManager.SaveSettings(_appSettings);
                    
                    // Обновляем предпросмотр с задержкой, чтобы не нагружать систему при перемещении слайдера
                    Dispatcher.BeginInvoke(new Action(() => {
                        try {
                            UpdateQrPreview();
                        } catch {
                            // При движении слайдера не показываем ошибки
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
            catch
            {
                // При движении слайдера не показываем ошибки
            }
        }

        private void BtnQrBackColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.Color = System.Drawing.ColorTranslator.FromHtml(_appSettings.QrBackgroundColor);
            
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Преобразуем цвет в строку формата #RRGGBB
                string colorHex = $"#{colorDialog.Color.R:X2}{colorDialog.Color.G:X2}{colorDialog.Color.B:X2}";
                _appSettings.QrBackgroundColor = colorHex;
                
                // Обновляем интерфейс
                rectQrBackColor.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex));
                
                // Сохраняем настройки
                SettingsManager.SaveSettings(_appSettings);
                
                // Обновляем предпросмотр
                try {
                    UpdateQrPreview();
                } catch (Exception ex) {
                    MessageBox.Show($"Ошибка при обновлении предпросмотра: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnQrForeColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.Color = System.Drawing.ColorTranslator.FromHtml(_appSettings.QrForegroundColor);
            
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Преобразуем цвет в строку формата #RRGGBB
                string colorHex = $"#{colorDialog.Color.R:X2}{colorDialog.Color.G:X2}{colorDialog.Color.B:X2}";
                _appSettings.QrForegroundColor = colorHex;
                
                // Обновляем интерфейс
                rectQrForeColor.Fill = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorHex));
                
                // Сохраняем настройки
                SettingsManager.SaveSettings(_appSettings);
                
                // Обновляем предпросмотр
                try {
                    UpdateQrPreview();
                } catch (Exception ex) {
                    MessageBox.Show($"Ошибка при обновлении предпросмотра: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnLoadQrLogo_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            dlg.Title = "Выберите логотип для QR-кода";
            
            if (dlg.ShowDialog() == true)
            {
                _appSettings.QrLogoPath = dlg.FileName;
                txtQrLogoPath.Text = dlg.FileName;
                
                // Сохраняем настройки
                SettingsManager.SaveSettings(_appSettings);
                
                // Обновляем предпросмотр
                try {
                    UpdateQrPreview();
                } catch (Exception ex) {
                    MessageBox.Show($"Ошибка при обновлении предпросмотра: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearQrLogo_Click(object sender, RoutedEventArgs e)
        {
            _appSettings.QrLogoPath = "";
            txtQrLogoPath.Text = "";
            
            // Сохраняем настройки
            SettingsManager.SaveSettings(_appSettings);
            
            // Обновляем предпросмотр
            try {
                UpdateQrPreview();
            } catch (Exception ex) {
                MessageBox.Show($"Ошибка при обновлении предпросмотра: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SliderQrLogoSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (txtQrLogoSize != null) // Проверяем, инициализирован ли элемент UI
                {
                    int size = (int)sliderQrLogoSize.Value;
                    txtQrLogoSize.Text = $"{size}%";
                    
                    _appSettings.QrLogoSize = size;
                    SettingsManager.SaveSettings(_appSettings);
                    
                    // Обновляем предпросмотр с задержкой, чтобы не нагружать систему при перемещении слайдера
                    Dispatcher.BeginInvoke(new Action(() => {
                        try {
                            UpdateQrPreview();
                        } catch {
                            // При движении слайдера не показываем ошибки
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
            catch
            {
                // При движении слайдера не показываем ошибки
            }
        }

        private void UpdateQrPreview()
        {
            try
            {
                // Проверяем, что элемент imgQrPreview инициализирован
                if (imgQrPreview == null) return;
                
                // Создаем временный QR-код для предпросмотра
                string previewText = "https://example.com/preview";
                int size = (int)sliderQrSize.Value;
                int logoSize = (int)sliderQrLogoSize.Value;
                
                // Получаем QR-код с нашими настройками
                var qr = GenerateCustomQrCode(
                    previewText, 
                    size,
                    _appSettings.QrForegroundColor, 
                    _appSettings.QrBackgroundColor, 
                    _appSettings.QrLogoPath, 
                    logoSize);
                
                // Преобразуем для отображения
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    qr.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                }
                
                imgQrPreview.Source = bitmapImage;
            }
            catch
            {
                // Не показываем сообщение об ошибке, чтобы не раздражать пользователя
                // Просто игнорируем ошибку обновления предпросмотра
            }
        }

        private System.Drawing.Bitmap GenerateCustomQrCode(string content, int size, string foreColor, string backColor, string logoPath, int logoSizePercent)
        {
            // Создаем генератор QR-кода
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
            QRCode qrCode = new QRCode(qrCodeData);
            
            // Преобразуем цвета из строк в объекты цветов
            System.Drawing.Color qrForeColor = System.Drawing.ColorTranslator.FromHtml(foreColor);
            System.Drawing.Color qrBackColor = System.Drawing.ColorTranslator.FromHtml(backColor);
            
            // Создаем QR-код с нашими цветами
            System.Drawing.Bitmap qrBitmap = qrCode.GetGraphic(20, qrForeColor, qrBackColor, false);
            
            // Изменяем размер QR-кода до нужного
            System.Drawing.Bitmap resizedQrBitmap = new System.Drawing.Bitmap(size, size);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(resizedQrBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(qrBitmap, new System.Drawing.Rectangle(0, 0, size, size));
            }
            
            // Если указан путь к логотипу, добавляем его в центр QR-кода
            if (!string.IsNullOrEmpty(logoPath) && System.IO.File.Exists(logoPath))
            {
                try
                {
                    // Загружаем логотип
                    System.Drawing.Bitmap logo = new System.Drawing.Bitmap(logoPath);
                    
                    // Определяем размер логотипа в процентах от размера QR-кода
                    int logoWidth = (int)(size * logoSizePercent / 100.0);
                    int logoHeight = (int)(size * logoSizePercent / 100.0);
                    
                    // Масштабируем логотип до нужного размера
                    System.Drawing.Bitmap resizedLogo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(logoWidth, logoHeight));
                    
                    // Рассчитываем позицию для вставки логотипа в центр QR-кода
                    int logoX = (size - logoWidth) / 2;
                    int logoY = (size - logoHeight) / 2;
                    
                    // Вставляем логотип в QR-код
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(resizedQrBitmap))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(resizedLogo, new System.Drawing.Rectangle(logoX, logoY, logoWidth, logoHeight));
                    }
                    
                    // Освобождаем ресурсы
                    logo.Dispose();
                    resizedLogo.Dispose();
                }
                catch
                {
                    // Игнорируем ошибку добавления логотипа, продолжаем без него
                }
            }
            
            return resizedQrBitmap;
        }
    }
}