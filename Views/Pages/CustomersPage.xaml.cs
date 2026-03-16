using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Pages
{
    public partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        private void LoadData()
        {
            using var ctx = new CarDealershipContext();
            var query = ctx.Customers.AsQueryable();
            var search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.LastName.ToLower().Contains(search)
                                      || c.FirstName.ToLower().Contains(search)
                                      || c.Phone.Contains(search)
                                      || c.Email.ToLower().Contains(search));
            var list = query.OrderBy(c => c.Id).ToList();  // default: by ID
            GridCustomers.ItemsSource = list;
            TxtCount.Text = $"{list.Count} клиентов в базе";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
        private void BtnReset_Click(object sender, RoutedEventArgs e) { TxtSearch.Text = ""; LoadData(); }

        private void GridCustomers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridCustomers.SelectedItem is Models.Customer c)
            {
                var dlg = new Dialogs.CustomerEditDialog(c.Id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.CustomerEditDialog(null);
            if (dlg.ShowDialog() == true) LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var dlg = new Dialogs.CustomerEditDialog(id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var r = MessageBox.Show("Удалить клиента?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.Yes)
                {
                    using var ctx = new CarDealershipContext();
                    var c = ctx.Customers.Find(id);
                    if (c != null) { ctx.Customers.Remove(c); ctx.SaveChanges(); LoadData(); }
                }
            }
        }
    }
}
