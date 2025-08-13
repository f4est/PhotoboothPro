using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для AppStartPage.xaml
    /// </summary>
    public partial class AppStartPage : Page
    {
        private GoogleDriveService _driveService;
        private string _eventFolderId;
        private string _eventName;
        private int _cornerClickCount = 0;
        private DateTime _lastClickTime = DateTime.Now;
        private Action _exitAppCallback;

        public AppStartPage(GoogleDriveService driveService, string eventFolderId = null, string eventName = null, Action exitCallback = null)
        {
            InitializeComponent();
            _driveService = driveService;
            _eventFolderId = eventFolderId;
            _eventName = eventName;
            _exitAppCallback = exitCallback;
            
            // Отображаем название события, если оно выбрано
            if (!string.IsNullOrEmpty(eventName))
            {
                txtEventName.Text = $"Событие: {eventName}";
            }
            
            // Регистрируем обработчик нажатия клавиш
            this.Focusable = true;
            this.Focus();
            this.PreviewKeyDown += AppStartPage_PreviewKeyDown;
        }

        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открываем страницу фотобудки
                NavigationService.Navigate(new PhotoBoothPage(_driveService, _eventFolderId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии фотобудки: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGallery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открываем галерею
                NavigationService.Navigate(new GalleryPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии галереи: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTakePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Открываем страницу фотобудки
                NavigationService.Navigate(new PhotoBoothPage(_driveService, _eventFolderId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии фотобудки: {ex.Message}", "Ошибка", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void AppStartPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Обработка нажатия клавиши F11 для возврата к настройкам
            if (e.Key == Key.F11)
            {
                ExitAppMode();
                e.Handled = true;
            }
        }
        
        private void CornerClickArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateTime now = DateTime.Now;
            
            // Сбрасываем счетчик, если прошло более 3 секунд с последнего клика
            if ((now - _lastClickTime).TotalSeconds > 3)
            {
                _cornerClickCount = 0;
            }
            
            _cornerClickCount++;
            _lastClickTime = now;
            
            // Если нажали 5 раз подряд в течение 3 секунд
            if (_cornerClickCount >= 5)
            {
                ExitAppMode();
                _cornerClickCount = 0;
            }
        }
        
        private void ExitAppMode()
        {
            // Вызываем колбэк для выхода из режима приложения
            _exitAppCallback?.Invoke();
        }
    }
}