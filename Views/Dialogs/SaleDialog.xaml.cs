using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Dialogs
{
    // Simple DTO to avoid reflection on anonymous types
    public class CarComboItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = "";
        public decimal Price { get; set; }
        public override string ToString() => DisplayName;
    }

    public partial class SaleDialog : Window
    {
        public SaleDialog()
        {
            InitializeComponent();
            LoadComboData();
        }

        private void LoadComboData()
        {
            using var ctx = new CarDealershipContext();

            var availableCars = ctx.Cars
                .Where(c => c.Status == "Available" || c.Status == "Reserved")
                .ToList()
                .Select(c => new CarComboItem
                {
                    Id = c.Id,
                    DisplayName = $"{c.Brand} {c.Model} {c.Year} — {c.Price:N0} \u20bd",
                    Price = c.Price
                })
                .ToList();
            CmbCar.ItemsSource = availableCars;

            CmbCustomer.ItemsSource = ctx.Customers.OrderBy(c => c.LastName).ToList();
            CmbManager.ItemsSource = ctx.Managers.Where(m => m.IsActive).OrderBy(m => m.LastName).ToList();
        }

        private void CmbCar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCar.SelectedItem is CarComboItem car)
            {
                TxtPrice.Text = car.Price.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCar.SelectedItem == null || CmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль и клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new CarDealershipContext();

            var carId = ((CarComboItem)CmbCar.SelectedItem).Id;
            var car = ctx.Cars.Find(carId);
            if (car != null) car.Status = "Sold";

            var sale = new Sale
            {
                CarId = carId,
                CustomerId = ((Customer)CmbCustomer.SelectedItem).Id,
                ManagerId = CmbManager.SelectedItem is Manager m ? m.Id : null,
                SalePrice = price,
                Discount = decimal.TryParse(TxtDiscount.Text, out decimal disc) ? disc : 0,
                PaymentMethod = (CmbPayment.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Наличные",
                Status = (CmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Completed",
                Notes = TxtNotes.Text.Trim()
            };

            ctx.Sales.Add(sale);
            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
