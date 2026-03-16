using System.Windows;
using CarDealership.Data;

namespace CarDealership
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using var context = new CarDealershipContext();
            context.Database.EnsureCreated();
            context.SeedData();
        }
    }
}
