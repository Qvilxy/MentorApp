using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class ReviewsPage : Page
    {
        public ReviewsPage()
        {
            InitializeComponent();
            LoadReviews();
        }

        private void LoadReviews()
        {
            using (var db = new MentorContext())
            {
                string currentUserRole = MainWindow.CurrentUserRole;

                if (currentUserRole == "Ментор")
                {
                    colDate.Header = "Дата встречи";
                    btnAddReview.Visibility = Visibility.Collapsed;
                }
                else
                {
                    colDate.Header = "Дата";
                    btnAddReview.Visibility = Visibility.Visible;
                }

                dgReviews.ItemsSource = db.Reviews
                    .Include(r => r.Mentor.User)
                    .Include(r => r.Mentee.User)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList();
            }
        }

        private void BtnAddReview_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserRole != "Менти")
            {
                MessageBox.Show("Только менти могут оставлять отзывы!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addReviewWindow = new AddReviewWindow();
            if (addReviewWindow.ShowDialog() == true)
            {
                MessageBox.Show("Отзыв успешно добавлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadReviews();
            }
        }
    }
}