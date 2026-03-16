using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Dialogs
{
    public partial class ManagerEditDialog : Window
    {
        private readonly int? _id;

        public ManagerEditDialog(int? id)
        {
            InitializeComponent();
            _id = id;
            if (id.HasValue)
            {
                TxtTitle.Text = "Редактировать менеджера";
                BtnSave.Content = "Обновить";
                using var ctx = new CarDealershipContext();
                var m = ctx.Managers.Find(id);
                if (m != null)
                {
                    TxtLastName.Text = m.LastName;
                    TxtFirstName.Text = m.FirstName;
                    TxtPosition.Text = m.Position;
                    TxtPhone.Text = m.Phone;
                    TxtEmail.Text = m.Email;
                    CmbActive.SelectedIndex = m.IsActive ? 0 : 1;
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
            Manager m;
            if (_id.HasValue) { m = ctx.Managers.Find(_id.Value)!; }
            else { m = new Manager(); ctx.Managers.Add(m); }

            m.LastName = TxtLastName.Text.Trim();
            m.FirstName = TxtFirstName.Text.Trim();
            m.Position = TxtPosition.Text.Trim();
            m.Phone = TxtPhone.Text.Trim();
            m.Email = TxtEmail.Text.Trim();
            m.IsActive = CmbActive.SelectedIndex == 0;

            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
