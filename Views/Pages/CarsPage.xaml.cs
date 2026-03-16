using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Data;
using CarDealership.Models;

namespace CarDealership.Views.Pages
{
    public partial class CarsPage : Page
    {
        private bool _loaded;

        public CarsPage()
        {
            InitializeComponent();
            Loaded += (_, _) => { _loaded = true; LoadCars(); };
        }

        private void LoadCars()
        {
            if (!_loaded) return;

            using var ctx = new CarDealershipContext();

            var query = ctx.Cars.AsQueryable();

            var search = TxtSearch?.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Brand.ToLower().Contains(search)
                                      || c.Model.ToLower().Contains(search)
                                      || c.VIN.ToLower().Contains(search)
                                      || c.Color.ToLower().Contains(search));

            var statusVal = (CmbStatus?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(statusVal) && statusVal != "Все статусы")
                query = query.Where(c => c.Status == statusVal);

            var fuelVal = (CmbFuelType?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(fuelVal) && fuelVal != "Все типы топлива")
                query = query.Where(c => c.FuelType == fuelVal);

            var cars = query.ToList();

            var sort = (CmbSort?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            cars = sort switch
            {
                "По цене ↓" => cars.OrderByDescending(c => c.Price).ToList(),
                "По цене ↑" => cars.OrderBy(c => c.Price).ToList(),
                "По году ↓" => cars.OrderByDescending(c => c.Year).ToList(),
                "По марке"  => cars.OrderBy(c => c.Brand).ToList(),
                _           => cars.OrderBy(c => c.Id).ToList()  // default: by ID
            };

            GridCars.ItemsSource = cars;
            TxtCarCount.Text = $"{cars.Count} автомобилей в базе · {cars.Count(c => c.Status == "Available")} доступных";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadCars();
        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => LoadCars();

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            CmbStatus.SelectedIndex = 0;
            CmbFuelType.SelectedIndex = 0;
            CmbSort.SelectedIndex = 0;
            LoadCars();
        }

        private void BtnAddCar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Dialogs.CarEditDialog(null);
            if (dlg.ShowDialog() == true)
                LoadCars();
        }

        private void GridCars_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void GridCars_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (GridCars.SelectedItem is Car car)
            {
                var dlg = new Dialogs.CarEditDialog(car.Id);
                if (dlg.ShowDialog() == true)
                    LoadCars();
            }
        }

        private void BtnEditCar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var dlg = new Dialogs.CarEditDialog(id);
                if (dlg.ShowDialog() == true)
                    LoadCars();
            }
        }

        private void BtnDeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var result = MessageBox.Show("Удалить автомобиль? Это действие необратимо.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using var ctx = new CarDealershipContext();
                    var car = ctx.Cars.Find(id);
                    if (car != null)
                    {
                        ctx.Cars.Remove(car);
                        ctx.SaveChanges();
                        LoadCars();
                    }
                }
            }
        }
    }
}
