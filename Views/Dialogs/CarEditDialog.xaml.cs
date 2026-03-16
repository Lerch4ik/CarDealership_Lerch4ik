using System;
using System.Linq;
using System.Windows;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Dialogs
{
    public partial class CarEditDialog : Window
    {
        private readonly int? _carId;

        public CarEditDialog(int? carId)
        {
            InitializeComponent();
            _carId = carId;

            if (carId.HasValue)
            {
                TxtTitle.Text = "Редактировать автомобиль";
                BtnSave.Content = "Обновить";
                LoadCar(carId.Value);
            }
        }

        private void LoadCar(int id)
        {
            using var ctx = new CarDealershipContext();
            var car = ctx.Cars.Find(id);
            if (car == null) return;

            TxtBrand.Text = car.Brand;
            TxtModel.Text = car.Model;
            TxtYear.Text = car.Year.ToString();
            TxtColor.Text = car.Color;
            TxtPrice.Text = car.Price.ToString();
            TxtMileage.Text = car.Mileage.ToString();
            TxtEngineVolume.Text = car.EngineVolume;
            TxtVIN.Text = car.VIN;
            TxtDescription.Text = car.Description;

            SetComboItem(CmbFuelType, car.FuelType);
            SetComboItem(CmbTransmission, car.Transmission);
            SetComboItem(CmbBodyType, car.BodyType);
            SetComboItem(CmbStatus, car.Status);
        }

        private void SetComboItem(System.Windows.Controls.ComboBox cmb, string value)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in cmb.Items)
            {
                if (item.Content?.ToString() == value)
                {
                    cmb.SelectedItem = item;
                    return;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBrand.Text) || string.IsNullOrWhiteSpace(TxtModel.Text))
            {
                MessageBox.Show("Марка и модель обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Введите корректный год.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text.Replace(" ", ""), out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var ctx = new CarDealershipContext();

            Car car;
            if (_carId.HasValue)
            {
                car = ctx.Cars.Find(_carId.Value)!;
                if (car == null) return;
            }
            else
            {
                car = new Car();
                ctx.Cars.Add(car);
            }

            car.Brand = TxtBrand.Text.Trim();
            car.Model = TxtModel.Text.Trim();
            car.Year = year;
            car.Color = TxtColor.Text.Trim();
            car.Price = price;
            car.Mileage = int.TryParse(TxtMileage.Text, out int mileage) ? mileage : 0;
            car.FuelType = (CmbFuelType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Бензин";
            car.Transmission = (CmbTransmission.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Автомат";
            car.BodyType = (CmbBodyType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Седан";
            car.EngineVolume = TxtEngineVolume.Text.Trim();
            car.VIN = TxtVIN.Text.Trim();
            car.Status = (CmbStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Available";
            car.Description = TxtDescription.Text.Trim();

            ctx.SaveChanges();
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
