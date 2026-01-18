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
using static MaterialDesignThemes.Wpf.Theme;

namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для CreateServiceWindow.xaml
    /// </summary>
    public partial class CreateServiceWindow : Window
    {
        private post_officeEntities2 context => ModelDB.GetContext();
        UserWindow _parentWindow;
        services new_service=null;
        public CreateServiceWindow(UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            
        }
        public CreateServiceWindow(services serv, UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            new_service = serv;
            servHeader.Text = "Редактирование услуги";
            servName_txt.Text = new_service.name; servPrice_txt.Text = new_service.price.ToString();
            Create_btn.Content = "Сохранить";
        }
        private void buttonCreateClick(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(servPrice_txt.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!");
                return;
            }
            if (string.IsNullOrWhiteSpace(servName_txt.Text)|| string.IsNullOrWhiteSpace(servPrice_txt.Text)
                || decimal.Parse(servPrice_txt.Text)<=0)
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            string service_name = servName_txt.Text.Trim();
            if (new_service != null)
            {
                var result = Validation.EditServiceValidation(service_name, new_service);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
            }
            else
            {
                var result = Validation.CreateServiceValidation(service_name);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
                new_service = new services();
                context.services.Add(new_service);
            }
            new_service.name = servName_txt.Text;
            new_service.price = decimal.Parse(servPrice_txt.Text);
            context.SaveChanges();
            _parentWindow.RefreshData();
            EdDelRep newrep = new EdDelRep();
            newrep.user_id = _parentWindow.current_emp.id;
            if (Create_btn.Content.ToString() == "Сохранить") { newrep.deistv_name = "отредактировал"; }
            else { newrep.deistv_name = "добавил"; }
            newrep.table_name = "услугу";
            newrep.date_update = DateTime.Now;
            context.EdDelRep.Add(newrep);
            context.SaveChanges();
            _parentWindow.RefreshData();
            this.Close();
        }
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!servPrice_txt.Text.Contains(".")
               && servPrice_txt.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
    }
}
