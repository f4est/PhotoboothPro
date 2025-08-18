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
using System.Windows.Media.Effects;

namespace UnifiedPhotoBooth
{
    public partial class GalleryPage : Page
    {
        private const string PhotosDir = "photos";
        private const string RecordingsDir = "recordings";
        private const int ThumbnailSize = 260;
        private string _eventFolderId;
        
        public GalleryPage(string eventFolderId = null)
        {
            InitializeComponent();
            _eventFolderId = eventFolderId;
            Loaded += GalleryPage_Loaded;
        }
        
        private void GalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGalleryItems();
        }
        
        // Публичный метод для обновления галереи извне
        public void RefreshGallery()
        {
            LoadGalleryItems();
        }
        
        private void LoadGalleryItems()
        {
            try
            {
                galleryPanel.Children.Clear();
                
                // Собираем все медиафайлы в один список
                var allMediaFiles = new List<MediaFileInfo>();
                
                // Добавляем фотографии
                if (Directory.Exists(PhotosDir))
                {
                    var photoFiles = Directory.GetFiles(PhotosDir, "*.jpg")
                        .Select(f => new FileInfo(f))
                        .Select(f => new MediaFileInfo
                        {
                            FilePath = f.FullName,
                            FileName = f.Name,
                            CreationTime = f.CreationTime,
                            Type = "Фото",
                            Extension = ".jpg"
                        });
                    allMediaFiles.AddRange(photoFiles);
                }
                
                // Добавляем видео
                if (Directory.Exists(RecordingsDir))
                {
                    var videoFiles = Directory.GetFiles(RecordingsDir, "*.mp4")
                        .Concat(Directory.GetFiles(RecordingsDir, "*.avi"))
                        .Select(f => new FileInfo(f))
                        .Select(f => new MediaFileInfo
                        {
                            FilePath = f.FullName,
                            FileName = f.Name,
                            CreationTime = f.CreationTime,
                            Type = "Видео",
                            Extension = Path.GetExtension(f.Name).ToLower()
                        });
                    allMediaFiles.AddRange(videoFiles);
                }
                
                // Сортируем все файлы по дате создания (сначала новые)
                var sortedFiles = allMediaFiles.OrderByDescending(f => f.CreationTime).ToList();
                
                // Создаем элементы галереи для каждого файла
                foreach (var mediaFile in sortedFiles)
                {
                    CreateGalleryItem(mediaFile);
                }
                
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
        
        private class MediaFileInfo
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public DateTime CreationTime { get; set; }
            public string Type { get; set; }
            public string Extension { get; set; }
        }
        
        private void CreateGalleryItem(MediaFileInfo mediaFile)
        {
            // Проверяем, существует ли связанный QR-код
            string qrFilePath = Path.Combine(
                Path.GetDirectoryName(mediaFile.FilePath), 
                Path.GetFileNameWithoutExtension(mediaFile.FileName) + "_qr.png");
            
            bool hasQrCode = File.Exists(qrFilePath);
            
            // Создаем элемент галереи (карточка)
            Border card = new Border
            {
                Width = ThumbnailSize,
                Height = ThumbnailSize + 60,
                Margin = new Thickness(12),
                Background = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
                CornerRadius = new CornerRadius(10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                BorderThickness = new Thickness(1),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 2,
                    Opacity = 0.3
                }
            };
            
            Grid cardGrid = new Grid();
            card.Child = cardGrid;
            
            // Верх: миниатюра
            Border thumbnailBorder = new Border
            {
                Width = ThumbnailSize,
                Height = ThumbnailSize,
                CornerRadius = new CornerRadius(10, 10, 0, 0),
                ClipToBounds = true
            };
            
            // Изображение миниатюры
            Image thumbnailImage = new Image
            {
                Stretch = Stretch.UniformToFill
            };
            
            // Загружаем миниатюру
            if (mediaFile.Type == "Фото")
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(mediaFile.FilePath);
                    bitmap.DecodePixelWidth = ThumbnailSize;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    thumbnailImage.Source = bitmap;
                }
                catch
                {
                    // В случае ошибки загрузки изображения используем заглушку
                    CreatePlaceholderImage(thumbnailImage, "ФОТО");
                }
            }
            else if (mediaFile.Type == "Видео")
            {
                try
                {
                    // Для видео используем значок видео
                    thumbnailImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/video_icon.png", UriKind.Absolute));
                }
                catch
                {
                    // В случае ошибки загрузки иконки видео используем заглушку
                    CreatePlaceholderImage(thumbnailImage, "ВИДЕО");
                }
            }
            
            thumbnailBorder.Child = thumbnailImage;
            
            // Низ: подпись
            Border captionBar = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)),
                Height = 60,
                VerticalAlignment = VerticalAlignment.Bottom,
                CornerRadius = new CornerRadius(0, 0, 10, 10)
            };
                
                StackPanel captionPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(10, 6, 10, 6)
                };
                
                TextBlock titleText = new TextBlock
                {
                    Text = mediaFile.Type,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 14
                };
                
                TextBlock dateText = new TextBlock
                {
                    Text = mediaFile.CreationTime.ToString("dd.MM.yyyy HH:mm"),
                    Foreground = Brushes.Gray,
                    FontSize = 12
                };
                
                captionPanel.Children.Add(titleText);
                captionPanel.Children.Add(dateText);
                captionBar.Child = captionPanel;
                
                // Слои в карточке
                cardGrid.Children.Add(thumbnailBorder);
                cardGrid.Children.Add(captionBar);
                
                // Создаем объект с информацией о медиафайле и QR-коде
                var mediaInfo = new MediaInfo
                {
                    FilePath = mediaFile.FilePath,
                    QrCodePath = hasQrCode ? qrFilePath : null
                };
                
                // Обработчик нажатия
                card.Tag = mediaInfo;
                card.MouseLeftButtonDown += GalleryItem_Click;
                card.Cursor = System.Windows.Input.Cursors.Hand;
                
                // Добавляем элемент в галерею
                galleryPanel.Children.Add(card);
        }
        
        private class MediaInfo
        {
            public string FilePath { get; set; }
            public string QrCodePath { get; set; }
        }
        
        private void GalleryItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is MediaInfo mediaInfo)
            {
                string filePath = mediaInfo.FilePath;
                string qrCodePath = mediaInfo.QrCodePath;
                
                // Определяем тип файла
                bool isVideo = filePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) || 
                              filePath.EndsWith(".avi", StringComparison.OrdinalIgnoreCase);
                
                // Открываем страницу с результатом
                if (isVideo)
                {
                    // Для видео
                    NavigationService.Navigate(new VideoResultPage(filePath, qrCodePath, _eventFolderId));
                }
                else
                {
                    // Для фото
                    NavigationService.Navigate(new PhotoResultPage(filePath, qrCodePath, _eventFolderId));
                }
            }
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на главную страницу
            NavigationService.GoBack();
        }
        
        private void CreatePlaceholderImage(Image image, string text)
        {
            try
            {
                // Создаем заглушку программно
                var renderTarget = new RenderTargetBitmap(ThumbnailSize, ThumbnailSize, 96, 96, PixelFormats.Pbgra32);
                var visual = new DrawingVisual();
                
                using (var context = visual.RenderOpen())
                {
                    // Фон
                    context.DrawRectangle(new SolidColorBrush(Color.FromRgb(64, 64, 64)), null, 
                        new Rect(0, 0, ThumbnailSize, ThumbnailSize));
                    
                    // Текст
                    var textBlock = new FormattedText(text, 
                        System.Globalization.CultureInfo.CurrentCulture, 
                        FlowDirection.LeftToRight, 
                        new Typeface("Arial"), 24, Brushes.White, VisualTreeHelper.GetDpi(visual).PixelsPerDip);
                    
                    var textRect = new Rect((ThumbnailSize - textBlock.Width) / 2, 
                        (ThumbnailSize - textBlock.Height) / 2, 
                        textBlock.Width, textBlock.Height);
                    
                    context.DrawText(textBlock, new Point(textRect.X, textRect.Y));
                }
                
                renderTarget.Render(visual);
                image.Source = renderTarget;
            }
            catch
            {
                // Если не удалось создать заглушку программно, используем простой цветной прямоугольник
                var bitmap = new WriteableBitmap(ThumbnailSize, ThumbnailSize, 96, 96, PixelFormats.Bgr32, null);
                image.Source = bitmap;
            }
        }
    }
    
    // Вспомогательный класс для отображения результата фото
    public class PhotoResultPage : Page
    {
        private string _imagePath;
        private string _qrCodePath;
        private string _eventFolderId;
        
        public PhotoResultPage(string imagePath, string qrCodePath = null, string eventFolderId = null)
        {
            _imagePath = imagePath;
            _qrCodePath = qrCodePath;
            _eventFolderId = eventFolderId;
            
            Grid grid = new Grid();
            
            // Изображение (загружаем полностью в память, чтобы не держать файл заблокированным)
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(imagePath, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            Image image = new Image
            {
                Source = bmp,
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
                Content = "Поделиться",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Padding = new Thickness(10, 5, 10, 5),
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
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
        
        private async void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Показываем индикатор загрузки
                ((Button)sender).IsEnabled = false;
                ((Button)sender).Content = "Загрузка...";
                
                if (!string.IsNullOrEmpty(_qrCodePath) && File.Exists(_qrCodePath))
                {
                    // Показываем страницу с QR-кодом, используя сохраненный файл QR-кода
                    NavigationService.Navigate(new QRCodePage(_qrCodePath, false, true));
                }
                else
                {
                    // Создаем новый QR-код и загружаем файл
                    var driveService = new GoogleDriveService();
                    string fileName = Path.GetFileName(_imagePath);
                    string folderName = $"PhotoBooth_{DateTime.Now:yyyyMMdd_HHmmss}";

                    // Открываем файл для копирования безопасно (исключая удержание блокировки)
                    string tempCopy = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + System.IO.Path.GetExtension(_imagePath));
                    try
                    {
                        using (var fs = new FileStream(_imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var outFs = new FileStream(tempCopy, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            await fs.CopyToAsync(outFs);
                        }

                        var result = await driveService.UploadPhotoAsync(tempCopy, folderName, _eventFolderId);
                        
                        if (result != null && result.QrCode != null)
                        {
                            // Сохраняем QR-код локально
                            string qrFilePath = Path.Combine(
                                Path.GetDirectoryName(_imagePath),
                                Path.GetFileNameWithoutExtension(fileName) + "_qr.png");
                            
                            result.QrCode.Save(qrFilePath, System.Drawing.Imaging.ImageFormat.Png);
                            
                            // Показываем QR-код
                            NavigationService.Navigate(new QRCodePage(qrFilePath, false, true));
                        }
                        else
                        {
                            MessageBox.Show("Не удалось создать QR-код для этого файла.", "Ошибка", 
                                           MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    finally
                    {
                        try { System.IO.File.Delete(tempCopy); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании QR-кода: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Восстанавливаем кнопку
                ((Button)sender).IsEnabled = true;
                ((Button)sender).Content = "Поделиться";
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
        private string _eventFolderId;
        private MediaElement _mediaElement;
        
        public VideoResultPage(string videoPath, string qrCodePath = null, string eventFolderId = null)
        {
            _videoPath = videoPath;
            _qrCodePath = qrCodePath;
            _eventFolderId = eventFolderId;
            
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
        
        private async void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Показываем индикатор загрузки
                ((Button)sender).IsEnabled = false;
                ((Button)sender).Content = "Загрузка...";
                
                if (!string.IsNullOrEmpty(_qrCodePath) && File.Exists(_qrCodePath))
                {
                    // Показываем страницу с QR-кодом, используя сохраненный файл QR-кода
                    NavigationService.Navigate(new QRCodePage(_qrCodePath, true, true));
                }
                else
                {
                    // Создаем новый QR-код и загружаем файл
                    var driveService = new GoogleDriveService();
                    string fileName = Path.GetFileName(_videoPath);
                    string folderName = $"VideoBooth_{DateTime.Now:yyyyMMdd_HHmmss}";

                    // Останавливаем воспроизведение, чтобы снять возможные блокировки файла
                    try { _mediaElement?.Stop(); } catch { }

                    // Копируем видео во временный файл, чтобы избежать блокировок
                    string tempCopy = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + System.IO.Path.GetExtension(_videoPath));
                    try
                    {
                        using (var fs = new FileStream(_videoPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var outFs = new FileStream(tempCopy, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            await fs.CopyToAsync(outFs);
                        }

                        var result = await driveService.UploadVideoAsync(tempCopy, folderName, _eventFolderId);
                        
                        if (result != null && result.QrCode != null)
                        {
                            // Сохраняем QR-код локально
                            string qrFilePath = Path.Combine(
                                Path.GetDirectoryName(_videoPath),
                                Path.GetFileNameWithoutExtension(fileName) + "_qr.png");
                            
                            result.QrCode.Save(qrFilePath, System.Drawing.Imaging.ImageFormat.Png);
                            
                            // Показываем QR-код
                            NavigationService.Navigate(new QRCodePage(qrFilePath, true, true));
                        }
                        else
                        {
                            MessageBox.Show("Не удалось создать QR-код для этого файла.", "Ошибка", 
                                           MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    finally
                    {
                        try { System.IO.File.Delete(tempCopy); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании QR-кода: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Восстанавливаем кнопку
                ((Button)sender).IsEnabled = true;
                ((Button)sender).Content = "Поделиться QR";
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