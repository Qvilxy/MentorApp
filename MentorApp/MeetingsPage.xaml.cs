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
    public partial class MeetingsPage : Page
    {
        public MeetingsPage()
        {
            InitializeComponent();
            LoadMeetings();
        }

        private void LoadMeetings()
        {
            using (var db = new MentorContext())
            {
                int currentUserId = MainWindow.CurrentUserID;
                string currentUserRole = MainWindow.CurrentUserRole;

                var query = db.Meetings
                    .Include(m => m.Application.Mentee.User)
                    .Include(m => m.Application.Mentor.User)
                    .AsQueryable();

                if (currentUserRole == "Ментор")
                {
                    var mentor = db.Mentors.FirstOrDefault(m => m.UserID == currentUserId);
                    if (mentor != null)
                    {
                        query = query.Where(m => m.Application.MentorID == mentor.MentorID);
                    }
                }
                else if (currentUserRole == "Менти")
                {
                    var mentee = db.Mentees.FirstOrDefault(m => m.UserID == currentUserId);
                    if (mentee != null)
                    {
                        query = query.Where(m => m.Application.MenteeID == mentee.MenteeID);
                    }
                }

                dgMeetings.ItemsSource = query.OrderByDescending(m => m.MeetingDate).ToList();
            }
        }

        private void DgMeetings_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var meeting = dgMeetings.SelectedItem as Meeting;
            if (meeting == null) return;

            string currentUserRole = MainWindow.CurrentUserRole;

            if (currentUserRole == "Менти" && meeting.Status == "Запланирована")
            {
                e.Handled = true;
                var contextMenu = new ContextMenu();

                var cancelItem = new MenuItem { Header = "Отменить встречу" };
                cancelItem.Click += (s, args) =>
                {
                    using (var db = new MentorContext())
                    {
                        var m = db.Meetings.Find(meeting.MeetingID);
                        m.Status = "Отменена";
                        db.SaveChanges();
                        MessageBox.Show("Встреча отменена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMeetings();
                    }
                };

                contextMenu.Items.Add(cancelItem);
                contextMenu.PlacementTarget = (FrameworkElement)sender;
                contextMenu.IsOpen = true;
            }
            else if (currentUserRole == "Ментор" && meeting.Status == "Запланирована")
            {
                e.Handled = true;
                var contextMenu = new ContextMenu();

                var completeItem = new MenuItem { Header = "Прошла успешно" };
                completeItem.Click += (s, args) =>
                {
                    using (var db = new MentorContext())
                    {
                        var m = db.Meetings.Find(meeting.MeetingID);
                        m.Status = "Прошла успешно";
                        db.SaveChanges();
                        MessageBox.Show("Встреча завершена! Менти может оставить отзыв.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMeetings();
                    }
                };

                var cancelItem = new MenuItem { Header = "Отменить встречу" };
                cancelItem.Click += (s, args) =>
                {
                    using (var db = new MentorContext())
                    {
                        var m = db.Meetings.Find(meeting.MeetingID);
                        m.Status = "Отменена";
                        db.SaveChanges();
                        MessageBox.Show("Встреча отменена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMeetings();
                    }
                };

                contextMenu.Items.Add(completeItem);
                contextMenu.Items.Add(cancelItem);
                contextMenu.PlacementTarget = (FrameworkElement)sender;
                contextMenu.IsOpen = true;
            }
        }
    }
}