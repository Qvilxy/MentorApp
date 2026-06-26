using System.Data.Entity;
using System.Linq;
using System.Windows;
using MentorApp.Data;

namespace MentorApp
{
    public partial class MentorProfileWindow : Window
    {
        private int _userId;

        public MentorProfileWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadProfileData();
            LoadReviews();
        }

        private void LoadProfileData()
        {
            using (var db = new MentorContext())
            {
                var user = db.Users.Find(_userId);
                var mentor = db.Mentors.Include(m => m.Direction).FirstOrDefault(m => m.UserID == _userId);

                if (user != null && mentor != null)
                {
                    txtFullName.Text = $"ФИО: {user.FullName}";
                    txtEmail.Text = $"Email: {user.Email}";
                    txtDirection.Text = $"Направление: {mentor.Direction.DirectionName}";

                    int years = mentor.Experience ?? 0;
                    int months = mentor.ExperienceMonths;
                    txtExperience.Text = $"Стаж: {years} г. {months} мес.";

                    txtHourlyRate.Text = $"Ставка: {mentor.HourlyRate} ₽/час";

                    var avgRating = db.Reviews.Where(r => r.MentorID == mentor.MentorID).Average(r => (int?)r.Rating);
                    txtAverageRating.Text = $"Средний рейтинг: {avgRating?.ToString("0.0") ?? "Нет оценок"} ";
                }
            }
        }

        private void LoadReviews()
        {
            using (var db = new MentorContext())
            {
                var mentor = db.Mentors.FirstOrDefault(m => m.UserID == _userId);
                if (mentor != null)
                {
                    var reviews = db.Reviews
                        .Include(r => r.Mentee.User)
                        .Where(r => r.MentorID == mentor.MentorID)
                        .OrderByDescending(r => r.ReviewDate)
                        .ToList();

                    dgReviews.ItemsSource = reviews;
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}