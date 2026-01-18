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
    /// Логика взаимодействия для CreateClientWindow.xaml
    /// </summary>
    public partial class CreateClientWindow : Window
    {
        private post_officeEntities2 context => ModelDB.GetContext();
        UserWindow _parentWindow;
        Client new_client = null;
        public CreateClientWindow(UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
        }
        public CreateClientWindow(Client cl, UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            new_client = cl;
            clHeader.Text = "Редактирование клиента";
            Create_btn.Content = "Сохранить";
            clName_txt.Text = cl.name; clSurname_txt.Text = cl.surname;
            clOtchestvo_txt.Text = cl.otchestvo; clAddress_txt.Text = cl.address;
            clPhone_txt.Text = cl.phone; clPassport_txt.Text = cl.passport_seria_number;
            clPassportKemVidan_txt.Text = cl.kem_vidan_passport; clPostIndex_txt.Text = cl.post_index;
        }     
        private void buttonCreateClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(clName_txt.Text) || string.IsNullOrWhiteSpace(clSurname_txt.Text) ||
                string.IsNullOrWhiteSpace(clOtchestvo_txt.Text) || string.IsNullOrWhiteSpace(clAddress_txt.Text) ||
                string.IsNullOrWhiteSpace(clPhone_txt.Text) || string.IsNullOrWhiteSpace(clPassport_txt.Text) ||
                string.IsNullOrWhiteSpace(clPassportKemVidan_txt.Text) || string.IsNullOrWhiteSpace(clPostIndex_txt.Text))
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            string phone = clPhone_txt.Text.Trim();
            string passport = clPassport_txt.Text.Trim();
            if (new_client != null)
            {
                var result = Validation.EditClientValidation(phone, passport, new_client);
                if(!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
            }
            else
            {
                var result = Validation.CreateClientValidation(phone, passport);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
                new_client = new Client();
                context.Client.Add(new_client);
            }
            new_client.name = clName_txt.Text.Trim();
            new_client.surname = clSurname_txt.Text.Trim();
            new_client.otchestvo = clOtchestvo_txt.Text.Trim();
            new_client.address = clAddress_txt.Text.Trim();
            new_client.phone = phone;
            new_client.passport_seria_number = passport;
            new_client.kem_vidan_passport = clPassportKemVidan_txt.Text.Trim();
            new_client.post_index = clPostIndex_txt.Text.Trim();
            context.SaveChanges();
            _parentWindow.RefreshData();
            EdDelRep newrep = new EdDelRep();
            newrep.user_id = _parentWindow.current_emp.id;
            if (Create_btn.Content.ToString() == "Сохранить") { newrep.deistv_name = "отредактировал"; }
            else { newrep.deistv_name = "добавил"; }
            newrep.table_name = "клиента";
            newrep.date_update = DateTime.Now;
            context.EdDelRep.Add(newrep);
            context.SaveChanges();
            _parentWindow.RefreshData();
            this.Close();
        }
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == clPhone_txt)
            {
                foreach (char c in e.Text)
                {
                    if (!Char.IsDigit(c))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (textBox == clPassport_txt)
            {
                foreach (char c in e.Text)
                {
                    if (!Char.IsDigit(c))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (textBox == clPostIndex_txt)
            {
                foreach (char c in e.Text)
                {
                    if (!Char.IsDigit(c))
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }
    }
}
