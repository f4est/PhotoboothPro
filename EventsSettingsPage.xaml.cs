using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UnifiedPhotoBooth
{
    /// <summary>
    /// Логика взаимодействия для EventsSettingsPage.xaml
    /// </summary>
    public partial class EventsSettingsPage : Page
    {
        private GoogleDriveService _driveService;
        private Dictionary<string, string> _eventFolders;
        private Action<string, string> _onEventSelectedCallback;

        public EventsSettingsPage(GoogleDriveService driveService, Action<string, string> onEventSelected)
        {
            InitializeComponent();
            _driveService = driveService;
            _eventFolders = new Dictionary<string, string>();
            _onEventSelectedCallback = onEventSelected;
            
            // Загрузка списка событий
            RefreshEvents();
        }

        private void RefreshEvents()
        {
            try
            {
                cbEvents.Items.Clear();
                _eventFolders.Clear();
                
                // Получаем список папок-событий
                var events = _driveService.ListEvents();
                
                if (events.Count > 0)
                {
                    cbEvents.Items.Add("Выберите событие");
                    foreach (var eventItem in events)
                    {
                        cbEvents.Items.Add(eventItem.Key);
                        _eventFolders[eventItem.Key] = eventItem.Value;
                    }
                    cbEvents.SelectedIndex = 0;
                }
                else
                {
                    cbEvents.Items.Add("Нет доступных событий");
                    cbEvents.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка событий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CbEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEvents.SelectedIndex <= 0)
            {
                // txtCurrentEvent больше нет в новом дизайне
                _onEventSelectedCallback?.Invoke(null, null);
                return;
            }

            var selectedEvent = cbEvents.SelectedItem.ToString();
            string eventFolderId = _eventFolders.ContainsKey(selectedEvent) ? _eventFolders[selectedEvent] : null;
            
            // txtCurrentEvent больше нет в новом дизайне
            _onEventSelectedCallback?.Invoke(eventFolderId, selectedEvent);
        }

        private void BtnNewEvent_Click(object sender, RoutedEventArgs e)
        {
            InputDialog inputDialog = new InputDialog("Введите название события:", "Новое событие");
            if (inputDialog.ShowDialog() == true)
            {
                string newEventName = inputDialog.Answer.Trim();
                if (!string.IsNullOrEmpty(newEventName))
                {
                    try
                    {
                        // Создаем новое событие
                        _driveService.CreateEvent(newEventName);
                        
                        // Обновляем список событий
                        RefreshEvents();
                        
                        // Выбираем новое событие в списке
                        for (int i = 0; i < cbEvents.Items.Count; i++)
                        {
                            if (cbEvents.Items[i].ToString() == newEventName)
                            {
                                cbEvents.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании события: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}