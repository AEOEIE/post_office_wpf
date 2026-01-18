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
    /// Логика взаимодействия для CreateSposobWindow.xaml
    /// </summary>
    public partial class CreateSposobWindow : Window
    {
        private post_officeEntities2 context => ModelDB.GetContext();
        UserWindow _parentWindow;
        Sposob_otpravki new_sposob = null;
        public CreateSposobWindow(UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
        }
        public CreateSposobWindow(Sposob_otpravki spos, UserWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            new_sposob = spos;
            servHeader.Text = "Редактирование способа доставки";
            sposobName_txt.Text = new_sposob.name; sposobPrice_txt.Text = new_sposob.price.ToString();
            sposobOpisanie_txt.Text = new_sposob.opisanie.ToString();
            Create_btn.Content = "Сохранить";
        }
        private void buttonCreateClick(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(sposobPrice_txt.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!");
                return;
            }
            if (string.IsNullOrWhiteSpace(sposobName_txt.Text) || string.IsNullOrWhiteSpace(sposobPrice_txt.Text)
                || decimal.Parse(sposobPrice_txt.Text) <= 0 || string.IsNullOrWhiteSpace(sposobOpisanie_txt.Text))
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            string sposob_name = sposobName_txt.Text.Trim();
            if (new_sposob != null)
            {
                var result = Validation.EditSposobValidation(sposob_name, new_sposob);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
            }
            else
            {
                var result = Validation.CreateSposobValidation(sposob_name);
                if (!result.isValid)
                {
                    MessageBox.Show(result.error);
                    return;
                }
                new_sposob = new Sposob_otpravki();
                context.Sposob_otpravki.Add(new_sposob);
            }
            new_sposob.name = sposobName_txt.Text;
            new_sposob.price = decimal.Parse(sposobPrice_txt.Text);
            new_sposob.opisanie = sposobOpisanie_txt.Text;
            context.SaveChanges();
            _parentWindow.RefreshData();
            EdDelRep newrep = new EdDelRep();
            newrep.user_id = _parentWindow.current_emp.id;
            if (Create_btn.Content.ToString() == "Сохранить") { newrep.deistv_name = "отредактировал"; }
            else { newrep.deistv_name = "добавил"; }
            newrep.table_name = "способ доставки";
            newrep.date_update = DateTime.Now;
            context.EdDelRep.Add(newrep);
            context.SaveChanges();
            _parentWindow.RefreshData();
            this.Close();
        }
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!sposobPrice_txt.Text.Contains(".")
               && sposobPrice_txt.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
    }
}
