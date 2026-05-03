using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MessengerWPF
{
    public partial class ChatWindow : Window
    {
        private string currentUser;
        private List<string> groupUsers;
        private DispatcherTimer timer;

        public ChatWindow(string login)
        {
            InitializeComponent();

            currentUser = login;
            Title = $"Messenger - {login}";

            LoadUsers();

            UsersList.SelectionChanged += UsersList_SelectionChanged;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void LoadUsers()
        {
            UsersList.Items.Clear();
            groupUsers = new List<string> { currentUser };

            foreach (var user in DataStorage.Users)
            {
                if (user.Login != currentUser)
                {
                    UsersList.Items.Add(user.Login);
                    groupUsers.Add(user.Login);
                }
            }

            UpdateOnlineStatus();
        }

        private void UpdateOnlineStatus()
        {

            var onlineUsers = groupUsers.Where(u => DataStorage.Users.FirstOrDefault(x => x.Login == u)?.IsOnline == true);
            OnlineStatusTextBlock.Text = "Онлайн: " + string.Join(", ", onlineUsers);
        }

        private void LoadChat()
        {
            MessagesPanel.Children.Clear();

            foreach (var msg in DataStorage.GroupChats)
            {
                bool isMe = msg.StartsWith(currentUser + ":");
                AddMessage(msg, isMe);
            }
        }

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadChat();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string msg = MessageBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(msg)) return;

            string fullMsg = $"{currentUser}: {msg}";

            SaveMessage(fullMsg); 
            MessageBox.Clear();
            LoadChat();
        }


        private void SaveMessage(string msg)
        {
            DataStorage.GroupChats.Add(msg);
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            LoadChat(); 
        }

        private void AddMessage(string text, bool isMe)
        {
            Border bubble = new Border
            {
                Background = isMe
                    ? Brushes.DodgerBlue
                    : new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(10),
                Margin = new Thickness(5),
                HorizontalAlignment = isMe
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left,
                MaxWidth = 320
            };

            TextBlock tb = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = tb;
            MessagesPanel.Children.Add(bubble);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.FocusedElement is TextBox)
            {
                Send_Click(sender, e);
            }
        }
    }
}