using System;
using System.Linq;
using System.Windows;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class ProfileSettingsWindow : Window
    {
        private int _currentUserId;

        public ProfileSettingsWindow(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            LoadProfileData();
            LoadDirections();
        }

        private void LoadProfileData()
        {
            using (var db = new MentorContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserID == _currentUserId);
                if (user != null)
                {
                    txtFullName.Text = user.FullName;
                    txtEmail.Text = user.Email;

                    var mentor = db.Mentors.FirstOrDefault(m => m.UserID == _currentUserId);
                    if (mentor != null)
                    {
                        txtHourlyRate.Text = mentor.HourlyRate.HasValue
                            ? mentor.HourlyRate.Value.ToString()
                            : "0";
                        txtExperienceYears.Text = (mentor.Experience ?? 0).ToString();
                        txtExperienceMonths.Text = mentor.ExperienceMonths.ToString();
                    }
                }
            }
        }

        private void LoadDirections()
        {
            using (var db = new MentorContext())
            {
                var directions = db.Directions
                    .Where(d => d.DirectionName == "Профессиональный ментор" ||
                                d.DirectionName == "IT ментор" ||
                                d.DirectionName == "Карьерный ментор")
                    .ToList();

                cmbDirection.ItemsSource = directions;
                cmbDirection.DisplayMemberPath = "DirectionName";
                cmbDirection.SelectedValuePath = "DirectionID";

                var currentMentor = db.Mentors.FirstOrDefault(m => m.UserID == _currentUserId);
                if (currentMentor != null)
                {
                    cmbDirection.SelectedValue = currentMentor.DirectionID;
                }
                else if (directions.Any())
                {
                    cmbDirection.SelectedIndex = 0;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDirection.SelectedItem == null)
            {
                MessageBox.Show("Выберите направление менторства!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal hourlyRate;
            if (!decimal.TryParse(txtHourlyRate.Text.Replace(".", ","), out hourlyRate))
            {
                MessageBox.Show("Введите корректную ставку (число)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int years, months;
            if (!int.TryParse(txtExperienceYears.Text, out years) || years < 0)
            {
                MessageBox.Show("Введите корректное количество лет стажа!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtExperienceMonths.Text, out months) || months < 0 || months > 11)
            {
                MessageBox.Show("Введите корректное количество месяцев стажа (0-11)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (hourlyRate < 0)
            {
                MessageBox.Show("Ставка не может быть отрицательной!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new MentorContext())
                {
                    var mentor = db.Mentors.FirstOrDefault(m => m.UserID == _currentUserId);
                    if (mentor != null)
                    {
                        mentor.HourlyRate = hourlyRate;
                        mentor.DirectionID = (int)cmbDirection.SelectedValue;
                        mentor.Experience = years;
                        mentor.ExperienceMonths = months;
                        mentor.IsAvailable = true;
                        db.SaveChanges();

                        MessageBox.Show("Профиль успешно обновлён!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Профиль ментора не найден!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
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