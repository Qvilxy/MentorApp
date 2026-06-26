using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MentorApp.Data;

namespace MentorApp
{
    public partial class MentorsPage : Page
    {
        public MentorsPage()
        {
            InitializeComponent();
            LoadMentors();
        }

        private void LoadMentors()
        {
            using (var db = new MentorContext())
            {
                dgMentors.ItemsSource = db.Mentors
                    .Include(m => m.User)
                    .Include(m => m.Direction)
                    .Where(m => m.IsAvailable)
                    .ToList();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new MentorContext())
            {
                var query = db.Mentors.Include(m => m.User).Include(m => m.Direction).Where(m => m.IsAvailable);
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                    query = query.Where(m => m.User.FullName.Contains(txtSearch.Text));
                dgMentors.ItemsSource = query.ToList();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadMentors();

        private void DgMentors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mentor = dgMentors.SelectedItem as Models.Mentor;
            if (mentor != null)
                new MentorDetailsWindow(mentor.MentorID).ShowDialog();
        }
    }
}