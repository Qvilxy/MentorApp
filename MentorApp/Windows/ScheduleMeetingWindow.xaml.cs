using System;
using System.Windows;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class ScheduleMeetingWindow : Window
    {
        private int _applicationId;

        public ScheduleMeetingWindow(int applicationId)
        {
            InitializeComponent();
            _applicationId = applicationId;
            dpMeetingDate.SelectedDate = DateTime.Now.AddDays(1);
            cmbMeetingTime.SelectedIndex = 2; 
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (dpMeetingDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату встречи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbMeetingTime.Text))
            {
                MessageBox.Show("Выберите время встречи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTopic.Text))
            {
                MessageBox.Show("Введите тему встречи!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {

                if (!TimeSpan.TryParse(cmbMeetingTime.Text, out TimeSpan meetingTime))
                {
                    MessageBox.Show("Неверный формат времени! Используйте формат ЧЧ:ММ", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime meetingDateTime = dpMeetingDate.SelectedDate.Value.Date + meetingTime;


                if (meetingDateTime < DateTime.Now)
                {
                    MessageBox.Show("Нельзя запланировать встречу в прошлом!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var db = new MentorContext())
                {
                    var meeting = new Meeting
                    {
                        ApplicationID = _applicationId,
                        MeetingDate = meetingDateTime,
                        Topic = txtTopic.Text.Trim(),
                        Status = "Запланирована"
                    };

                    db.Meetings.Add(meeting);
                    db.SaveChanges();

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при планировании встречи: {ex.Message}", "Ошибка",
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