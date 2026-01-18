using kurs4v28.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window 
    {
        private post_officeEntities2 context => ModelDB.GetContext();
        private employee new_emp;
        private AdminWindow _parentWindow;
        public CreateUserWindow(AdminWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
        }
        public CreateUserWindow(employee emp, AdminWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            new_emp = emp;
            usHeader.Text = "Редактирование пользователя";
            Create_btn.Content = "Сохранить";
            empName_txt.Text = new_emp.emp_name; empSurname_txt.Text = new_emp.emp_surname;
            empOtchestvo_txt.Text = new_emp.emp_otchestvo; empLogin_txt.Text = new_emp.login;
            empRole_txt.Text = new_emp.Role.role_name;
        }
        private void buttonCreateClick(object sender, RoutedEventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(empName_txt.Text) || string.IsNullOrWhiteSpace(empSurname_txt.Text) ||
                string.IsNullOrWhiteSpace(empLogin_txt.Text) || string.IsNullOrWhiteSpace(empRole_txt.Text))
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            if (new_emp == null && string.IsNullOrWhiteSpace(empPassword_txt.Text))
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            string login = empLogin_txt.Text.Trim();
            string role_name = empRole_txt.Text.Trim();
            var current_role = context.Role.FirstOrDefault(r => r.role_name == role_name);
            if (current_role == null)
            {
                MessageBox.Show("Роль пользователя не найдена!");
                return;
            }
            if (new_emp == null)
            {
                if (empPassword_txt.Text != empPassword1_txt.Text) { MessageBox.Show("Пароли не совпадают!"); return; }
                var result = Validation.CreateUserValidation(login, empPassword_txt.Text);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
                new_emp = new employee();
                context.employee.Add(new_emp);
            }
            else
            {
                if (empPassword_txt.Text != empPassword1_txt.Text) { MessageBox.Show("Пароли не совпадают!"); return; }
                var result = Validation.EditUserValidation(login, empPassword_txt.Text, current_role, new_emp);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }               
            }
            new_emp.emp_name = empName_txt.Text.Trim();
            new_emp.emp_surname = empSurname_txt.Text.Trim();
            new_emp.emp_otchestvo = empOtchestvo_txt.Text.Trim();
            new_emp.login = login;
            new_emp.role_id = current_role.id;
            if (!string.IsNullOrWhiteSpace(empPassword_txt.Text))
            {
                new_emp.password = Pass.HashPasswordWithBcrypt(empPassword_txt.Text.Trim());
            }
            context.SaveChanges();
            MessageBox.Show(Create_btn.Content.ToString() == "Сохранить" ?
                "Данные о пользователе изменены!" : "Пользователь добавлен!");
            EdDelRep newrep = new EdDelRep();
            newrep.user_id = _parentWindow.current_emp.id;
            if(Create_btn.Content.ToString() == "Сохранить") { newrep.deistv_name = "отредактировал"; }
            else { newrep.deistv_name = "добавил"; }                
            newrep.table_name = "пользователя";
            newrep.date_update = DateTime.Now;
            context.EdDelRep.Add(newrep);
            context.SaveChanges();
            _parentWindow.RefreshData();
            this.Close();
        }
    }
}