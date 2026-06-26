using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class UsersManagementPage : Page
    {
        public UsersManagementPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new MentorContext())
            {
                dgUsers.ItemsSource = db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName != "Администратор" && !u.IsDeleted)
                    .ToList();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as User;

            if (user == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {user.FullName}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var db = new MentorContext())
                {
                    var userToDelete = db.Users.Find(user.UserID);
                    if (userToDelete != null)
                    {
                        userToDelete.IsDeleted = true;
                        db.SaveChanges();
                        MessageBox.Show("Пользователь удален!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                }
            }
        }
    }
}