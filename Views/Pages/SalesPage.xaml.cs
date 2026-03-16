using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Pages
{
    public partial class SalesPage : Page
    {
        public SalesPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        private void LoadData()
        {
            using var ctx = new CarDealershipContext();
            var query = ctx.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .Include(s => s.Manager)
                .AsQueryable();

            var search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s =>
                    (s.Car != null && (s.Car.Brand.ToLower().Contains(search) || s.Car.Model.ToLower().Contains(search))) ||
                    (s.Customer != null && (s.Customer.LastName.ToLower().Contains(search) || s.Customer.FirstName.ToLower().Contains(search))));

            var sales = query.OrderBy(s => s.Id)  // default: by ID
                .ToList()
                .Select(s => new
                {
                    s.Id,
                    s.SaleDate,
                    CarName = s.Car != null ? $"{s.Car.Brand} {s.Car.Model} ({s.Car.Year})" : "—",
                    CustomerName = s.Customer != null ? s.Customer.FullName : "—",
                    ManagerName = s.Manager != null ? s.Manager.FullName : "—",
                    s.PaymentMethod,
                    s.Discount,
                    s.SalePrice,
                    s.Status
                }).ToList();

            GridSales.ItemsSource = sales;
            TxtCount.Text = $"{sales.Count} сделок";

            if (sales.Any())
            {
                TxtTotalRevenue.Text = $"₽ {sales.Sum(s => s.SalePrice):N0}";
                TxtTotalSales.Text = sales.Count.ToString();
                TxtAvgSale.Text = $"₽ {sales.Average(s => s.SalePrice):N0}";
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();

        private void GridSales_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridSales.SelectedItem is { } item)
            {
                var id = (int)item.GetType().GetProperty("Id")!.GetValue(item)!;
                var dlg = new Dialogs.SaleEditDialog(id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.SaleDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var r = MessageBox.Show("Удалить запись о продаже?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.Yes)
                {
                    using var ctx = new CarDealershipContext();
                    var s = ctx.Sales.Find(id);
                    if (s != null) { ctx.Sales.Remove(s); ctx.SaveChanges(); LoadData(); }
                }
            }
        }
    }
}
