using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Dialogs
{
    public partial class ServiceEditDialog : Window
    {
        private readonly int _id;

        public ServiceEditDialog(int id)
        {
            InitializeComponent();
            _id = id;

            using var ctx = new CarDealershipContext();
            var customers = ctx.Customers.OrderBy(c => c.LastName).ToList();
            CmbCustomer.ItemsSource = customers;

            var sr = ctx.ServiceRecords
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .FirstOrDefault(s => s.Id == id);

            if (sr == null) { Close(); return; }

            TxtSubtitle.Text = $"#{sr.Id}  ·  {(sr.Car != null ? $"{sr.Car.Brand} {sr.Car.Model}" : "—")}";
            TxtCost.Text = sr.Cost.ToString();
            TxtDesc.Text = sr.Description;
            TxtTech.Text = sr.TechnicianName;

            SetComboByContent(CmbType, sr.ServiceType);
            SetComboByContent(CmbStatus, sr.Status);

            if (sr.CustomerId.HasValue)
                CmbCustomer.SelectedItem = customers.FirstOrDefault(c => c.Id == sr.CustomerId);
        }

        private static void SetComboByContent(ComboBox cmb, string value)
        {
            foreach (ComboBoxItem item in cmb.Items)
                if (item.Content?.ToString() == value) { cmb.SelectedItem = item; return; }
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using var ctx = new CarDealershipContext();
            var sr = ctx.ServiceRecords.Find(_id);
            if (sr == null) return;

            sr.ServiceType = (CmbType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? sr.ServiceType;
            sr.Status = (CmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? sr.Status;
            sr.Cost = decimal.TryParse(TxtCost.Text, out decimal cost) ? cost : sr.Cost;
            sr.Description = TxtDesc.Text.Trim();
            sr.TechnicianName = TxtTech.Text.Trim();
            sr.CustomerId = CmbCustomer.SelectedItem is Customer c ? c.Id : sr.CustomerId;

            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
