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
using System.Data.Entity;

namespace kurs4v28
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private post_officeEntities2 context => ModelDB.GetContext();
        internal employee current_emp;
        private List<services> servicess;
        private List<Client> clientss;
        private List<post_otpravlenie> post_otpravleniee;
        private post_otpravlenie current_otpr;
        private List<Sposob_otpravki> sposobb;
        public UserWindow(employee emp)
        {
            InitializeComponent();
            current_emp = emp;
            title_user.Text += " " + current_emp.emp_name + " " + current_emp.emp_surname;
            if (emp.Role.role_name == "operator"|| emp.Role.role_name =="branch_manager")
            {
                ServicesData.ContextMenu = null;
                SposobData.ContextMenu = null;
            }
            if(emp.Role.role_name == "service_manager") { ServicesTab.IsSelected = true; }

            VisibleIfRole(emp);
            //тут услуги
            servicess = context.services.Where(s => s.isDeleted!=true).ToList();
            ServicesData.ItemsSource = servicess;
            //тоже для service, еще одна его вкладочка:)
            sposobb = context.Sposob_otpravki.Where(s => s.isDeleted != true).ToList();
            SposobData.ItemsSource = sposobb;
            //клиенты
            clientss = context.Client.Where(s => s.isDeleted != true).ToList();
            clientsData.ItemsSource = clientss;
            //почтовое отправление
            post_otpravleniee = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki).Where(s => s.isDeleted != true).ToList();
            otprData.ItemsSource = post_otpravleniee;
            //combobox
            PostService_txt.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            var clients = clientss.Select(cl => new { cl.id, FullName = $"{cl.surname} {cl.name} {cl.otchestvo}" }).ToList();
            PostClient_txt.ItemsSource = clients;
            PostSposob_txt.ItemsSource = context.Sposob_otpravki.Where(s => s.isDeleted != true).ToList();
            cbSotrudnik.ItemsSource = context.employee.Where(s => s.isDeleted != true).ToList();
            cbServices.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            //итоговая цена
            PostWeight_txt.TextChanged += (s, e) => CalculatePrice();
            PostSize_txt.TextChanged += (s, e) => CalculatePrice();
            PostService_txt.SelectionChanged += (s, e) => CalculatePrice();
            PostSposob_txt.SelectionChanged += (s, e) => CalculatePrice();
            //на всякий случай
            cbSotrudnik.ItemsSource = context.employee.Where(s => s.isDeleted != true).ToList();
            cbServices.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            PostService_txt.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            PostSposob_txt.ItemsSource = context.Sposob_otpravki.Where(s => s.isDeleted != true).ToList();

        }
        private ContextMenu CreateServicesContextMenu() 
        {
            RefreshData();
            var menu = new ContextMenu();
            menu.FontFamily = new FontFamily("Comic Sans MS");
            var addItem = new MenuItem() { Header = "Добавить" };
            addItem.Click += buttonAddServiceClick;
            var editItem = new MenuItem() { Header = "Редактировать" };
            editItem.Click += buttonEditServiceClick;
            var deleteItem = new MenuItem() { Header = "Удалить" };
            deleteItem.Click += buttonDeleteServiceClick;
            menu.Items.Add(addItem);
            menu.Items.Add(editItem);
            menu.Items.Add(deleteItem);
            return menu;
        }
        private ContextMenu CreateSposobContextMenu()
        {
            RefreshData();
            var menu = new ContextMenu();
            menu.FontFamily = new FontFamily("Comic Sans MS");
            var addItem = new MenuItem() { Header = "Добавить" };
            addItem.Click += buttonAddSposobClick;
            var editItem = new MenuItem() { Header = "Редактировать" };
            editItem.Click += buttonEditSposobClick;
            var deleteItem = new MenuItem() { Header = "Удалить" };
            deleteItem.Click += buttonDeleteSposobClick;
            menu.Items.Add(addItem);
            menu.Items.Add(editItem);
            menu.Items.Add(deleteItem);
            return menu;
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
                MainWindow mv = new MainWindow()
                {
                    WindowStartupLocation = WindowStartupLocation,
                    Left = Left,
                    Top = Top
                };
                mv.Show();
                this.Close();
            }
        }
        public void VisibleIfRole(employee empl)
        {
            if (empl.Role.role_name == "service_manager")
            {
                MainTab.Visibility = Visibility.Collapsed;
                AllPostOtpr.Visibility = Visibility.Collapsed;
                ClientsTab.Visibility = Visibility.Collapsed;
            }
            if (empl.Role.role_name == "operator" || empl.Role.role_name == "branch_manager")
            {
                ServicesData.ContextMenu = null;
            }
            else
            {
                ServicesData.ContextMenu = CreateServicesContextMenu();
                SposobData.ContextMenu = CreateSposobContextMenu();
            }                                                           
            if(empl.Role.role_name == "branch_manager")
            {
                otprData.ContextMenu = null;
                ServicesData.ContextMenu = null;
                clientsData.ContextMenu = null;
                SposobData.ContextMenu= null;
            }
        }
        public void RefreshData()
        {
            servicess = context.services.Where(s => s.isDeleted!=true).ToList();
            ServicesData.ItemsSource = servicess;
            ServicesData.Items.Refresh();
            clientss = context.Client.Where(s => s.isDeleted != true).ToList();
            clientsData.ItemsSource = clientss;
            clientsData.Items.Refresh();
            sposobb = context.Sposob_otpravki.Where(s => s.isDeleted != true).ToList();
            SposobData.ItemsSource = sposobb;
            SposobData.Items.Refresh();
            post_otpravleniee = context.post_otpravlenie.Where(s => s.isDeleted != true).ToList();
            otprData.ItemsSource = post_otpravleniee;
            otprData.Items.Refresh();
            //обновление списков
            PostService_txt.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            var clients = clientss.Select(cl => new { cl.id, FullName = $"{cl.surname} {cl.name} {cl.otchestvo}" }).ToList();
            PostClient_txt.ItemsSource = clients;
            PostSposob_txt.ItemsSource = context.Sposob_otpravki.Where(s => s.isDeleted != true).ToList();
            cbSotrudnik.ItemsSource = context.employee.Where(s => s.isDeleted != true).ToList();
            cbServices.ItemsSource = context.services.Where(s => s.isDeleted != true).ToList();
            PostClient_txt.Items.Refresh();
            PostService_txt.Items.Refresh();
        }
        //Для service
        private void buttonAddServiceClick(object sender, RoutedEventArgs e)
        {
            var csw = new CreateServiceWindow(this);
            csw.ShowDialog();
        }
        private void buttonDeleteServiceClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = ServicesData.SelectedItem as dynamic;
            if (selectedItem == null)
            {
                return;
            }
            string del = selectedItem.name;
            services deleted = context.services.FirstOrDefault(de => de.name == del);
            var userService = context.post_otpravlenie.FirstOrDefault(d => d.service_id == deleted.id);
            MessageBoxResult result = MessageBox.Show(
              "Вы действительно хотите удалить эту услугу?",
              "Подтверждение выхода",
              MessageBoxButton.YesNo,
              MessageBoxImage.Question
           );
            if (result == MessageBoxResult.Yes)
            {
                //context.services.Remove(selectedItem);
                selectedItem.isDeleted = true;
                context.SaveChanges();
                MessageBox.Show("Услуга удалена");
                RefreshData();
                EdDelRep newrep = new EdDelRep();
                newrep.user_id = current_emp.id;
                newrep.deistv_name = "удалил";
                newrep.table_name = "услугу";
                newrep.date_update = DateTime.Now;
                context.EdDelRep.Add(newrep);
                context.SaveChanges();
            }
        }

        private void buttonEditServiceClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = ServicesData.SelectedItem as dynamic;
            if (selectedItem == null)
            {
                return;
            }
            string edit = selectedItem.name;
            services edited = context.services.FirstOrDefault(de => de.name == edit && de.isDeleted !=true);
            CreateServiceWindow csw = new CreateServiceWindow(edited, this);
            csw.ShowDialog();
        }
        private void textChangedSearch(object sender, TextChangedEventArgs e)
        {
            List<services> serchServ = context.services.Where(s => s.name.Contains(searchService.Text)).Where(s => s.isDeleted != true).ToList();
            ServicesData.ItemsSource = serchServ;
        }
        //service, но для способов доставки
        private void buttonAddSposobClick(object sender, RoutedEventArgs e)
        {
            var cspw = new CreateSposobWindow(this);
            cspw.ShowDialog();
        }
        private void buttonEditSposobClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = SposobData.SelectedItem as dynamic;
            if (selectedItem == null)
            {
                return;
            }
            string edit = selectedItem.name;
            Sposob_otpravki edited = context.Sposob_otpravki.FirstOrDefault(de => de.name == edit && de.isDeleted != true);
            CreateSposobWindow csw = new CreateSposobWindow(edited, this);
            csw.ShowDialog();
        }
        private void buttonDeleteSposobClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = SposobData.SelectedItem as dynamic;
            if (selectedItem == null)
            {
                return;
            }
            string del = selectedItem.name;
            Sposob_otpravki deleted = context.Sposob_otpravki.FirstOrDefault(de => de.name == del);
            var userSposob = context.post_otpravlenie.FirstOrDefault(d => d.sposob_otprki_id == deleted.id);
            MessageBoxResult result = MessageBox.Show(
               "Вы действительно хотите удалить этот способ доставки?",
               "Подтверждение выхода",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question
            );
            if (result == MessageBoxResult.Yes)
            {
                selectedItem.isDeleted = true;
                context.SaveChanges();
                MessageBox.Show("Способ доставки удален");
                RefreshData();
                EdDelRep newrep = new EdDelRep();
                newrep.user_id = current_emp.id;
                newrep.deistv_name = "удалил";
                newrep.table_name = "способ доставки";
                newrep.date_update = DateTime.Now;
                context.EdDelRep.Add(newrep);
                context.SaveChanges();
            }
        }
        private void searchSposob_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Sposob_otpravki> searchSposobList = context.Sposob_otpravki
                .Where(s => s.name.Contains(searchSposob.Text)).Where(s => s.isDeleted != true).ToList();
            SposobData.ItemsSource = searchSposobList;
        }
        //для клиентов
        private void buttonAddClientClick(object sender, RoutedEventArgs e)
        {
            CreateClientWindow ccw = new CreateClientWindow(this);
            ccw.ShowDialog();
            RefreshData();
        }
        private void buttonDeleteClientClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = clientsData.SelectedItem as dynamic;
            if (selectedItem == null) { return; }
            string del = selectedItem.phone;
            Client deleted = context.Client.FirstOrDefault(de => de.phone == del);
            var OtprClient = context.post_otpravlenie.FirstOrDefault(c => c.client_id == deleted.id);
            MessageBoxResult res = MessageBox.Show("Вы дейстивительно хотите удалить этого клиента?",
                    "Подтверждение выхода",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                deleted.isDeleted = true;
                context.SaveChanges();
                MessageBox.Show("Клиент удален");
                EdDelRep newrep = new EdDelRep();
                newrep.user_id = current_emp.id;
                newrep.deistv_name = "удалил";
                newrep.table_name = "клиента";
                newrep.date_update = DateTime.Now;
                context.EdDelRep.Add(newrep);
                context.SaveChanges();
                RefreshData();
            }
        }
        private void buttonEditClientClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = clientsData.SelectedItem as dynamic;
            if (selectedItem == null) { return; }
            string edit = selectedItem.phone;
            Client cl = context.Client.FirstOrDefault(ed => ed.phone == edit && ed.isDeleted != true);
            CreateClientWindow ccw = new CreateClientWindow(cl, this);
            ccw.ShowDialog();
        }
        private string _currentFilterSurname = null;
        private void ViewOtprClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = clientsData.SelectedItem as dynamic;
            if (selectedItem == null) { return; }
            string edit = selectedItem.surname;
            _currentFilterSurname = edit;

            otprData.ItemsSource = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki)
                .Where(pos => pos.Client.surname == edit).Where(s => s.isDeleted != true).ToList();
            AllPostOtpr.IsSelected = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Contains(AllPostOtpr) && _currentFilterSurname != null)
            {
                _currentFilterSurname = null;
                RefreshOtprData(); 
            }
        }
        private void RefreshOtprData()
        {
            otprData.ItemsSource = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki).Where(s => s.isDeleted != true)
                .ToList();
        }
        private void searchClientTextChanged(object sender, TextChangedEventArgs e)
        {
            List<Client> searchCl = context.Client.Where(cl => cl.phone.Contains(searchClient.Text) || cl.passport_seria_number.Contains(searchClient.Text)|| cl.surname.Contains(searchClient.Text)).Where(s => s.isDeleted != true).ToList();
            clientsData.ItemsSource = searchCl;
        }
        //почтовое отправление и все почтовые отправления
        private void PostClient_txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PostClient_txt.SelectedItem == null)
            {
                PostClient_txt.Text = "";
            }
        }
        private void buttonOformitClick(object sender, RoutedEventArgs e)
        {
            if (PostDate_txt.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату доставки!");
                return;
            }
            DateTime deliveryDate = PostDate_txt.SelectedDate.Value;
            if (deliveryDate < DateTime.Now)
            {
                MessageBox.Show("Вы не можете выбрать эту дату доставки!");
                return;
            }
            if (string.IsNullOrWhiteSpace(PostClient_txt.Text) || string.IsNullOrWhiteSpace(PostSize_txt.Text) ||
                string.IsNullOrWhiteSpace(PostWeight_txt.Text) || string.IsNullOrWhiteSpace(PostSposob_txt.Text) ||
                string.IsNullOrWhiteSpace(PostService_txt.Text) || string.IsNullOrWhiteSpace(PostAddress_txt.Text) ||
                string.IsNullOrWhiteSpace(PostIndex_txt.Text)
                )
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            if (!decimal.TryParse(PostWeight_txt.Text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal weight) || weight < 0)
            {
                MessageBox.Show("Введите корректный вес! Используйте точку для дробей: 1.5");
                return;
            }
            if (!decimal.TryParse(PostSize_txt.Text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal size) || size < 0)
            {
                MessageBox.Show("Введите корректный объем! Используйте точку для дробей: 1.5");
                return;
            }
            current_otpr = new post_otpravlenie();
            if (PostClient_txt.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента из списка!");
                return;
            }
            var selectedClient = PostClient_txt.SelectedItem as dynamic;
            current_otpr.client_id = selectedClient.id;
            current_otpr.emp_id = current_emp.id;
            services post_service = new services();
            post_service = context.services.FirstOrDefault(i => i.name == PostService_txt.Text && i.isDeleted != true);
            current_otpr.service_id = post_service.id;
            current_otpr.weight = weight;
            current_otpr.size = size;
            current_otpr.index_dostavki = PostIndex_txt.Text;
            current_otpr.date_dostavki = deliveryDate;
            Sposob_otpravki post_sposob = new Sposob_otpravki();
            post_sposob = context.Sposob_otpravki.FirstOrDefault(i => i.name == PostSposob_txt.Text && i.isDeleted != true);
            current_otpr.sposob_otprki_id = post_sposob.id;
            current_otpr.full_price = ((weight * 100) + (size * 10) + post_sposob.price + post_service.price);
            current_otpr.address_dostavki = PostAddress_txt.Text;
            context.post_otpravlenie.Add(current_otpr);
            context.SaveChanges();
            RefreshData();
            ClearMainTabFields();
            AllPostOtpr.IsSelected = true;
            EdDelRep newrep = new EdDelRep();
            newrep.user_id = current_emp.id;
            newrep.deistv_name = "добавил";
            newrep.table_name = "почтовое отправление";
            newrep.date_update = DateTime.Now;
            context.EdDelRep.Add(newrep);
            context.SaveChanges();
        }
        private void buttonOtprAddClick(object sender, RoutedEventArgs e)
        {
            MainTab.IsSelected = true;
            ClearMainTabFields();
        }
        private void buttonOtprDeleteClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = otprData.SelectedItem as post_otpravlenie;
            MessageBoxResult res = MessageBox.Show("Вы дейстивительно хотите удалить это почтовое отправление?",
                    "Подтверждение выхода",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                selectedItem.isDeleted = true;
                context.SaveChanges();
                MessageBox.Show("Почтовое отправление удалено");
                RefreshData();
                EdDelRep newrep = new EdDelRep();
                newrep.user_id = current_emp.id;
                newrep.deistv_name = "удалил";
                newrep.table_name = "почтовое отправление";
                newrep.date_update = DateTime.Now;
                context.EdDelRep.Add(newrep);
                context.SaveChanges();
            }
        }
        private void PostWeight_txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == "." && !textBox.Text.Contains("."))))
            {
                e.Handled = true;
            }
        }
        private void PostSize_txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == "." && !textBox.Text.Contains("."))))
            {
                e.Handled = true;
            }
        }
        private void PostIndex_txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void CalculatePrice()
        {
            decimal? price = 0;
            if (decimal.TryParse(PostWeight_txt.Text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal weight))
            {
                price += weight * 100;
            }
            if (decimal.TryParse(PostSize_txt.Text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal size))
            {
                price += size * 10;
            }
            if (PostService_txt.SelectedItem is services selectedService)
                price += selectedService.price;
            if (PostSposob_txt.SelectedItem is Sposob_otpravki selectedSposob)
                price += selectedSposob.price;
            ItogPrice_txt.Text = $"Итоговая цена: {price} руб.";
        }

        private void ClearMainTabFields()
        {
            PostClient_txt.SelectedIndex = -1;
            PostClient_txt.Text = "";
            PostSize_txt.Text = "";
            PostWeight_txt.Text = "";
            PostSposob_txt.SelectedIndex = -1;
            PostService_txt.SelectedIndex = -1;
            PostAddress_txt.Text = "";
            PostIndex_txt.Text = "";
            PostDate_txt.SelectedDate = null;
            ItogPrice_txt.Text = "Итоговая цена: 0 руб.";
        }
        private void comboboxEmpClick(object sender, SelectionChangedEventArgs e)
        {
            if (cbSotrudnik.SelectedItem is employee selected)
            {
                post_otpravleniee = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki)
                .Where(e1 => e1.employee.id == selected.id).Where(s => s.isDeleted !=true)
                    .ToList();
                otprData.ItemsSource = post_otpravleniee;
                otprData.Items.Refresh();
            }
            else
            {
                post_otpravleniee = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki).Where(s => s.isDeleted != true).ToList();
                otprData.ItemsSource = post_otpravleniee;
                otprData.Items.Refresh();
            }
        }
        private void comboboxServClick(object sender, SelectionChangedEventArgs e)
        {
            if(cbServices.SelectedItem is services selected)
            {
                post_otpravleniee = context.post_otpravlenie.Include(pos => pos.Client)
               .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki)
               .Where(e1 => e1.services.id == selected.id).Where(s => s.isDeleted != true).ToList();
                otprData.ItemsSource = post_otpravleniee;
                otprData.Items.Refresh();
            }
            else
            {
                post_otpravleniee = context.post_otpravlenie.Include(pos => pos.Client)
                .Include(pos => pos.employee).Include(pos => pos.services).Include(pos => pos.Sposob_otpravki).Where(s => s.isDeleted != true).ToList();
                otprData.ItemsSource = post_otpravleniee;
                otprData.Items.Refresh();
            }
        }
    }
}
