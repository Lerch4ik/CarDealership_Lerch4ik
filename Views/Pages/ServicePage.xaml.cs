using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Pages
{
    public partial class ServicePage : Page
    {
        private bool _loaded;

        public ServicePage()
        {
            InitializeComponent();
            Loaded += (_, _) => { _loaded = true; LoadData(); };
        }

        private void LoadData()
        {
            if (!_loaded) return;

            using var ctx = new CarDealershipContext();
            var query = ctx.ServiceRecords
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .AsQueryable();

            var search = TxtSearch?.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s =>
                    (s.Car != null && (s.Car.Brand.ToLower().Contains(search) || s.Car.Model.ToLower().Contains(search))) ||
                    s.Description.ToLower().Contains(search) ||
                    s.TechnicianName.ToLower().Contains(search));

            var statusVal = (CmbStatus?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(statusVal) && statusVal != "Все статусы")
                query = query.Where(s => s.Status == statusVal);

            var records = query.OrderBy(s => s.Id)  // default: by ID
                .ToList()
                .Select(s => new
                {
                    s.Id,
                    s.ServiceDate,
                    CarName = s.Car != null ? $"{s.Car.Brand} {s.Car.Model}" : "—",
                    CustomerName = s.Customer != null ? s.Customer.FullName : "—",
                    s.ServiceType,
                    s.Description,
                    s.TechnicianName,
                    s.Cost,
                    s.Status
                }).ToList();

            GridService.ItemsSource = records;
            TxtCount.Text = $"{records.Count} записей · {records.Count(r => r.Status == "In Progress")} в работе";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => LoadData();

        private void GridService_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridService.SelectedItem is { } item)
            {
                var id = (int)item.GetType().GetProperty("Id")!.GetValue(item)!;
                var dlg = new Dialogs.ServiceEditDialog(id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.ServiceDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var r = MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.Yes)
                {
                    using var ctx = new CarDealershipContext();
                    var s = ctx.ServiceRecords.Find(id);
                    if (s != null) { ctx.ServiceRecords.Remove(s); ctx.SaveChanges(); LoadData(); }
                }
            }
        }
    }
}
