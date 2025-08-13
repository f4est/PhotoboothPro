using System;
using System.Windows;
using System.Windows.Controls;
using UnifiedPhotoBooth;
using System.Drawing.Printing;
using System.Printing;
using System.Drawing;
using System.Windows.Threading;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для PrinterSettingsPage.xaml
    /// </summary>
    public partial class PrinterSettingsPage : Page
    {
        private AppSettings _appSettings;
        private DNP_PrinterSettings.DNPSettings _dnpSettings;
        private bool _isDnpPrinter = false;

        public PrinterSettingsPage(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            
            // Инициализация UI элементов
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Заполняем список принтеров
            RefreshPrinterList();
            
            // Выбираем сохраненный принтер
            if (!string.IsNullOrEmpty(_appSettings.PrinterName))
            {
                foreach (string printer in cbPrinters.Items)
                {
                    if (printer == _appSettings.PrinterName)
                    {
                        cbPrinters.SelectedItem = printer;
                        break;
                    }
                }
            }
            else if (cbPrinters.Items.Count > 0)
            {
                cbPrinters.SelectedIndex = 0;
            }
            
            // Устанавливаем размеры печати
            txtPrintWidth.Text = _appSettings.PrintWidth.ToString("F2");
            txtPrintHeight.Text = _appSettings.PrintHeight.ToString("F2");
            
            // Режим обработки для печати
            int printProcessingModeIndex = (int)_appSettings.PrintProcessingMode;
            if (printProcessingModeIndex >= 0 && printProcessingModeIndex < cbPrintProcessingMode.Items.Count)
            {
                cbPrintProcessingMode.SelectedIndex = printProcessingModeIndex;
            }
            else
            {
                cbPrintProcessingMode.SelectedIndex = 0; // По умолчанию растягивание
            }

            // Инициализация настроек DNP принтера, если выбран принтер DNP
            CheckAndSetupDnpPrinter();
        }

        private void RefreshPrinterList()
        {
            cbPrinters.Items.Clear();
            
            // Получаем список принтеров из системы
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cbPrinters.Items.Add(printer);
            }
            
            if (cbPrinters.Items.Count == 0)
            {
                cbPrinters.Items.Add("Принтеры не найдены");
            }
        }

        private void CbPrinters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPrinters.SelectedIndex >= 0 && cbPrinters.SelectedItem.ToString() != "Принтеры не найдены")
            {
                _appSettings.PrinterName = cbPrinters.SelectedItem.ToString();
                SettingsManager.SaveSettings(_appSettings);

                // Проверяем, является ли выбранный принтер DNP DS-RX1
                CheckAndSetupDnpPrinter();
            }
        }

        private void CheckAndSetupDnpPrinter()
        {
            // Проверяем, является ли выбранный принтер DNP DS-RX1
            _isDnpPrinter = false;

            if (cbPrinters.SelectedItem != null && cbPrinters.SelectedItem.ToString() != "Принтеры не найдены")
            {
                string selectedPrinter = cbPrinters.SelectedItem.ToString();
                _isDnpPrinter = DNP_PrinterSettings.IsDNPPrinter(selectedPrinter);

                // Показываем или скрываем панели настроек DNP
                dnpSettingsGroup.Visibility = _isDnpPrinter ? Visibility.Visible : Visibility.Collapsed;
                dnpControlGroup.Visibility = _isDnpPrinter ? Visibility.Visible : Visibility.Collapsed;

                // Если это DNP принтер, загружаем его настройки
                if (_isDnpPrinter)
                {
                    try
                    {
                        _dnpSettings = DNP_PrinterSettings.GetCurrentDNPSettings(selectedPrinter);
                        UpdateDnpUI();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке настроек DNP принтера: {ex.Message}", "Ошибка", 
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                        _dnpSettings = new DNP_PrinterSettings.DNPSettings();
                    }
                }
            }
            else
            {
                dnpSettingsGroup.Visibility = Visibility.Collapsed;
                dnpControlGroup.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateDnpUI()
        {
            // Обновляем интерфейс в соответствии с настройками DNP
            cbMediaType.SelectedIndex = (int)_dnpSettings.MediaType;
            cbPrintType.SelectedIndex = (int)_dnpSettings.Type;
            cbPhotoSize.SelectedIndex = (int)_dnpSettings.Size;
            cbPrintQuality.SelectedIndex = (int)_dnpSettings.Quality;
            txtCopiesCount.Text = _dnpSettings.Copies.ToString();
            chkColorMatching.IsChecked = _dnpSettings.EnableColorMatching;
            chkMirrorPrint.IsChecked = _dnpSettings.MirrorPrint;
            chkMultiCut.IsChecked = _dnpSettings.MultiCut;
            
            // Обновляем слайдеры
            sliderSharpness.Value = _dnpSettings.Sharpness;
            txtSharpness.Text = _dnpSettings.Sharpness.ToString();
            sliderBrightness.Value = _dnpSettings.Brightness;
            txtBrightness.Text = _dnpSettings.Brightness.ToString();
            sliderContrast.Value = _dnpSettings.Contrast;
            txtContrast.Text = _dnpSettings.Contrast.ToString();

            // Если выбран пользовательский размер, обновляем размеры печати
            if (_dnpSettings.Size == DNP_PrinterSettings.PrintSize.Custom)
            {
                double customWidthCm = _dnpSettings.CustomWidth / 100.0; // Переводим 0.1 мм в см
                double customHeightCm = _dnpSettings.CustomHeight / 100.0;
                txtPrintWidth.Text = customWidthCm.ToString("F2");
                txtPrintHeight.Text = customHeightCm.ToString("F2");
            }
        }

        private void TxtPrintWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtPrintWidth.Text, out double width))
            {
                _appSettings.PrintWidth = width;
                SettingsManager.SaveSettings(_appSettings);

                // Если это принтер DNP и выбран пользовательский размер, обновляем настройки DNP
                if (_isDnpPrinter && _dnpSettings.Size == DNP_PrinterSettings.PrintSize.Custom)
                {
                    _dnpSettings.CustomWidth = (int)(width * 100); // Преобразуем см в 0.1 мм
                }
            }
        }

        private void TxtPrintHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(txtPrintHeight.Text, out double height))
            {
                _appSettings.PrintHeight = height;
                SettingsManager.SaveSettings(_appSettings);

                // Если это принтер DNP и выбран пользовательский размер, обновляем настройки DNP
                if (_isDnpPrinter && _dnpSettings.Size == DNP_PrinterSettings.PrintSize.Custom)
                {
                    _dnpSettings.CustomHeight = (int)(height * 100); // Преобразуем см в 0.1 мм
                }
            }
        }

        private void CbPrintProcessingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPrintProcessingMode.SelectedIndex >= 0)
            {
                _appSettings.PrintProcessingMode = (ImageProcessingMode)cbPrintProcessingMode.SelectedIndex;
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void BtnTestPrint_Click(object sender, RoutedEventArgs e)
        {
            if (cbPrinters.SelectedIndex < 0 || cbPrinters.SelectedItem.ToString() == "Принтеры не найдены")
            {
                MessageBox.Show("Выберите принтер для тестовой печати", "Принтер не выбран", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                // Создаем тестовое изображение
                System.Drawing.Bitmap testImage = CreateTestPrintImage();
                
                // Создаем документ для печати
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = _appSettings.PrinterName;
                
                // Если это принтер DNP, применяем его специфичные настройки
                if (_isDnpPrinter)
                {
                    DNP_PrinterSettings.ApplyDNPSettings(pd, _dnpSettings);
                }
                
                // Устанавливаем обработчик для печати
                pd.PrintPage += (printSender, printArgs) =>
                {
                    // Получаем размеры печатного поля
                    System.Drawing.RectangleF printableArea = printArgs.MarginBounds;
                    
                    // Определяем размеры изображения в точках принтера
                    float widthInPoints = ConvertCmToPoints(_appSettings.PrintWidth);
                    float heightInPoints = ConvertCmToPoints(_appSettings.PrintHeight);
                    
                    // Центрируем изображение на странице
                    float x = (printArgs.PageBounds.Width - widthInPoints) / 2;
                    float y = (printArgs.PageBounds.Height - heightInPoints) / 2;
                    
                    System.Drawing.RectangleF destRect = new System.Drawing.RectangleF(x, y, widthInPoints, heightInPoints);
                    
                    // Выводим изображение
                    printArgs.Graphics.DrawImage(testImage, destRect);
                };
                
                // Запускаем печать
                pd.Print();
                
                // Освобождаем ресурсы
                testImage.Dispose();
                
                MessageBox.Show("Тестовая печать отправлена на принтер", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Создаем тестовое изображение для печати
        private System.Drawing.Bitmap CreateTestPrintImage()
        {
            // Создаем изображение с высоким разрешением
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1200, 1800); // 300 dpi для 10x15 см
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                // Устанавливаем высокое качество отрисовки
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                
                // Заливаем фон
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, bmp.Width, bmp.Height);
                
                // Рамка по периметру
                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black, 5), 10, 10, bmp.Width - 20, bmp.Height - 20);
                
                // Градиентный фон
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new System.Drawing.Point(0, 0),
                    new System.Drawing.Point(bmp.Width, bmp.Height),
                    System.Drawing.Color.LightBlue,
                    System.Drawing.Color.LightPink))
                {
                    g.FillRectangle(brush, 20, 20, bmp.Width - 40, bmp.Height - 40);
                }
                
                // Добавляем текст
                using (System.Drawing.Font titleFont = new System.Drawing.Font("Arial", 48, System.Drawing.FontStyle.Bold))
                {
                    string title = "Тестовая печать";
                    System.Drawing.SizeF titleSize = g.MeasureString(title, titleFont);
                    float titleX = (bmp.Width - titleSize.Width) / 2;
                    float titleY = 100;
                    
                    // Добавляем тень для текста
                    using (System.Drawing.SolidBrush shadowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0)))
                    {
                        g.DrawString(title, titleFont, shadowBrush, titleX + 3, titleY + 3);
                    }
                    
                    // Рисуем текст
                    g.DrawString(title, titleFont, System.Drawing.Brushes.White, titleX, titleY);
                }
                
                // Добавляем информацию о размерах
                using (System.Drawing.Font infoFont = new System.Drawing.Font("Arial", 28))
                {
                    string sizeInfo = $"Размер: {_appSettings.PrintWidth} x {_appSettings.PrintHeight} см";
                    System.Drawing.SizeF infoSize = g.MeasureString(sizeInfo, infoFont);
                    float infoX = (bmp.Width - infoSize.Width) / 2;
                    float infoY = 250;
                    g.DrawString(sizeInfo, infoFont, System.Drawing.Brushes.Black, infoX, infoY);
                }
                
                // Добавляем цветные квадраты для проверки печати
                int squareSize = 150;
                int startY = 400;
                
                g.FillRectangle(System.Drawing.Brushes.Red, (bmp.Width / 5) - (squareSize / 2), startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Green, (bmp.Width * 2 / 5) - (squareSize / 2), startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Blue, (bmp.Width * 3 / 5) - (squareSize / 2), startY, squareSize, squareSize);
                g.FillRectangle(System.Drawing.Brushes.Yellow, (bmp.Width * 4 / 5) - (squareSize / 2), startY, squareSize, squareSize);
                
                // Добавляем текущую дату
                using (System.Drawing.Font dateFont = new System.Drawing.Font("Arial", 24))
                {
                    string dateInfo = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                    System.Drawing.SizeF dateSize = g.MeasureString(dateInfo, dateFont);
                    float dateX = (bmp.Width - dateSize.Width) / 2;
                    float dateY = bmp.Height - 100;
                    g.DrawString(dateInfo, dateFont, System.Drawing.Brushes.Black, dateX, dateY);
                }
            }
            
            return bmp;
        }
        
        // Конвертация сантиметров в точки принтера (1 см = ~28.3465 точек)
        private float ConvertCmToPoints(double cm)
        {
            return (float)(cm * 28.3465);
        }

        // Обработчики для настроек DNP принтера
        private void CbMediaType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isDnpPrinter && cbMediaType.SelectedIndex >= 0)
            {
                _dnpSettings.MediaType = (DNP_PrinterSettings.PrintMediaType)cbMediaType.SelectedIndex;
            }
        }

        private void CbPrintType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isDnpPrinter && cbPrintType.SelectedIndex >= 0)
            {
                _dnpSettings.Type = (DNP_PrinterSettings.PrintType)cbPrintType.SelectedIndex;
            }
        }

        private void CbPhotoSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isDnpPrinter && cbPhotoSize.SelectedIndex >= 0)
            {
                _dnpSettings.Size = (DNP_PrinterSettings.PrintSize)cbPhotoSize.SelectedIndex;
                
                // Обновляем размеры печати в зависимости от выбранного формата
                switch (_dnpSettings.Size)
                {
                    case DNP_PrinterSettings.PrintSize.Postcard:
                        txtPrintWidth.Text = "10.16";  // 10.16 см (4 дюйма)
                        txtPrintHeight.Text = "15.24"; // 15.24 см (6 дюймов)
                        break;
                    case DNP_PrinterSettings.PrintSize.LSize:
                        txtPrintWidth.Text = "8.90";   // 8.90 см (3.5 дюйма)
                        txtPrintHeight.Text = "12.70"; // 12.70 см (5 дюймов)
                        break;
                    case DNP_PrinterSettings.PrintSize.Custom:
                        // Оставляем текущие значения
                        break;
                }
            }
        }

        private void CbPrintQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isDnpPrinter && cbPrintQuality.SelectedIndex >= 0)
            {
                _dnpSettings.Quality = (DNP_PrinterSettings.PrintQuality)cbPrintQuality.SelectedIndex;
            }
        }

        private void BtnCopiesMinus_Click(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter && _dnpSettings.Copies > 1)
            {
                _dnpSettings.Copies--;
                txtCopiesCount.Text = _dnpSettings.Copies.ToString();
            }
        }

        private void BtnCopiesPlus_Click(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter && _dnpSettings.Copies < 10)
            {
                _dnpSettings.Copies++;
                txtCopiesCount.Text = _dnpSettings.Copies.ToString();
            }
        }

        private void ChkColorMatching_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.EnableColorMatching = chkColorMatching.IsChecked == true;
            }
        }

        private void ChkMirrorPrint_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.MirrorPrint = chkMirrorPrint.IsChecked == true;
            }
        }

        private void ChkMultiCut_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.MultiCut = chkMultiCut.IsChecked == true;
            }
        }

        private void SliderSharpness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.Sharpness = (int)sliderSharpness.Value;
                txtSharpness.Text = _dnpSettings.Sharpness.ToString();
            }
        }

        private void SliderBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.Brightness = (int)sliderBrightness.Value;
                txtBrightness.Text = _dnpSettings.Brightness.ToString();
            }
        }

        private void SliderContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDnpPrinter)
            {
                _dnpSettings.Contrast = (int)sliderContrast.Value;
                txtContrast.Text = _dnpSettings.Contrast.ToString();
            }
        }

        private void BtnResetDnpSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_isDnpPrinter)
            {
                // Сбрасываем настройки DNP принтера к значениям по умолчанию
                _dnpSettings = new DNP_PrinterSettings.DNPSettings();
                UpdateDnpUI();
                
                MessageBox.Show("Настройки принтера DNP сброшены к значениям по умолчанию", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDnpTestPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDnpPrinter)
            {
                MessageBox.Show("Эта функция доступна только для принтеров DNP DS-RX1", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                // Создаем тестовое изображение для принтера DNP
                System.Drawing.Bitmap testImage = DNP_PrinterSettings.CreateDNPTestPage();
                
                // Создаем документ для печати
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = _appSettings.PrinterName;
                
                // Применяем настройки DNP
                DNP_PrinterSettings.ApplyDNPSettings(pd, _dnpSettings);
                
                // Устанавливаем обработчик для печати
                pd.PrintPage += (printSender, printArgs) =>
                {
                    // Выводим изображение
                    printArgs.Graphics.DrawImage(testImage, 0, 0);
                };
                
                // Запускаем печать
                pd.Print();
                
                // Освобождаем ресурсы
                testImage.Dispose();
                
                MessageBox.Show("Специальная тестовая печать DNP отправлена на принтер", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnShowDnpDialog_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDnpPrinter)
            {
                MessageBox.Show("Эта функция доступна только для принтеров DNP DS-RX1", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            try
            {
                // Создаем документ для печати
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = _appSettings.PrinterName;
                
                // Показываем стандартное диалоговое окно настройки печати
                System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();
                printDialog.Document = pd;
                printDialog.AllowSomePages = true;
                
                // Показываем окно настроек
                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Настройки принтера DNP обновлены", "Информация", 
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // После закрытия диалога обновляем настройки
                    _dnpSettings = DNP_PrinterSettings.GetCurrentDNPSettings(_appSettings.PrinterName);
                    UpdateDnpUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии диалога настроек принтера: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}