using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MessengerWPF
{
    public partial class LoginWindow : Window
    {
        private static string file = "users.json";
        private static Dictionary<string, string> users = LoadUsers();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заповніть всі поля!");
                return;
            }

            if (!users.ContainsKey(login))
            {
                MessageBox.Show("Користувача не знайдено!");
                return;
            }

            if (users[login] != password)
            {
                MessageBox.Show("Невірний пароль!");
                return;
            }

            ChatWindow chat = new ChatWindow(login);
            chat.Show();
            this.Close();
        }

        private static Dictionary<string, string> LoadUsers()
        {
            if (!File.Exists(file))
                return new Dictionary<string, string>();

            return JsonSerializer.Deserialize<Dictionary<string, string>>(
                File.ReadAllText(file)
            ) ?? new Dictionary<string, string>();
        }
        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow reg = new RegisterWindow();
            reg.Show();
            this.Close();
        }
    }
}