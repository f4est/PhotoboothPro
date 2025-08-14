using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Threading.Tasks;
using System.Printing;
using System.Windows.Documents;

namespace UnifiedPhotoBooth
{
    public partial class GalleryPage : Page
    {
        private const string PhotosDir = "photos";
        private const string RecordingsDir = "recordings";
        private const int ThumbnailSize = 200;
        
        public GalleryPage()
        {
            InitializeComponent();
            Loaded += GalleryPage_Loaded;
        }
        
        private void GalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGalleryItems();
        }
        
        private void LoadGalleryItems()
        {
            try
            {
                galleryPanel.Children.Clear();
                
                // Загружаем фотографии
                LoadMediaFiles(PhotosDir, ".jpg", "Фото");
                
                // Загружаем видео
                LoadMediaFiles(RecordingsDir, ".mp4", "Видео");
                
                // Если нет элементов, показываем сообщение
                if (galleryPanel.Children.Count == 0)
                {
                    TextBlock noItemsText = new TextBlock
                    {
                        Text = "Нет доступных фотографий или видео",
                        FontSize = 18,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20)
                    };
                    galleryPanel.Children.Add(noItemsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке галереи: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadMediaFiles(string directory, string extension, string typeLabel)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }
            
            // Получаем файлы и сортируем их по дате создания (сначала новые)
            var files = Directory.GetFiles(directory, $"*{extension}")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .ToList();
            
            foreach (var file in files)
            {
                // Проверяем, существует ли связанный QR-код
                string qrFilePath = Path.Combine(
                    file.DirectoryName, 
                    Path.GetFileNameWithoutExtension(file.Name) + "_qr.png");
                
                bool hasQrCode = File.Exists(qrFilePath);
                
                // Создаем элемент галереи
                Grid itemGrid = new Grid
                {
                    Width = ThumbnailSize,
                    Height = ThumbnailSize + 50, // Дополнительное пространство для метки
                    Margin = new Thickness(10)
                };
                
                // Контейнер для миниатюры
                Border thumbnailBorder = new Border
                {
                    Width = ThumbnailSize,
                    Height = ThumbnailSize,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Top
                };
                
                // Изображение миниатюры
                Image thumbnailImage = new Image
                {
                    Stretch = Stretch.UniformToFill
                };
                
                // Загружаем миниатюру
                if (extension == ".jpg")
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(file.FullName);
                        bitmap.DecodePixelWidth = ThumbnailSize;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        thumbnailImage.Source = bitmap;
                    }
                    catch
                    {
                        // В случае ошибки загрузки изображения используем заглушку
                        thumbnailImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/no_image.png", UriKind.Absolute));
                    }
                }
                else
                {
                    // Для видео используем значок видео
                    thumbnailImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/video_icon.png", UriKind.Absolute));
                }
                
                thumbnailBorder.Child = thumbnailImage;
                
                // Метка с типом и датой
                TextBlock infoText = new TextBlock
                {
                    Text = $"{typeLabel} - {file.CreationTime.ToString("dd.MM.yyyy HH:mm")}",
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, ThumbnailSize + 5, 0, 0)
                };
                
                // Добавляем элементы в сетку
                itemGrid.Children.Add(thumbnailBorder);
                itemGrid.Children.Add(infoText);
                
                // Создаем объект с информацией о медиафайле и QR-коде
                var mediaInfo = new MediaInfo
                {
                    FilePath = file.FullName,
                    QrCodePath = hasQrCode ? qrFilePath : null
                };
                
                // Обработчик нажатия
                itemGrid.Tag = mediaInfo;
                itemGrid.MouseLeftButtonDown += GalleryItem_Click;
                itemGrid.Cursor = System.Windows.Input.Cursors.Hand;
                
                // Добавляем элемент в галерею
                galleryPanel.Children.Add(itemGrid);
            }
        }
        
        private class MediaInfo
        {
            public string FilePath { get; set; }
            public string QrCodePath { get; set; }
        }
        
        private void GalleryItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.Tag is MediaInfo mediaInfo)
            {
                string filePath = mediaInfo.FilePath;
                string qrCodePath = mediaInfo.QrCodePath;
                
                // Определяем тип файла
                bool isVideo = filePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase);
                
                // Открываем страницу с результатом
                if (isVideo)
                {
                    // Для видео
                    NavigationService.Navigate(new VideoResultPage(filePath, qrCodePath));
                }
                else
                {
                    // Для фото
                    NavigationService.Navigate(new PhotoResultPage(filePath, qrCodePath));
                }
            }
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на главную страницу
            NavigationService.GoBack();
        }
    }
    
    // Вспомогательный класс для отображения результата фото
    public class PhotoResultPage : Page
    {
        private string _imagePath;
        private string _qrCodePath;
        
        public PhotoResultPage(string imagePath, string qrCodePath = null)
        {
            _imagePath = imagePath;
            _qrCodePath = qrCodePath;
            
            Grid grid = new Grid();
            
            // Изображение
            Image image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(20)
            };
            
            // Кнопка "Назад"
            Button btnBack = new Button
            {
                Content = "Назад",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnBack.Click += BtnBack_Click;
            
            // Кнопка "Поделиться"
            Button btnShare = new Button
            {
                Content = "Поделиться QR",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5),
                Visibility = string.IsNullOrEmpty(_qrCodePath) ? Visibility.Collapsed : Visibility.Visible
            };
            btnShare.Click += BtnShare_Click;
            
            // Кнопка "Печать"
            Button btnPrint = new Button
            {
                Content = "Печать",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnPrint.Click += BtnPrint_Click;
            
            // Добавляем элементы в сетку
            grid.Children.Add(image);
            grid.Children.Add(btnBack);
            grid.Children.Add(btnShare);
            grid.Children.Add(btnPrint);
            
            // Назначаем сетку содержимым страницы
            Content = grid;
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        
        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_qrCodePath) && File.Exists(_qrCodePath))
            {
                // Показываем страницу с QR-кодом, используя сохраненный файл QR-кода
                NavigationService.Navigate(new QRCodePage(_qrCodePath, false, true));
            }
            else
            {
                MessageBox.Show("QR-код не найден для этого файла.", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Печать без диалогового окна, используя сохраненные настройки
                DirectPrint(_imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DirectPrint(string imagePath)
        {
            try
            {
                // Проверяем, выбран ли принтер в настройках
                if (string.IsNullOrEmpty(SettingsWindow.AppSettings.SelectedPrinter))
                {
                    MessageBox.Show("В настройках не выбран принтер. Пожалуйста, настройте принтер в настройках приложения.", 
                                   "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var printDialog = new PrintDialog();
                // Устанавливаем принтер из сохраненных настроек
                printDialog.PrintQueue = new PrintQueue(new PrintServer(), SettingsWindow.AppSettings.SelectedPrinter);
                
                // Загружаем изображение
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                
                // Создаем элемент Image для печати
                Image printImage = new Image();
                printImage.Source = bitmapImage;
                
                // Настраиваем режим обработки изображения
                switch (SettingsWindow.AppSettings.PrintProcessingMode)
                {
                    case ImageProcessingMode.Stretch:
                        printImage.Stretch = Stretch.Fill;
                        break;
                    case ImageProcessingMode.Crop:
                        printImage.Stretch = Stretch.UniformToFill;
                        break;
                    case ImageProcessingMode.Scale:
                        printImage.Stretch = Stretch.Uniform;
                        break;
                }
                
                // Устанавливаем размеры страницы с учётом специфичных форматов
                string paperPreset = SettingsWindow.AppSettings.PaperPreset;
                if (paperPreset == "PR 4x6")
                {
                    // Используем стандартный размер 4x6 для PR фотопечати
                    printDialog.PrintTicket.PageMediaSize = new PageMediaSize(
                        PageMediaSizeName.NorthAmerica4x6);
                        
                    // Устанавливаем дополнительные параметры для PR 4x6
                    printDialog.PrintTicket.OutputColor = OutputColor.Color;
                    printDialog.PrintTicket.InputBin = InputBin.AutoSelect;
                    // Установка печати без полей не поддерживается напрямую через PageBorderless.On
                    
                    System.Diagnostics.Debug.WriteLine("Установлен формат PR 4x6 (NorthAmerica4x6) с цветной печатью");
                }
                else if (paperPreset == "PR 3.5x5")
                {
                    // Для 3.5x5 используем точные размеры (в 1/100 дюйма)
                    printDialog.PrintTicket.PageMediaSize = new PageMediaSize(
                        3.5 * 96, // 3.5 дюйма в пикселях
                        5 * 96);  // 5 дюймов в пикселях
                    System.Diagnostics.Debug.WriteLine("Установлен формат PR 3.5x5");
                }
                else if (paperPreset == "PR 4x6 x 2")
                {
                    // Для 4x6x2 используем точные размеры
                    printDialog.PrintTicket.PageMediaSize = new PageMediaSize(
                        4 * 96,      // 4 дюйма в пикселях
                        6 * 2 * 96); // 12 дюймов в пикселях
                    System.Diagnostics.Debug.WriteLine("Установлен формат PR 4x6 x 2");
                }
                else
                {
                        // Для других форматов учитываем ориентацию
                    bool isImageLandscape = bitmapImage.PixelWidth > bitmapImage.PixelHeight;
                    bool isLandscapePage = SettingsWindow.AppSettings.PrintOrientation == "Альбом" ||
                                      (SettingsWindow.AppSettings.PrintOrientation == "Авто" && isImageLandscape);
                    
                    // Получаем размеры из настроек
                    double widthCm = SettingsWindow.AppSettings.PrintWidth;
                    double heightCm = SettingsWindow.AppSettings.PrintHeight;
                    
                    double width, height;
                    
                    if (isLandscapePage)
                    {
                        // Ландшафт: ширина > высоты
                        width = Math.Max(widthCm, heightCm) * 96;
                        height = Math.Min(widthCm, heightCm) * 96;
                    }
                    else
                    {
                        // Портрет: высота > ширины
                        width = Math.Min(widthCm, heightCm) * 96;
                        height = Math.Max(widthCm, heightCm) * 96;
                    }
                    
                    printDialog.PrintTicket.PageMediaSize = new PageMediaSize(width, height);
                    System.Diagnostics.Debug.WriteLine($"Установлен пользовательский формат: ширина={width/96.0} см, высота={height/96.0} см, ориентация={SettingsWindow.AppSettings.PrintOrientation}");
                }
                
                // Устанавливаем ориентацию страницы с учетом размеров бумаги и типа формата
                bool isLandscape = bitmapImage.PixelWidth > bitmapImage.PixelHeight;
                
                // Для форматов PR всегда используем портретную ориентацию
                if (paperPreset != null && paperPreset.StartsWith("PR "))
                {
                    // PR форматы всегда в портретной ориентации
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
                    System.Diagnostics.Debug.WriteLine("Используется портретная ориентация для PR формата");
                }
                else if (SettingsWindow.AppSettings.PrintOrientation == "Альбом")
                {
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                    System.Diagnostics.Debug.WriteLine("Используется альбомная ориентация по настройкам пользователя");
                }
                else if (SettingsWindow.AppSettings.PrintOrientation == "Портрет")
                {
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
                    System.Diagnostics.Debug.WriteLine("Используется портретная ориентация по настройкам пользователя");
                }
                else // Авто
                {
                    // Определяем ориентацию по соотношению сторон изображения
                    PageOrientation orientation = isLandscape ? PageOrientation.Landscape : PageOrientation.Portrait;
                    printDialog.PrintTicket.PageOrientation = orientation;
                    
                    // Отладочная информация для проверки применяемых настроек
                    string orientationName = orientation == PageOrientation.Landscape ? "альбомная" : "портретная";
                    System.Diagnostics.Debug.WriteLine($"Автоматически выбрана {orientationName} ориентация по размерам изображения {bitmapImage.PixelWidth}x{bitmapImage.PixelHeight}");
                }
                
                // Печать без полей при включенном флаге
                if (SettingsWindow.AppSettings.PrintStretchFull)
                {
                    try
                    {
                        printDialog.PrintTicket.PageBorderless = PageBorderless.Borderless;
                    }
                    catch { /* Может не поддерживаться драйвером */ }
                }

                // Количество копий (по умолчанию 1)
                printDialog.PrintTicket.CopyCount = 1;
                
                // Масштабируем изображение для печати
                var capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                double pageW = capabilities?.OrientedPageMediaWidth ?? printDialog.PrintableAreaWidth;
                double pageH = capabilities?.OrientedPageMediaHeight ?? printDialog.PrintableAreaHeight;
                double originX = capabilities?.PageImageableArea?.OriginWidth ?? 0;
                double originY = capabilities?.PageImageableArea?.OriginHeight ?? 0;

                if (SettingsWindow.AppSettings.PrintStretchFull)
                {
                    // Заполнить всю страницу с учётом полей
                    printImage.Stretch = Stretch.Fill;
                    printImage.Width = pageW;
                    printImage.Height = pageH;
                    printImage.Measure(new System.Windows.Size(pageW, pageH));
                    printImage.Arrange(new Rect(-originX, -originY, pageW, pageH));
                }
                else
                {
                    // В пределах printable area
                    printImage.Measure(new System.Windows.Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                    printImage.Arrange(new Rect(0, 0, printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                }
                
                // Отправляем на печать
                printDialog.PrintVisual(printImage, "Печать фотографии");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при подготовке изображения к печати: {ex.Message}", ex);
            }
        }
    }
    
    // Вспомогательный класс для отображения результата видео
    public class VideoResultPage : Page
    {
        private string _videoPath;
        private string _qrCodePath;
        private MediaElement _mediaElement;
        
        public VideoResultPage(string videoPath, string qrCodePath = null)
        {
            _videoPath = videoPath;
            _qrCodePath = qrCodePath;
            
            Grid grid = new Grid();
            
            // Видеоплеер
            _mediaElement = new MediaElement
            {
                Source = new Uri(videoPath, UriKind.Absolute),
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Stop,
                Stretch = Stretch.Uniform,
                Margin = new Thickness(20)
            };
            
            // Обработчики событий видео
            _mediaElement.Loaded += MediaElement_Loaded;
            _mediaElement.MediaEnded += MediaElement_MediaEnded;
            
            // Кнопка "Назад"
            Button btnBack = new Button
            {
                Content = "Назад",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnBack.Click += BtnBack_Click;
            
            // Кнопка "Поделиться"
            Button btnShare = new Button
            {
                Content = "Поделиться QR",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnShare.Click += BtnShare_Click;
            
            // Кнопка "Воспроизведение/Пауза"
            Button btnPlayPause = new Button
            {
                Content = "▶/⏸",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnPlayPause.Click += BtnPlayPause_Click;
            
            // Добавляем элементы в сетку
            grid.Children.Add(_mediaElement);
            grid.Children.Add(btnBack);
            grid.Children.Add(btnShare);
            grid.Children.Add(btnPlayPause);
            
            // Назначаем сетку содержимым страницы
            Content = grid;
        }
        
        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            _mediaElement.Play();
        }
        
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Перезапускаем видео с начала
            _mediaElement.Position = TimeSpan.Zero;
            _mediaElement.Play();
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем видео и возвращаемся назад
            _mediaElement.Stop();
            NavigationService.GoBack();
        }
        
        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_qrCodePath) && File.Exists(_qrCodePath))
            {
                // Показываем страницу с QR-кодом, используя сохраненный файл QR-кода
                NavigationService.Navigate(new QRCodePage(_qrCodePath, true, true));
            }
            else
            {
                MessageBox.Show("QR-код не найден для этого файла.", "Информация", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            // Переключаем воспроизведение/паузу
            if (_mediaElement.Position >= _mediaElement.NaturalDuration.TimeSpan)
            {
                _mediaElement.Position = TimeSpan.Zero;
            }
            
            if (_mediaElement.IsLoaded)
            {
                if (_mediaElement.CanPause)
                {
                    _mediaElement.Pause();
                }
                else
                {
                    _mediaElement.Play();
                }
            }
        }
    }
    
    // Страница с QR-кодом для доступа к фото/видео
    public class QRCodePage : Page
    {
        private string _mediaPath;
        private bool _isVideo;
        private bool _showSavedQR;
        
        public QRCodePage(string mediaPath, bool isVideo, bool showSavedQR = false)
        {
            _mediaPath = mediaPath;
            _isVideo = isVideo;
            _showSavedQR = showSavedQR;
            
            Grid grid = new Grid();
            
            // Заголовок
            TextBlock title = new TextBlock
            {
                Text = "Сканируйте QR-код для доступа к файлу",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 20,
                Margin = new Thickness(0, 20, 0, 20)
            };
            
            // QR-код
            Image qrImage = new Image
            {
                Width = 300,
                Height = 300,
                Margin = new Thickness(20)
            };
            
            // Если нужно показать сохраненный QR-код
            if (_showSavedQR)
            {
                try
                {
                    // Загружаем QR-код из файла
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_mediaPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    qrImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке QR-кода: {ex.Message}", "Ошибка", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Создаем новый QR-код
                GoogleDriveService driveService = new GoogleDriveService();
                string folderName = $"{(_isVideo ? "VideoBooth" : "PhotoBooth")}_{System.DateTime.Now:yyyyMMdd_HHmmss}";
                
                try
                {
                    // Создаем задачу для загрузки файла
                    Task<UploadResult> uploadTask;
                    if (_isVideo)
                    {
                        uploadTask = driveService.UploadVideoAsync(_mediaPath, folderName, null);
                    }
                    else
                    {
                        uploadTask = driveService.UploadPhotoAsync(_mediaPath, folderName, null);
                    }
                    
                    // Запускаем и ожидаем завершения
                    uploadTask.Wait();
                    
                    // Получаем результат
                    var result = uploadTask.Result;
                    
                    // Отображаем QR-код
                    if (result != null && result.QrCode != null)
                    {
                        // Конвертируем QR-код в формат для отображения
                        BitmapImage qrBitmap = new BitmapImage();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            result.QrCode.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;
                            qrBitmap.BeginInit();
                            qrBitmap.CacheOption = BitmapCacheOption.OnLoad;
                            qrBitmap.StreamSource = ms;
                            qrBitmap.EndInit();
                        }
                        
                        qrImage.Source = qrBitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании QR-кода: {ex.Message}", "Ошибка", 
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
            // Кнопка "Назад"
            Button btnBack = new Button
            {
                Content = "Назад",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(20),
                Padding = new Thickness(10, 5, 10, 5)
            };
            btnBack.Click += BtnBack_Click;
            
            // Добавляем элементы в сетку
            grid.Children.Add(title);
            
            // Создаем StackPanel для центрирования QR-кода
            StackPanel centerPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            centerPanel.Children.Add(qrImage);
            
            grid.Children.Add(centerPanel);
            grid.Children.Add(btnBack);
            
            // Назначаем сетку содержимым страницы
            Content = grid;
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 