using System.Windows;
using System.Windows.Controls;
using MentorApp.Data;

namespace MentorApp
{
    public partial class MainApplicationWindow : Window
    {
        public MainApplicationWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            MainFrame.Navigate(new MentorsPage());
        }

        private void LoadUserInfo()
        {
            using (var db = new MentorContext())
            {
                var user = db.Users.Find(MainWindow.CurrentUserID);
                string role = MainWindow.CurrentUserRole;

                btnUserInfo.Content = $"👤 {user.FullName} [{role}]";

                if (role == "Ментор")
                {
                    btnApplications.Content = "Поступившие заявки";
                    btnUserInfo.ToolTip = "Нажмите для просмотра профиля и отзывов";
                }
                else if (role == "Менти")
                {
                    btnApplications.Content = "Мои заявки";
                    btnUserInfo.ToolTip = "Нажмите для просмотра профиля";
                }
                else if (role == "Администратор")
                {
                    adminPanel.Visibility = Visibility.Visible;
                    btnMentors.Visibility = Visibility.Collapsed;
                    btnApplications.Visibility = Visibility.Collapsed;
                    btnMeetings.Visibility = Visibility.Collapsed;
                    btnUserInfo.Visibility = Visibility.Collapsed;
                    MainFrame.Navigate(new UsersManagementPage());
                    return;
                }
            }
        }

        private void BtnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserRole == "Ментор")
            {
                var profileWindow = new MentorProfileWindow(MainWindow.CurrentUserID);
                profileWindow.ShowDialog();
            }
            else if (MainWindow.CurrentUserRole == "Менти")
            {
                var profileWindow = new MenteeProfileWindow(MainWindow.CurrentUserID);
                profileWindow.ShowDialog();
            }
        }

        private void BtnMentors_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new MentorsPage());
        private void BtnApplications_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ApplicationsPage());
        private void BtnMeetings_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new MeetingsPage());

        private void BtnUsers_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new UsersManagementPage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}