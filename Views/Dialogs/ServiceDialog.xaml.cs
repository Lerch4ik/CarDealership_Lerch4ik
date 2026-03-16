using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Dialogs
{
    public class CarSimpleItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = "";
        public override string ToString() => DisplayName;
    }

    public partial class ServiceDialog : Window
    {
        public ServiceDialog()
        {
            InitializeComponent();

            using var ctx = new CarDealershipContext();
            CmbCar.ItemsSource = ctx.Cars
                .ToList()
                .Select(c => new CarSimpleItem
                {
                    Id = c.Id,
                    DisplayName = $"{c.Brand} {c.Model} {c.Year} — {c.VIN}"
                })
                .ToList();
            CmbCustomer.ItemsSource = ctx.Customers.OrderBy(c => c.LastName).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCar.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new CarDealershipContext();
            var carId = ((CarSimpleItem)CmbCar.SelectedItem).Id;

            var sr = new ServiceRecord
            {
                CarId = carId,
                CustomerId = CmbCustomer.SelectedItem is Customer c ? c.Id : null,
                ServiceType = (CmbType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "ТО",
                Description = TxtDesc.Text.Trim(),
                Cost = decimal.TryParse(TxtCost.Text, out decimal cost) ? cost : 0,
                TechnicianName = TxtTech.Text.Trim(),
                Status = (CmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "In Progress"
            };

            ctx.ServiceRecords.Add(sr);
            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
