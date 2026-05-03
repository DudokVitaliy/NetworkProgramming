using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MessengerWPF
{
    public partial class RegisterWindow : Window
    {
        private static string file = "users.json";
        private static Dictionary<string, string> users = LoadUsers();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заповніть всі поля!");
                return;
            }

            if (users.ContainsKey(login))
            {
                MessageBox.Show("Такий користувач вже існує!");
                return;
            }

            users.Add(login, password);
            SaveUsers();

            MessageBox.Show("Успішна реєстрація!");

            ChatWindow chat = new ChatWindow(login);
            chat.Show();
            this.Close();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private static Dictionary<string, string> LoadUsers()
        {
            if (!File.Exists(file))
                return new Dictionary<string, string>();

            return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));
        }

        private static void SaveUsers()
        {
            File.WriteAllText(file, JsonSerializer.Serialize(users));
        }
    }
}