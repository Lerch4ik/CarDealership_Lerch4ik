using System;
using System.Windows;
using System.Windows.Controls;
using CarDealership.Themes;
using CarDealership.Views.Pages;

namespace CarDealership.Views
{
    public partial class MainWindow : Window
    {
        private Button? _activeButton;
        private Action? _reloadCurrentPage;

        public MainWindow()
        {
            InitializeComponent();
            TxtCurrentDate.Text = DateTime.Now.ToString("dd MMMM yyyy",
                new System.Globalization.CultureInfo("ru-RU"));
            _activeButton = BtnDashboard;
            UpdateThemeButton();
            NavigateTo(new DashboardPage(), BtnDashboard);
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Toggle();
            UpdateThemeButton();
            // Re-navigate current page so its Background (set via DynamicResource) refreshes
            _reloadCurrentPage?.Invoke();
        }

        private void UpdateThemeButton()
        {
            bool dark = ThemeManager.IsDark;

            if (BtnTheme.Template.FindName("TxtThemeIcon", BtnTheme) is TextBlock icon)
                icon.Text = dark ? "☀️" : "🌙";

            if (BtnTheme.Template.FindName("TxtThemeLabel", BtnTheme) is TextBlock label)
                label.Text = dark ? "Светлая тема" : "Тёмная тема";

            if (BtnTheme.Template.FindName("PillDot", BtnTheme) is System.Windows.Shapes.Ellipse dot)
            {
                dot.HorizontalAlignment = dark ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                dot.Margin = dark ? new Thickness(3, 0, 0, 0) : new Thickness(0, 0, 3, 0);
            }
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
                _activeButton.Style = (Style)FindResource("NavButton");
            _activeButton = btn;
            _activeButton.Style = (Style)FindResource("NavButtonActive");
        }

        private void NavigateTo(Page page, Button btn)
        {
            SetActiveButton(btn);
            MainFrame.Navigate(page);
            // Store reload lambda so theme toggle can re-create the same page
            _reloadCurrentPage = () => NavigateTo(Activator.CreateInstance(page.GetType()) as Page ?? page, btn);
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new DashboardPage(), BtnDashboard);

        private void BtnCars_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new CarsPage(), BtnCars);

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new CustomersPage(), BtnCustomers);

        private void BtnSales_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new SalesPage(), BtnSales);

        private void BtnManagers_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new ManagersPage(), BtnManagers);

        private void BtnService_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new ServicePage(), BtnService);

        private void BtnReports_Click(object sender, RoutedEventArgs e)
            => NavigateTo(new ReportsPage(), BtnReports);
    }
}
