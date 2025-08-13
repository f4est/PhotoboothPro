using System;
using System.Windows;
using System.Windows.Controls;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для AppLauncherPage.xaml
    /// </summary>
    public partial class AppLauncherPage : Page
    {
        private GoogleDriveService _driveService;
        private string _eventFolderId;
        private string _eventName;

        public AppLauncherPage(GoogleDriveService driveService, string eventFolderId = null, string eventName = null)
        {
            InitializeComponent();
            _driveService = driveService;
            _eventFolderId = eventFolderId;
            _eventName = eventName;
            
            // Отображаем название события, если оно выбрано
            if (!string.IsNullOrEmpty(eventName))
            {
                txtEventName.Text = $"Событие: {eventName}";
            }
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
    }
}