using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Dialogs
{
    public partial class SaleEditDialog : Window
    {
        private readonly int _saleId;

        public SaleEditDialog(int saleId)
        {
            InitializeComponent();
            _saleId = saleId;

            using var ctx = new CarDealershipContext();
            CmbManager.ItemsSource = ctx.Managers.Where(m => m.IsActive).OrderBy(m => m.LastName).ToList();

            var sale = ctx.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .Include(s => s.Manager)
                .FirstOrDefault(s => s.Id == saleId);

            if (sale == null) { Close(); return; }

            TxtSubtitle.Text = $"#{sale.Id}  ·  {(sale.Car != null ? $"{sale.Car.Brand} {sale.Car.Model}" : "—")}  ·  {(sale.Customer != null ? sale.Customer.FullName : "—")}";
            TxtPrice.Text = sale.SalePrice.ToString();
            TxtDiscount.Text = sale.Discount.ToString();
            TxtNotes.Text = sale.Notes;

            SetComboByContent(CmbPayment, sale.PaymentMethod);
            SetComboByContent(CmbStatus, sale.Status);

            if (sale.ManagerId.HasValue)
            {
                var mgr = CmbManager.Items.Cast<Manager>().FirstOrDefault(m => m.Id == sale.ManagerId);
                CmbManager.SelectedItem = mgr;
            }
        }

        private static void SetComboByContent(ComboBox cmb, string value)
        {
            foreach (ComboBoxItem item in cmb.Items)
                if (item.Content?.ToString() == value) { cmb.SelectedItem = item; return; }
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new CarDealershipContext();
            var sale = ctx.Sales.Find(_saleId);
            if (sale == null) return;

            sale.SalePrice = price;
            sale.Discount = decimal.TryParse(TxtDiscount.Text, out decimal disc) ? disc : 0;
            sale.PaymentMethod = (CmbPayment.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? sale.PaymentMethod;
            sale.Status = (CmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? sale.Status;
            sale.ManagerId = CmbManager.SelectedItem is Manager m ? m.Id : sale.ManagerId;
            sale.Notes = TxtNotes.Text.Trim();

            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
