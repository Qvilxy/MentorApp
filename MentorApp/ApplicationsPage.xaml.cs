using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class ApplicationsPage : Page
    {
        public ApplicationsPage()
        {
            InitializeComponent();
            LoadApplications();
        }

        private void LoadApplications()
        {
            using (var db = new MentorContext())
            {
                int currentUserId = MainWindow.CurrentUserID;
                string currentUserRole = MainWindow.CurrentUserRole;

                var query = db.Applications
                    .Include(a => a.Mentee.User)
                    .Include(a => a.Mentor.User)
                    .AsQueryable();

                if (currentUserRole == "Ментор")
                {
                    var mentor = db.Mentors.FirstOrDefault(m => m.UserID == currentUserId);
                    if (mentor != null)
                    {
                        query = query.Where(a => a.MentorID == mentor.MentorID);
                    }
                }
                else if (currentUserRole == "Менти")
                {
                    var mentee = db.Mentees.FirstOrDefault(m => m.UserID == currentUserId);
                    if (mentee != null)
                    {
                        query = query.Where(a => a.MenteeID == mentee.MenteeID);
                    }
                }

                dgApplications.ItemsSource = query.OrderByDescending(a => a.CreatedDate).ToList();
            }
        }

        private void DgApplications_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var application = dgApplications.SelectedItem as MentorApp.Models.Application;
            if (application == null) return;

            string currentUserRole = MainWindow.CurrentUserRole;

            if (currentUserRole == "Ментор" && application.Status == "Принята")
            {
                var scheduleWindow = new ScheduleMeetingWindow(application.ApplicationID);
                if (scheduleWindow.ShowDialog() == true)
                {
                    LoadApplications();
                }
            }
            else if (currentUserRole == "Ментор" && application.Status == "На рассмотрении")
            {
                var result = MessageBox.Show($"Принять заявку от {application.Mentee.User.FullName}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new MentorContext())
                    {
                        var app = db.Applications.Find(application.ApplicationID);
                        app.Status = "Принята";
                        db.SaveChanges();
                        LoadApplications();
                    }
                }
            }
        }

        private void DgApplications_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var application = dgApplications.SelectedItem as MentorApp.Models.Application;
            if (application == null) return;

            string currentUserRole = MainWindow.CurrentUserRole;

            // Проверяем, что клик был по ячейке со статусом
            var hitTestResult = VisualTreeHelper.HitTest(dgApplications, e.GetPosition(dgApplications));
            if (hitTestResult == null) return;

            DependencyObject obj = hitTestResult.VisualHit;
            while (obj != null && !(obj is DataGridCell))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            if (obj is DataGridCell cell)
            {
                var column = cell.Column as DataGridTextColumn;
                if (column != null && column.Header.ToString() == "Статус")
                {
                    e.Handled = true;
                    ShowStatusContextMenu(application);
                }
            }
        }

        private void ShowStatusContextMenu(MentorApp.Models.Application application)
        {
            var contextMenu = new ContextMenu();
            string currentUserRole = MainWindow.CurrentUserRole;

            if (currentUserRole == "Ментор")
            {
                if (application.Status == "На рассмотрении")
                {
                    var confirmItem = new MenuItem { Header = "Подтвердить. Назначить дату" };
                    confirmItem.Click += (s, args) =>
                    {
                        using (var db = new MentorContext())
                        {
                            var app = db.Applications.Find(application.ApplicationID);
                            app.Status = "Принята";
                            db.SaveChanges();
                        }

                        var scheduleWindow = new ScheduleMeetingWindow(application.ApplicationID);
                        if (scheduleWindow.ShowDialog() == true)
                        {
                            LoadApplications();
                        }
                    };
                    contextMenu.Items.Add(confirmItem);

                    var cancelItem = new MenuItem { Header = "Отменить" };
                    cancelItem.Click += (s, args) =>
                    {
                        using (var db = new MentorContext())
                        {
                            var app = db.Applications.Find(application.ApplicationID);
                            app.Status = "Отменена";
                            db.SaveChanges();
                            LoadApplications();
                        }
                    };
                    contextMenu.Items.Add(cancelItem);
                }
            }
            else if (currentUserRole == "Менти")
            {
                if (application.Status == "Принята" || application.Status == "Запланирована")
                {
                    var cancelItem = new MenuItem { Header = "Отменить встречу" };
                    cancelItem.Click += (s, args) =>
                    {
                        using (var db = new MentorContext())
                        {
                            var app = db.Applications.Find(application.ApplicationID);
                            app.Status = "Встреча отменена";
                            db.SaveChanges();
                            LoadApplications();
                        }
                    };
                    contextMenu.Items.Add(cancelItem);
                }
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.IsOpen = true;
            }
        }
    }
}