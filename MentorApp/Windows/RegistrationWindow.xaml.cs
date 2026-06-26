using System;
using System.Linq;
using System.Windows;
using MentorApp.Data;
using MentorApp.Models;

namespace MentorApp
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                using (var db = new MentorContext())
                {
                    // Проверяем подключение к базе данных
                    if (!db.Database.Exists())
                    {
                        MessageBox.Show("База данных MentorDB не найдена!\n\nСоздайте базу данных в SQL Server Management Studio.",
                            "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получаем роли (исключая Администратора)
                    var roles = db.Roles.Where(r => r.RoleName != "Администратор").ToList();

                    if (roles == null || roles.Count == 0)
                    {
                        MessageBox.Show("Таблица Roles пустая или не содержит данных!\n\nДобавьте роли 'Ментор' и 'Менти' в базу данных.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Заполняем ComboBox
                    cmbRole.ItemsSource = roles;
                    cmbRole.DisplayMemberPath = "RoleName";
                    cmbRole.SelectedValuePath = "RoleID";
                    cmbRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей:\n\n{ex.Message}\n\n" +
                    $"Возможные причины:\n" +
                    $"1. SQL Server Express не запущен\n" +
                    $"2. База данных MentorDB не существует\n" +
                    $"3. Таблица Roles не заполнена данными\n" +
                    $"4. Неверная строка подключения в App.config",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите Email!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите Логин!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите Пароль!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите Роль!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbRole.Focus();
                return;
            }

            // Проверка корректности email
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Введите корректный email адрес!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // Проверка длины пароля
            if (txtPassword.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать не менее 4 символов!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                using (var db = new MentorContext())
                {
                    // Проверка на существующий логин
                    if (db.Users.Any(u => u.Login == txtLogin.Text.Trim()))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!\n\nПридумайте другой логин.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtLogin.Focus();
                        return;
                    }

                    // Проверка на существующий email
                    if (db.Users.Any(u => u.Email == txtEmail.Text.Trim()))
                    {
                        MessageBox.Show("Пользователь с таким email уже существует!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtEmail.Focus();
                        return;
                    }

                    // Получаем выбранную роль
                    var selectedRoleId = (int)cmbRole.SelectedValue;
                    var selectedRole = db.Roles.Find(selectedRoleId);

                    if (selectedRole == null)
                    {
                        MessageBox.Show("Выбранная роль не найдена в базе данных!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получаем первое направление по умолчанию
                    var defaultDirection = db.Directions.FirstOrDefault();
                    if (defaultDirection == null)
                    {
                        MessageBox.Show("В базе данных нет направлений!\n\nОбратитесь к администратору.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Создаем нового пользователя
                    var user = new User
                    {
                        FullName = txtFullName.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Login = txtLogin.Text.Trim(),
                        Password = txtPassword.Password, // В реальном проекте нужно хешировать!
                        RoleID = selectedRoleId
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    // Создаем запись в таблице Ментор или Менти
                    if (selectedRole.RoleName == "Ментор")
                    {
                        var mentor = new Mentor
                        {
                            UserID = user.UserID,
                            DirectionID = defaultDirection.DirectionID,
                            Experience = 0,
                            Description = "",
                            HourlyRate = 0,
                            IsAvailable = true
                        };
                        db.Mentors.Add(mentor);
                    }
                    else if (selectedRole.RoleName == "Менти")
                    {
                        var mentee = new Mentee
                        {
                            UserID = user.UserID,
                            DirectionID = defaultDirection.DirectionID,
                            Goal = ""
                        };
                        db.Mentees.Add(mentee);
                    }

                    db.SaveChanges();

                    MessageBox.Show($"Регистрация успешна!\n\n" +
                        $"ФИО: {user.FullName}\n" +
                        $"Логин: {user.Login}\n" +
                        $"Роль: {selectedRole.RoleName}\n\n" +
                        $"Теперь вы можете войти в систему.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Возвращаемся на окно авторизации
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации:\n\n{ex.Message}\n\n" +
                    $"Проверьте подключение к базе данных и попробуйте еще раз.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}