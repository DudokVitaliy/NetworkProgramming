using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetClient
{
    public partial class Form1 : Form
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;

        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            string index = textBoxIndex.Text.Trim();

            if (string.IsNullOrEmpty(index))
            {
                MessageBox.Show("Введіть поштовий індекс!");
                return;
            }

            buttonSend.Enabled = false; // блокуємо кнопку під час запиту

            try
            {
                string result = await Task.Run(() => SendRequest(index));

                listBoxStreets.Items.Clear();

                if (result == "Not found")
                {
                    listBoxStreets.Items.Add("Індекс не знайдено");
                    return;
                }

                if (result.StartsWith("Error"))
                {
                    listBoxStreets.Items.Add(result);
                    return;
                }

                string[] streets = result.Split(',');

                foreach (string street in streets)
                {
                    listBoxStreets.Items.Add(street);
                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Не вдалося підключитися до сервера!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
            finally
            {
                buttonSend.Enabled = true; // повертаємо кнопку
            }
        }

        private string SendRequest(string index)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork,
                                              SocketType.Stream,
                                              ProtocolType.Tcp))
            {
                // Підключення
                socket.Connect(SERVER_IP, SERVER_PORT);

                // Відправка індексу
                byte[] requestData = Encoding.UTF8.GetBytes(index);
                socket.Send(requestData);

                // Отримання відповіді
                byte[] buffer = new byte[1024];
                int size = socket.Receive(buffer);

                string response = Encoding.UTF8.GetString(buffer, 0, size);

                return response;
            }
        }
    }
}