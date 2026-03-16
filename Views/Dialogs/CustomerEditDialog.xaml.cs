using System.Windows;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Dialogs
{
    public partial class CustomerEditDialog : Window
    {
        private readonly int? _id;

        public CustomerEditDialog(int? id)
        {
            InitializeComponent();
            _id = id;
            if (id.HasValue)
            {
                TxtTitle.Text = "Редактировать клиента";
                BtnSave.Content = "Обновить";
                using var ctx = new CarDealershipContext();
                var c = ctx.Customers.Find(id);
                if (c != null)
                {
                    TxtLastName.Text = c.LastName;
                    TxtFirstName.Text = c.FirstName;
                    TxtMiddleName.Text = c.MiddleName;
                    TxtPhone.Text = c.Phone;
                    TxtEmail.Text = c.Email;
                    TxtPassport.Text = c.PassportNumber;
                    TxtAddress.Text = c.Address;
                    TxtNotes.Text = c.Notes;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) || string.IsNullOrWhiteSpace(TxtFirstName.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new CarDealershipContext();
            Customer c;
            if (_id.HasValue) { c = ctx.Customers.Find(_id.Value)!; }
            else { c = new Customer(); ctx.Customers.Add(c); }

            c.LastName = TxtLastName.Text.Trim();
            c.FirstName = TxtFirstName.Text.Trim();
            c.MiddleName = TxtMiddleName.Text.Trim();
            c.Phone = TxtPhone.Text.Trim();
            c.Email = TxtEmail.Text.Trim();
            c.PassportNumber = TxtPassport.Text.Trim();
            c.Address = TxtAddress.Text.Trim();
            c.Notes = TxtNotes.Text.Trim();

            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
