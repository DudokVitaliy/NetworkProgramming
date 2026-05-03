using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        string filePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AttachBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == true)
            {
                filePath = dlg.FileName;
                fileLabel.Text = filePath;
            }
        }

        private void SendBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(emailBox.Text);
                mail.To.Add(toBox.Text);
                mail.Subject = subjectBox.Text;
                mail.Body = bodyBox.Text;

                if (!string.IsNullOrEmpty(filePath))
                {
                    mail.Attachments.Add(new Attachment(filePath));
                }

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(emailBox.Text, passBox.Password);
                smtp.EnableSsl = true;

                smtp.Send(mail);

                MessageBox.Show("Email sent!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}