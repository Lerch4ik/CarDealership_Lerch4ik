using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CarDealership.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealership.Views.Pages
{
    public partial class ReportsPage : Page
    {
        private string _period = "month"; // month, quarter, year, all

        public ReportsPage()
        {
            InitializeComponent();
            Loaded += (_, _) => LoadData();
        }

        private void BtnPeriod_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            _period = btn.Tag?.ToString() ?? "month";

            // Update button styles
            var accent = (Brush)FindResource("AccentBrush");
            var transparent = Brushes.Transparent;
            var secondary = (Brush)FindResource("TextSecondaryBrush");
            var white = Brushes.White;

            foreach (var b in new[] { BtnPeriod1, BtnPeriod2, BtnPeriod3, BtnPeriod4 })
            {
                bool active = b.Tag?.ToString() == _period;
                b.Background = active ? accent : transparent;
                b.Foreground = active ? white : secondary;
                b.FontWeight = active ? FontWeights.SemiBold : FontWeights.Normal;
            }

            LoadData();
        }

        private (DateTime from, DateTime to) GetPeriodRange()
        {
            var now = DateTime.Now;
            return _period switch
            {
                "month"   => (new DateTime(now.Year, now.Month, 1), now),
                "quarter" => (new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1), now),
                "year"    => (new DateTime(now.Year, 1, 1), now),
                _         => (DateTime.MinValue, now)
            };
        }

        private string PeriodLabel() => _period switch
        {
            "month"   => $"За {DateTime.Now:MMMM yyyy}",
            "quarter" => $"За {DateTime.Now.Month switch { <= 3 => "I", <= 6 => "II", <= 9 => "III", _ => "IV" }} квартал {DateTime.Now.Year}",
            "year"    => $"За {DateTime.Now.Year} год",
            _         => "За всё время"
        };

        private void LoadData()
        {
            using var ctx = new CarDealershipContext();

            var (from, to) = GetPeriodRange();
            TxtPeriodLabel.Text = PeriodLabel();

            var allSales = ctx.Sales
                .Include(s => s.Car)
                .Include(s => s.Customer)
                .Include(s => s.Manager)
                .ToList();

            var periodSales = allSales
                .Where(s => s.SaleDate >= from && s.SaleDate <= to)
                .ToList();

            var allService = ctx.ServiceRecords.ToList();
            var periodService = allService
                .Where(s => s.ServiceDate >= from && s.ServiceDate <= to)
                .ToList();

            // --- Previous period for comparison (skip for "all time" to avoid DateTime overflow) ---
            List<CarDealership.Models.Sale> prevSales;
            if (_period == "all")
            {
                prevSales = new List<CarDealership.Models.Sale>();
            }
            else
            {
                var span = to - from;
                var prevFrom = from - span;
                prevSales = allSales.Where(s => s.SaleDate >= prevFrom && s.SaleDate < from).ToList();
            }

            // --- KPI Row 1 ---
            var revenue = periodSales.Sum(s => s.SalePrice);
            var prevRevenue = prevSales.Sum(s => s.SalePrice);
            TxtRevenue.Text = $"₽ {revenue:N0}";
            TxtRevenueChange.Text = FormatChange(revenue, prevRevenue, "vs пред. период");

            TxtSalesCount.Text = periodSales.Count.ToString();
            TxtSalesChange.Text = FormatChange(periodSales.Count, prevSales.Count, "vs пред. период");

            var avg = periodSales.Count > 0 ? revenue / periodSales.Count : 0;
            var prevAvg = prevSales.Count > 0 ? prevSales.Sum(s => s.SalePrice) / prevSales.Count : 0;
            TxtAvgDeal.Text = $"₽ {avg:N0}";
            TxtAvgChange.Text = FormatChange(avg, prevAvg, "vs пред. период");

            var srvRev = periodService.Sum(s => s.Cost);
            TxtServiceRev.Text = $"₽ {srvRev:N0}";
            TxtServiceCount.Text = $"{periodService.Count} заявок";

            // --- KPI Row 2 ---
            var now = DateTime.Now;
            var newClients = _period switch
            {
                "month"   => ctx.Customers.Count(c => c.DateAdded.Month == now.Month && c.DateAdded.Year == now.Year),
                "quarter" => ctx.Customers.Count(c => c.DateAdded >= from),
                "year"    => ctx.Customers.Count(c => c.DateAdded.Year == now.Year),
                _         => ctx.Customers.Count()
            };
            TxtNewClients.Text = newClients.ToString();
            TxtNewClientsNote.Text = $"Всего в базе: {ctx.Customers.Count()}";

            var totalCars = ctx.Cars.Count();
            var soldCars = ctx.Cars.Count(c => c.Status == "Sold");
            var conv = totalCars > 0 ? (double)soldCars / totalCars * 100 : 0;
            TxtConversion.Text = $"{conv:N1}%";
            TxtConversionNote.Text = $"{soldCars} продано из {totalCars} авт.";

            var maxDeal = periodSales.Count > 0 ? periodSales.Max(s => s.SalePrice) : 0;
            var maxDealSale = periodSales.FirstOrDefault(s => s.SalePrice == maxDeal);
            TxtMaxDeal.Text = $"₽ {maxDeal:N0}";
            TxtMaxDealNote.Text = maxDealSale?.Car != null ? $"{maxDealSale.Car.Brand} {maxDealSale.Car.Model}" : "";

            var totalDiscount = periodSales.Sum(s => s.Discount);
            TxtDiscounts.Text = $"₽ {totalDiscount:N0}";
            var discPct = revenue + totalDiscount > 0 ? totalDiscount / (revenue + totalDiscount) * 100 : 0;
            TxtDiscountsNote.Text = $"{discPct:N1}% от суммы сделок";

            // --- Managers ---
            var totalPeriodRev = (double)(revenue > 0 ? revenue : 1);
            var managerSales = periodSales
                .GroupBy(s => s.Manager?.FullName ?? "Не указан")
                .Select(g => new
                {
                    ManagerName = g.Key,
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(s => s.SalePrice),
                    AvgSale = g.Count() > 0 ? g.Sum(s => s.SalePrice) / g.Count() : 0,
                    SharePercent = (double)g.Sum(s => s.SalePrice) / totalPeriodRev * 100
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();
            GridManagerSales.ItemsSource = managerSales;

            // --- Payment methods with bar widths ---
            var totalCount = (double)(periodSales.Count > 0 ? periodSales.Count : 1);
            var payments = periodSales
                .GroupBy(s => string.IsNullOrEmpty(s.PaymentMethod) ? "Не указано" : s.PaymentMethod)
                .Select(g => new
                {
                    Method = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(s => s.SalePrice),
                    Percent = g.Count() / totalCount * 100,
                    BarWidth = g.Count() / totalCount * 260  // max bar width px
                })
                .OrderByDescending(x => x.Count)
                .ToList();
            IcPayments.ItemsSource = payments;

            // --- Brand sales ---
            var brandSales = periodSales
                .Where(s => s.Car != null)
                .GroupBy(s => s.Car!.Brand)
                .Select(g => new
                {
                    Brand = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(s => s.SalePrice),
                    Percent = g.Count() / totalCount * 100,
                    AvgPrice = g.Count() > 0 ? g.Sum(s => s.SalePrice) / g.Count() : 0
                })
                .OrderByDescending(x => x.Count)
                .ToList();
            GridBrandSales.ItemsSource = brandSales;

            // --- Car fleet status ---
            var cars = ctx.Cars.ToList();
            var cTotal = cars.Count > 0 ? cars.Count : 1;
            var cAvail = cars.Count(c => c.Status == "Available");
            var cRes = cars.Count(c => c.Status == "Reserved");
            var cSold = cars.Count(c => c.Status == "Sold");
            TxtFleetAvailable.Text = cAvail.ToString();
            TxtFleetAvailablePct.Text = $"{(double)cAvail / cTotal * 100:N0}%";
            TxtFleetReserved.Text = cRes.ToString();
            TxtFleetReservedPct.Text = $"{(double)cRes / cTotal * 100:N0}%";
            TxtFleetSold.Text = cSold.ToString();
            TxtFleetSoldPct.Text = $"{(double)cSold / cTotal * 100:N0}%";

            // --- Top customers ---
            var topCustomers = periodSales
                .Where(s => s.Customer != null)
                .GroupBy(s => s.Customer!.FullName)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    PurchaseCount = g.Count(),
                    TotalSpent = g.Sum(s => s.SalePrice)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(10)
                .Select((x, i) => new { Rank = i + 1, x.CustomerName, x.PurchaseCount, x.TotalSpent })
                .ToList();
            GridTopCustomers.ItemsSource = topCustomers;

            // --- Service types ---
            var srvTotal = (double)(periodService.Count > 0 ? periodService.Count : 1);
            var serviceTypes = periodService
                .GroupBy(s => string.IsNullOrEmpty(s.ServiceType) ? "Прочее" : s.ServiceType)
                .Select(g => new
                {
                    ServiceType = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(s => s.Cost),
                    Percent = g.Count() / srvTotal * 100
                })
                .OrderByDescending(x => x.Count)
                .ToList();
            GridServiceTypes.ItemsSource = serviceTypes;

            // --- Monthly chart (current year) ---
            var yearSales = allSales.Where(s => s.SaleDate.Year == now.Year).ToList();
            var maxMonthRev = Enumerable.Range(1, 12)
                .Select(m => (double)yearSales.Where(s => s.SaleDate.Month == m).Sum(s => s.SalePrice))
                .Max();
            maxMonthRev = maxMonthRev > 0 ? maxMonthRev : 1;
            const double maxBarH = 90.0;

            var months = Enumerable.Range(1, 12).Select(m => new
            {
                MonthShort = new DateTime(now.Year, m, 1).ToString("MMM"),
                Revenue = (double)yearSales.Where(s => s.SaleDate.Month == m).Sum(s => s.SalePrice),
                RevenueShort = FormatShort((double)yearSales.Where(s => s.SaleDate.Month == m).Sum(s => s.SalePrice)),
                BarH = Math.Max(4, yearSales.Where(s => s.SaleDate.Month == m).Sum(s => s.SalePrice) / (decimal)maxMonthRev * (decimal)maxBarH)
            }).ToList();
            IcMonthly.ItemsSource = months;
        }

        private static string FormatChange(decimal current, decimal previous, string suffix)
        {
            if (previous == 0) return suffix;
            var pct = (double)(current - previous) / (double)previous * 100;
            var sign = pct >= 0 ? "▲" : "▼";
            return $"{sign} {Math.Abs(pct):N1}% {suffix}";
        }

        private static string FormatChange(int current, int previous, string suffix)
            => FormatChange((decimal)current, (decimal)previous, suffix);

        private static string FormatShort(double v)
        {
            if (v == 0) return "";
            if (v >= 1_000_000) return $"{v / 1_000_000:N1}M";
            if (v >= 1_000) return $"{v / 1_000:N0}K";
            return v.ToString("N0");
        }
    }
}
