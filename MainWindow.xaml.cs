using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace UnifiedPhotoBooth
{
    public partial class MainWindow : System.Windows.Window
    {
        private GoogleDriveService _driveService;
        private Dictionary<string, string> _eventFolders;
        private WindowState _previousWindowState;
        private double _windowNormalWidth;
        private double _windowNormalHeight;
        private double _windowNormalLeft;
        private double _windowNormalTop;
        
        private AppSettings _appSettings;
        private string _currentEventId;
        private string _currentEventName;
        private bool _isInAppMode;
        private int _cornerClickCount = 0;
        private DispatcherTimer _cornerClickTimer;
        
        public MainWindow()
        {
            InitializeComponent();
            _driveService = new GoogleDriveService();
            _eventFolders = new Dictionary<string, string>();
            _previousWindowState = WindowState;
            
            // Загружаем настройки
            _appSettings = SettingsManager.LoadSettings();
            _cornerClickTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _cornerClickTimer.Tick += (s, ev) => { _cornerClickCount = 0; _cornerClickTimer.Stop(); };
            
            // По умолчанию показываем страницу управления событиями
            ShowSettingsPage("Events");
        }
        
        // Заглушка - события теперь управляются через EventsSettingsPage
        private void RefreshEvents()
        {
            // Больше не используется
        }
        
        // Заглушка - события теперь управляются через EventsSettingsPage
        private void cbEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Больше не используется
        }
        
        // Заглушки - эти кнопки больше не используются в новом дизайне
        private void BtnPhotoMode_Click(object sender, RoutedEventArgs e)
        {
            // Больше не используется
        }
        
        private void BtnVideoMode_Click(object sender, RoutedEventArgs e)
        {
            // Больше не используется
        }
        
        // Заглушка - создание событий теперь в EventsSettingsPage
        private void BtnNewEvent_Click(object sender, RoutedEventArgs e)
        {
            // Больше не используется
        }
        
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        
        private void BtnGallery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(new GalleryPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии галереи: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void BtnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            ToggleFullscreen();
        }
        
        private void ToggleFullscreen()
        {
            if (WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                ExitFullscreenMode();
            }
            else
            {
                EnterFullscreenMode();
            }
            
            UpdateFullscreenButtonIcon();
        }
        
        private void EnterFullscreenMode()
        {
            // Сохраняем текущее состояние окна
            _previousWindowState = WindowState;
            _windowNormalWidth = Width;
            _windowNormalHeight = Height;
            _windowNormalLeft = Left;
            _windowNormalTop = Top;

            // Скрываем панель задач
            var handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var taskbar = FindWindow("Shell_TrayWnd", null);
            ShowWindow(taskbar, 0); // SW_HIDE

            // Устанавливаем полноэкранный режим
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Topmost = true;
        }
        
        private void ExitFullscreenMode()
        {
            // Показываем панель задач
            var taskbar = FindWindow("Shell_TrayWnd", null);
            ShowWindow(taskbar, 1); // SW_SHOW

            // Восстанавливаем предыдущее состояние
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = _previousWindowState;
            Width = _windowNormalWidth;
            Height = _windowNormalHeight;
            Left = _windowNormalLeft;
            Top = _windowNormalTop;
            Topmost = false;
        }
        
        private void UpdateFullscreenButtonIcon()
        {
            // Обновляем иконку кнопки полноэкранного режима
            if (WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                // В полноэкранном режиме показываем иконку выхода из него
                // btnFullscreen.Content = "⮽"; // Удален
                // btnFullscreen.ToolTip = "Выйти из полноэкранного режима (F11)"; // Удален
            }
            else
            {
                // В обычном режиме показываем иконку входа в полноэкранный режим
                // btnFullscreen.Content = "⛶"; // Удален
                // btnFullscreen.ToolTip = "Полноэкранный режим (F11)"; // Удален
            }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            // Обработка нажатия клавиши F11 для переключения полноэкранного режима
            if (e.Key == Key.F11)
            {
                ToggleFullscreen();
                e.Handled = true;
            }
            
            // Обработка нажатия клавиши Escape для выхода из полноэкранного режима
            if (e.Key == Key.Escape && WindowState == WindowState.Maximized && WindowStyle == WindowStyle.None)
            {
                ExitFullscreenMode();
                UpdateFullscreenButtonIcon();
                e.Handled = true;
            }
        }

        private void BtnSettingsSection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ShowSettingsPage(button.Tag.ToString());
                HighlightActiveSection(button);
            }
        }

        private void BtnStartApp_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AppStartPage(_driveService, _currentEventId, _currentEventName, ExitAppMode));
            _isInAppMode = true;
            sidePanel.Visibility = Visibility.Collapsed;
            CornerClickArea.Visibility = Visibility.Visible;
        }

        private void CornerClickArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _cornerClickCount++;
            _cornerClickTimer.Stop();
            _cornerClickTimer.Start();

            if (_cornerClickCount >= 5)
            {
                ExitAppMode();
                _cornerClickCount = 0;
                _cornerClickTimer.Stop();
            }
        }

        private void ShowSettingsPage(string tag)
        {
            Page page = tag switch
            {
                "Events" => new EventsSettingsPage(_driveService, OnEventSelected),
                "Camera" => new CameraSettingsPage(_appSettings),
                "PhotoBooth" => new PhotoBoothSettingsPage(_appSettings),
                "VideoBooth" => new VideoBoothSettingsPage(_appSettings),
                "Printer" => new PrinterSettingsPage(_appSettings),
                "QrCode" => new QrCodeSettingsPage(_appSettings),
                _ => null
            };

            if (page != null)
            {
                MainFrame.Navigate(page);
            }
        }

        private void HighlightActiveSection(Button activeButton)
        {
            var buttons = new[] { btnEventsSection, btnCameraSection, btnPhotoBoothSection, btnVideoBoothSection, btnPrinterSection, btnQrCodeSection };
            foreach (var button in buttons)
            {
                button.Style = (Style)FindResource("MenuButtonStyle");
            }
            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void ExitAppMode()
        {
            _isInAppMode = false;
            sidePanel.Visibility = Visibility.Visible;
            CornerClickArea.Visibility = Visibility.Collapsed;
            MainFrame.Navigate(new AppStartPage(_driveService, _currentEventId, _currentEventName, ExitAppMode)); // Или возвращение к настройкам
        }

        private void OnEventSelected(string eventId, string eventName)
        {
            _currentEventId = eventId;
            _currentEventName = eventName;
        }

        private void BtnOpenPositionSetup_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог настройки позиций из SettingsWindow (существующая реализация)
            try
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.Owner = this;
                // Используем публичный метод, который откроет уже реализованный интерфейс
                settingsWindow.OpenPositionSetupDialog(this);
            }
            catch
            {
                // Fallback: если публичного метода ещё нет, вызываем обработчик напрямую
                try
                {
                    var settingsWindow = new SettingsWindow();
                    var mi = typeof(SettingsWindow).GetMethod("BtnSetupPositions_Click", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(settingsWindow, new object[] { null, new RoutedEventArgs() });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть редактор позиций: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
} 