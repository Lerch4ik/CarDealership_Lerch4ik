using System;
using System.Linq;
using System.Windows.Controls;
using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        private void LoadData()
        {
            var hour = DateTime.Now.Hour;
            TxtGreeting.Text = hour < 12 ? "Доброе утро! Вот сводка на сегодня."
                             : hour < 18 ? "Добрый день! Вот сводка на сегодня."
                             : "Добрый вечер! Вот сводка на сегодня.";

            using var ctx = new CarDealershipContext();

            var cars = ctx.Cars.ToList();
            TxtTotalCars.Text = cars.Count.ToString();
            TxtAvailableCars.Text = $"{cars.Count(c => c.Status == "Available")} доступных";
            TxtStatusAvailable.Text = cars.Count(c => c.Status == "Available").ToString();
            TxtStatusReserved.Text = cars.Count(c => c.Status == "Reserved").ToString();
            TxtStatusSold.Text = cars.Count(c => c.Status == "Sold").ToString();

            TxtTotalCustomers.Text = ctx.Customers.Count().ToString();

            var now = DateTime.Now;
            var monthlySales = ctx.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .Where(s => s.SaleDate.Month == now.Month && s.SaleDate.Year == now.Year)
                .ToList();
            TxtMonthlySales.Text = monthlySales.Count.ToString();
            TxtMonthlyRevenue.Text = $"₽ {monthlySales.Sum(s => s.SalePrice):N0}";

            TxtActiveService.Text = ctx.ServiceRecords.Count(sr => sr.Status == "In Progress").ToString();

            // Recent Sales — ToList() before Select to avoid EF translating computed FullName property
            var recentSales = ctx.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToList()
                .Select(s => new
                {
                    s.SaleDate,
                    CarName = s.Car != null ? $"{s.Car.Brand} {s.Car.Model}" : "—",
                    CustomerName = s.Customer != null ? s.Customer.FullName : "—",
                    s.SalePrice
                })
                .ToList();
            GridRecentSales.ItemsSource = recentSales;

            // Top Cars — ToList() first to avoid decimal ORDER BY issue in SQLite
            var topCars = ctx.Cars
                .ToList()
                .OrderByDescending(c => c.Price)
                .Take(8)
                .Select(c => new
                {
                    c.Id,
                    BrandModel = $"{c.Brand} {c.Model}",
                    c.Year,
                    c.Color,
                    c.FuelType,
                    c.Price,
                    c.Status
                })
                .ToList();
            GridTopCars.ItemsSource = topCars;
        }
    }
}
