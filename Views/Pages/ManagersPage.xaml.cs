using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Pages
{
    public class ManagerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Position { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public System.DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public int SalesCount { get; set; }
    }

    public partial class ManagersPage : Page
    {
        public ManagersPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        private void LoadData()
        {
            using var ctx = new CarDealershipContext();
            var managers = ctx.Managers
                .ToList()
                .Select(m => new ManagerDto
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    Position = m.Position,
                    Phone = m.Phone,
                    Email = m.Email,
                    HireDate = m.HireDate,
                    IsActive = m.IsActive,
                    SalesCount = ctx.Sales.Count(s => s.ManagerId == m.Id)
                })
                .OrderBy(m => m.Id)  // default: by ID
                .ToList();
            GridManagers.ItemsSource = managers;
            TxtCount.Text = $"{managers.Count} менеджеров · {managers.Count(m => m.IsActive)} активных";
        }

        private void GridManagers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridManagers.SelectedItem is ManagerDto dto)
            {
                var dlg = new Dialogs.ManagerEditDialog(dto.Id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.ManagerEditDialog(null);
            if (dlg.ShowDialog() == true) LoadData();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var dlg = new Dialogs.ManagerEditDialog(id);
                if (dlg.ShowDialog() == true) LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var r = MessageBox.Show("Удалить менеджера?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.Yes)
                {
                    using var ctx = new CarDealershipContext();
                    var m = ctx.Managers.Find(id);
                    if (m != null) { ctx.Managers.Remove(m); ctx.SaveChanges(); LoadData(); }
                }
            }
        }
    }
}
