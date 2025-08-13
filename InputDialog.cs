using System.Windows;
using System.Windows.Controls;

namespace UnifiedPhotoBooth
{
    public class InputDialog : Window
    {
        private TextBox txtAnswer;
        private Button btnDialogOk;
        
        public string Answer { get; private set; }
        
        public InputDialog(string question, string title)
        {
            this.Title = title;
            
            // Создаем элементы диалогового окна
            Grid grid = new Grid();
            grid.Margin = new Thickness(10);
            
            // Определяем строки
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // Добавляем текст вопроса
            TextBlock questionText = new TextBlock();
            questionText.Text = question;
            questionText.Margin = new Thickness(0, 0, 0, 10);
            Grid.SetRow(questionText, 0);
            grid.Children.Add(questionText);
            
            // Добавляем поле ввода
            txtAnswer = new TextBox();
            txtAnswer.Margin = new Thickness(0, 0, 0, 10);
            Grid.SetRow(txtAnswer, 1);
            grid.Children.Add(txtAnswer);
            
            // Добавляем кнопки
            StackPanel buttonsPanel = new StackPanel();
            buttonsPanel.Orientation = Orientation.Horizontal;
            buttonsPanel.HorizontalAlignment = HorizontalAlignment.Right;
            
            Button btnDialogCancel = new Button();
            btnDialogCancel.Content = "Отмена";
            btnDialogCancel.Margin = new Thickness(5, 0, 0, 0);
            btnDialogCancel.Click += (sender, e) => { this.DialogResult = false; this.Close(); };
            
            btnDialogOk = new Button();
            btnDialogOk.Content = "OK";
            btnDialogOk.IsDefault = true;
            btnDialogOk.Click += BtnDialogOk_Click;
            
            buttonsPanel.Children.Add(btnDialogOk);
            buttonsPanel.Children.Add(btnDialogCancel);
            
            Grid.SetRow(buttonsPanel, 2);
            grid.Children.Add(buttonsPanel);
            
            // Добавляем сетку в окно
            this.Content = grid;
            
            // Настройки окна
            this.Width = 300;
            this.SizeToContent = SizeToContent.Height;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize;
        }
        
        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Answer = txtAnswer.Text;
            DialogResult = true;
            Close();
        }
    }
}