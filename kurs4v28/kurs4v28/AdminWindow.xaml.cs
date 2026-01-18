using kurs4v28.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private string admindeliting; //удалю по рофлу добавила
        private string admindeliting1; //удалю по рофлу добавила
        private string admindeliting2; //удалю по рофлу добавила
        private post_officeEntities2 context => ModelDB.GetContext();
        internal employee current_emp;
        private List<employee> _employees;
        private List<Role> _rolee;
        private List <EdDelRep> _report;
        public AdminWindow(employee emp)
        {
            InitializeComponent();
            current_emp = emp;
            _employees = context.employee.Include(e => e.Role).Where(s => s.isDeleted != true).ToList();
            usersData.ItemsSource = _employees;
            title_user.Text += " " + current_emp.emp_name + " " + current_emp.emp_surname;
            userPassword.Binding = new Binding("********");
            _rolee = context.Role.ToList();
            //отчетики
            _report = context.EdDelRep.Include(r => r.employee).ToList();
            repData.ItemsSource = _report;  

        }
        public void buttonExitClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите выйти из аккаунта?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mv = new MainWindow() { WindowStartupLocation = WindowStartupLocation, Left = Left, Top = Top };
                mv.Show();
                this.Close();
            }
        }
        public void RefreshData()
        {
            _employees = context.employee.Include(e => e.Role).Where(s => s.isDeleted!=true).ToList();
            usersData.ItemsSource = _employees;
            _report = context.EdDelRep.Include(r => r.employee).ToList();
            repData.ItemsSource = _report;
            usersData.Items.Refresh(); 
            repData.Items.Refresh();
        }
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                var deleteItem = contextMenu.Items.OfType<MenuItem>().FirstOrDefault(x => x.Header.ToString() == "Удалить");
                if (deleteItem != null)
                {
                    var selectedUser = usersData.SelectedItem as employee;
                    deleteItem.IsEnabled = selectedUser != null && selectedUser.id != current_emp.id;
                }
            }
        }
        private void usersData_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (usersData.SelectedItem == null)
            {
                e.Handled = true;
            }
        }
        private void buttonAddUserClick(object sender, RoutedEventArgs e)
        {
            var createUserWindow = new CreateUserWindow(this);
            createUserWindow.ShowDialog();
            

        }
        private void buttonDeleteUserClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = usersData.SelectedItem as dynamic;
            if (selectedItem == null) { return; }
            string del = selectedItem.login;
            employee delemp = context.employee.Where(s => s.isDeleted != true).FirstOrDefault(emp => emp.login == del);
            if (delemp == null)
            {
                MessageBox.Show("Сотрудник не найден!");
                return;
            }
            var otprEmoloyee = context.post_otpravlenie.Where(s => s.isDeleted!=true).FirstOrDefault(eid => eid.emp_id == delemp.id);
            if (current_emp.id == delemp.id) { MessageBox.Show("Вы не можете удалить свой аккаунт!"); return; }
            MessageBoxResult resultt = MessageBox.Show(
           "Вы действительно хотите удалить этого сотрудника?",
           "Подтверждение выхода",
           MessageBoxButton.YesNo,
           MessageBoxImage.Question

        );
            if (resultt == MessageBoxResult.Yes)
            {
                selectedItem.isDeleted = true;
                context.SaveChanges();
                MessageBox.Show("Сотрудник удален!");
                EdDelRep newrep = new EdDelRep();
                newrep.user_id = current_emp.id;
                newrep.deistv_name = "удалил";
                newrep.table_name = "пользователя";
                newrep.date_update = DateTime.Now;
                context.EdDelRep.Add(newrep);
                context.SaveChanges();
                RefreshData();
            }
            
        }
        private void buttonEditUserClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = usersData.SelectedItem as dynamic;
            if (selectedItem == null) { return; }
            string edit = selectedItem.login;
            employee edemp = context.employee.Where(s => s.isDeleted != true).FirstOrDefault(emp => emp.login == edit);
            if (edemp == null) 
            {
                MessageBox.Show("Сотрудник не найден!");
                return;
            }
            CreateUserWindow createUserWindow = new CreateUserWindow(edemp, this);
            createUserWindow.ShowDialog();
            RefreshData();
        }
        private void comboboxElementClick(object sender, SelectionChangedEventArgs e)
        {
            if (empRole_txt.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Content.ToString();
                if (selectedRole == "Выберите роль")
                {
                    _employees = context.employee.Include(e1 => e1.Role).Where(s => s.isDeleted != true).ToList();
                }
                else
                {
                    _employees = context.employee.Include(e1 => e1.Role).Where(e1 => e1.Role.role_name == selectedRole && e1.isDeleted != true).ToList();
                }
                usersData.ItemsSource = _employees;
                usersData.Items.Refresh();
            }
        }
        private void loginSearchTextChamged(object sender, TextChangedEventArgs e)
        {
            List<employee> serchEmp = context.employee.Where(e1 => e1.login.Contains(loginSearch.Text)||e1.emp_surname.Contains(loginSearch.Text)).Where(s => s.isDeleted!=true).ToList();          
            usersData.ItemsSource = serchEmp;
        }

    }
}
