using kurs4v28.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string path = "aaa.chm";
        private post_officeEntities2 context => ModelDB.GetContext();
        private employee emp;
        private popitki_vhoda popit;

        private popitki_vhoda current_popit;

        public MainWindow()
        {
            InitializeComponent();
            popitochka();
            CreateDefaultAdmin();
            LoadRememberedCredentials();
        }
        private void LoadRememberedCredentials()
        {
            try
            {
                var (savedLogin, savedPassword) = RememberMe.LoadCredentials();
                if (!string.IsNullOrEmpty(savedLogin) && !string.IsNullOrEmpty(savedPassword))
                {
                    login_txt.Text = savedLogin;
                    password_txt.Password = savedPassword;
                    chkRememberMe.IsChecked = true;
                }
            }
            catch {/*и так сойдет*/ }
        }
        private void popitochka()
        {
            string pc = System.Environment.MachineName;
            if(!context.popitki_vhoda.Any(pop => pop.pc_name == pc))
            {
                current_popit = new popitki_vhoda();
                current_popit.popitka_number = 0;
                current_popit.first_popitka = DateTime.Now;
                current_popit.last_popitka = DateTime.Now;
                current_popit.pc_name = pc;
                current_popit.isBlocked = false;
                Vhod_btn.IsEnabled = true;
                context.popitki_vhoda.Add(current_popit);
                context.SaveChanges();
            }
            else
            {
                current_popit = context.popitki_vhoda.FirstOrDefault(p => p.pc_name == pc);
            }
            if (current_popit.last_popitka < DateTime.Now)
            {
                current_popit.isBlocked = false;
                Vhod_btn.IsEnabled = true;
                current_popit.popitka_number = 0;
                context.SaveChanges();
            }
            if (current_popit.isBlocked) { Vhod_btn.IsEnabled = false; }
            else { Vhod_btn.IsEnabled = true; }
        }
        private void CreateDefaultAdmin()
        {
            bool isEmpty = !context.employee.Any(e => e.role_id == 1);
            if (isEmpty)
            {
                emp = new employee();
                emp.emp_name = "Иван"; emp.emp_surname = "Иванов"; emp.emp_otchestvo = "Иванович";
                string defaultpass = "12345678";
                emp.password = Pass.HashPasswordWithBcrypt(defaultpass);
                emp.role_id = 1; emp.login = "admin";
                context.employee.Add(emp);
                context.SaveChanges();
                MessageBox.Show("Пользователи admin отсутствуют в базе! Создан стандартный пользователь admin.");
            }
            else
            {
                return;
            }       
        }
        private void Button_voiti_Click(object sender, RoutedEventArgs e)
        {
            if (current_popit.isBlocked && current_popit.last_popitka > DateTime.Now)
            {
                MessageBox.Show("Приложение заблокировано! Попробуйте позже.");
                return;
            }
            var user = context.employee.FirstOrDefault(emp => emp.login == login_txt.Text);
            if (user != null && Pass.VerifyBcryptPassword(password_txt.Password, user.password))
            {
                current_popit.popitka_number = 0; current_popit.isBlocked = false; context.SaveChanges(); Vhod_btn.IsEnabled = true;
                if (chkRememberMe.IsChecked == true)
                {
                    RememberMe.SaveCredentials(login_txt.Text, password_txt.Password);
                }
                else
                {
                    RememberMe.ClearCredentials();
                }
                if (user.role_id == 1)
                {
                    AdminWindow adminWindow = new AdminWindow(user)
                    {
                        WindowStartupLocation = WindowStartupLocation,
                        Left = Left,
                        Top = Top
                    };
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    UserWindow userWindow = new UserWindow(user)
                    {
                        WindowStartupLocation = WindowStartupLocation,
                        Left = Left,
                        Top = Top
                    };
                    userWindow.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Ошибка в логине или пароле! Пользователь не существует!");
                if (current_popit.popitka_number < 9)
                {
                    current_popit.isBlocked = false;
                    current_popit.popitka_number++;
                    current_popit.last_popitka = DateTime.Now;
                    MessageBox.Show("Приложение заблокируется через " + (10 - current_popit.popitka_number) + " попыток.");
                    Vhod_btn.IsEnabled = true;
                    context.SaveChanges();
                }
                else
                {
                    current_popit.popitka_number = 0;
                    current_popit.last_popitka = DateTime.Now.AddMinutes(5);
                    current_popit.isBlocked = true;
                    Vhod_btn.IsEnabled = false;
                    context.SaveChanges();
                    MessageBox.Show("Вы превысили допустимое количество попыток входа, приложение заблокировано! Попробуйте позже через 5 минут.");
                }
            }
        }
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"Файл справки не найден по пути:\n{fullPath}");
                return;
            }
            // Открытие справки
            System.Diagnostics.Process.Start(fullPath);
        }
    }
}
