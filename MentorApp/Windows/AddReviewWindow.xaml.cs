using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class AddReviewWindow : Window
    {
        public AddReviewWindow()
        {
            InitializeComponent();
            LoadMentors();
            cmbRating.SelectedIndex = 4;
        }

        private void LoadMentors()
        {
            using (var db = new MentorContext())
            {
                int currentUserId = MainWindow.CurrentUserID;
                var mentee = db.Mentees.FirstOrDefault(m => m.UserID == currentUserId);

                if (mentee != null)
                {
                    var completedMeetings = db.Meetings
                        .Include(m => m.Application.Mentor.User)
                        .Where(m => m.Application.MenteeID == mentee.MenteeID &&
                                   m.Status == "Прошла успешно")
                        .Select(m => m.Application.Mentor)
                        .Distinct()
                        .ToList();

                    cmbMentor.ItemsSource = completedMeetings;
                    cmbMentor.DisplayMemberPath = "User.FullName";
                    cmbMentor.SelectedValuePath = "MentorID";
                }
            }
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMentor.SelectedItem == null)
            {
                MessageBox.Show("Выберите ментора!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbRating.SelectedItem == null)
            {
                MessageBox.Show("Выберите оценку!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rating = cmbRating.SelectedIndex + 1;

            try
            {
                using (var db = new MentorContext())
                {
                    int currentUserId = MainWindow.CurrentUserID;
                    var mentee = db.Mentees.FirstOrDefault(m => m.UserID == currentUserId);
                    int mentorId = (int)cmbMentor.SelectedValue;

                    var review = new Review
                    {
                        MentorID = mentorId,
                        MenteeID = mentee.MenteeID,
                        Rating = rating,
                        Comment = txtComment.Text.Trim(),
                        ReviewDate = DateTime.Now
                    };

                    db.Reviews.Add(review);
                    db.SaveChanges();

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}