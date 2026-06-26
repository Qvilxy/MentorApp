using System.Linq;
using System.Windows;
using MentorApp.Data;

namespace MentorApp
{
    public partial class MenteeProfileWindow : Window
    {
        public MenteeProfileWindow(int userId)
        {
            InitializeComponent();
            LoadProfileData(userId);
        }

        private void LoadProfileData(int userId)
        {
            using (var db = new MentorContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    txtFullName.Text = $"ФИО: {user.FullName}";
                    txtEmail.Text = $"Email: {user.Email}";
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}