using System.Linq;
using System.Windows;
using MentorApp.Data;

namespace MentorApp
{
    public partial class MainWindow : Window
    {
        public static int CurrentUserID;
        public static string CurrentUserRole;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new MentorContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == txtLogin.Text && u.Password == txtPassword.Password);
                if (user != null)
                {
                    CurrentUserID = user.UserID;
                    CurrentUserRole = db.Roles.First(r => r.RoleID == user.RoleID).RoleName;
                    new MainApplicationWindow().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
            this.Close();
        }
    }
}