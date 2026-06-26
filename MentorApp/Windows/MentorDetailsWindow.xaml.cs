using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class MentorDetailsWindow : Window
    {
        private int _mentorID;
        public MentorDetailsWindow(int mentorID)
        {
            InitializeComponent();
            _mentorID = mentorID;
            using (var db = new MentorContext())
            {
                var m = db.Mentors.Include(x => x.User).Include(x => x.Direction).First(x => x.MentorID == mentorID);
                txtName.Text = m.User.FullName;
                txtDirection.Text = "Направление: " + m.Direction.DirectionName;
                txtExperience.Text = "Опыт: " + m.Experience + " лет";
                txtRate.Text = "Ставка: " + m.HourlyRate + " ₽/час";
                txtDescription.Text = m.Description ?? "Описание не указано";
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            int currentUserID = MainWindow.CurrentUserID;
            using (var db = new MentorContext())
            {
                var mentee = db.Mentees.FirstOrDefault(m => m.UserID == currentUserID);
                if (mentee == null)
                {
                    MessageBox.Show("Только Менти могут подавать заявки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (db.Applications.Any(a => a.MenteeID == mentee.MenteeID && a.MentorID == _mentorID && a.Status == "На рассмотрении"))
                {
                    MessageBox.Show("Заявка уже отправлена!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                db.Applications.Add(new MentorApp.Models.Application
                {
                    MenteeID = mentee.MenteeID,
                    MentorID = _mentorID,
                    Status = "На рассмотрении",
                    CreatedDate = DateTime.Now,
                    Comment = txtComment.Text
                });
                db.SaveChanges();
                MessageBox.Show("Заявка отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}