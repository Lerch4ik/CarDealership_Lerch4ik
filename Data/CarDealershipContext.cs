using Microsoft.EntityFrameworkCore;
using CarDealership.Models;
using System;
using System.IO;
using System.Linq;

namespace CarDealership.Data
{
    public class CarDealershipContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cardealership.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Sale>()
                .Property(s => s.SalePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Sale>()
                .Property(s => s.Discount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ServiceRecord>()
                .Property(sr => sr.Cost)
                .HasColumnType("decimal(18,2)");
        }

        public void SeedData()
        {
            if (Cars.Any()) return;

            var managers = new[]
            {
                new Manager { FirstName = "Алексей", LastName = "Петров", Phone = "+7 (495) 123-4567", Email = "petrov@dealer.ru", Position = "Старший менеджер", HireDate = new DateTime(2020, 1, 15) },
                new Manager { FirstName = "Мария", LastName = "Иванова", Phone = "+7 (495) 234-5678", Email = "ivanova@dealer.ru", Position = "Менеджер по продажам", HireDate = new DateTime(2021, 3, 10) },
                new Manager { FirstName = "Дмитрий", LastName = "Сидоров", Phone = "+7 (495) 345-6789", Email = "sidorov@dealer.ru", Position = "Менеджер по продажам", HireDate = new DateTime(2022, 6, 1) },
            };
            Managers.AddRange(managers);
            SaveChanges();

            var customers = new[]
            {
                new Customer { FirstName = "Иван", LastName = "Смирнов", Phone = "+7 (916) 111-2233", Email = "smirnov@mail.ru", Address = "Москва, ул. Ленина, 15", DateOfBirth = new DateTime(1985, 5, 20), PassportNumber = "4512 345678" },
                new Customer { FirstName = "Елена", LastName = "Козлова", Phone = "+7 (926) 222-3344", Email = "kozlova@gmail.com", Address = "Москва, ул. Пушкина, 8", DateOfBirth = new DateTime(1990, 8, 15), PassportNumber = "4513 456789" },
                new Customer { FirstName = "Сергей", LastName = "Новиков", Phone = "+7 (903) 333-4455", Email = "novikov@yandex.ru", Address = "МО, Химки, ул. Мира, 23", DateOfBirth = new DateTime(1978, 3, 7), PassportNumber = "4514 567890" },
                new Customer { FirstName = "Ольга", LastName = "Морозова", Phone = "+7 (985) 444-5566", Email = "morozova@mail.ru", Address = "Москва, Кутузовский пр., 47", DateOfBirth = new DateTime(1995, 12, 1), PassportNumber = "4515 678901" },
                new Customer { FirstName = "Андрей", LastName = "Волков", Phone = "+7 (917) 555-6677", Email = "volkov@gmail.com", Address = "Москва, ул. Арбат, 12", DateOfBirth = new DateTime(1982, 7, 22), PassportNumber = "4516 789012" },
            };
            Customers.AddRange(customers);
            SaveChanges();

            var cars = new[]
            {
                new Car { Brand = "Toyota", Model = "Camry", Year = 2023, Color = "Белый перламутр", Price = 2850000, Mileage = 0, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Седан", EngineVolume = "2.5", VIN = "JT2BF22K900123456", Status = "Available", Description = "Новый автомобиль, комплектация Prestige" },
                new Car { Brand = "Toyota", Model = "RAV4", Year = 2023, Color = "Чёрный металлик", Price = 3200000, Mileage = 0, FuelType = "Гибрид", Transmission = "Вариатор", BodyType = "Кроссовер", EngineVolume = "2.5", VIN = "JT3HP10V5X0234567", Status = "Available", Description = "Полный привод, комплектация Luxe" },
                new Car { Brand = "BMW", Model = "3 Series", Year = 2022, Color = "Серебристый", Price = 4500000, Mileage = 12000, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Седан", EngineVolume = "2.0", VIN = "WBAFR9C50BC345678", Status = "Available", Description = "Пробег 12000 км, отличное состояние" },
                new Car { Brand = "Mercedes-Benz", Model = "GLC 300", Year = 2023, Color = "Графитовый металлик", Price = 6800000, Mileage = 0, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Кроссовер", EngineVolume = "2.0", VIN = "WDC0G8EB1KF456789", Status = "Reserved", Description = "Новый, AMG пакет" },
                new Car { Brand = "Hyundai", Model = "Tucson", Year = 2022, Color = "Синий металлик", Price = 2200000, Mileage = 8500, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Кроссовер", EngineVolume = "2.0", VIN = "TMAJ381AANX567890", Status = "Available", Description = "Один владелец, сервисная книжка" },
                new Car { Brand = "Kia", Model = "K5", Year = 2023, Color = "Красный металлик", Price = 2450000, Mileage = 0, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Седан", EngineVolume = "2.5", VIN = "KNALN4278P5678901", Status = "Available", Description = "Комплектация GT" },
                new Car { Brand = "Volkswagen", Model = "Tiguan", Year = 2022, Color = "Белый", Price = 3100000, Mileage = 15000, FuelType = "Дизель", Transmission = "DSG", BodyType = "Кроссовер", EngineVolume = "2.0", VIN = "WVGFK77L08D789012", Status = "Sold" },
                new Car { Brand = "Audi", Model = "A6", Year = 2023, Color = "Серый металлик", Price = 5900000, Mileage = 0, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Седан", EngineVolume = "3.0", VIN = "WAUZZZ4G0BN890123", Status = "Available" },
                new Car { Brand = "Lexus", Model = "RX 350", Year = 2022, Color = "Белый жемчуг", Price = 5500000, Mileage = 20000, FuelType = "Бензин", Transmission = "Автомат", BodyType = "Кроссовер", EngineVolume = "3.5", VIN = "2T2BZMCA2KC901234", Status = "Available" },
                new Car { Brand = "Skoda", Model = "Octavia", Year = 2021, Color = "Чёрный", Price = 1850000, Mileage = 35000, FuelType = "Бензин", Transmission = "Механика", BodyType = "Лифтбек", EngineVolume = "1.4", VIN = "TMBEG7NE8L0012345", Status = "Available" },
            };
            Cars.AddRange(cars);
            SaveChanges();

            var sales = new[]
            {
                new Sale { CarId = 7, CustomerId = 1, ManagerId = 1, SalePrice = 3050000, Discount = 50000, PaymentMethod = "Кредит", SaleDate = new DateTime(2024, 11, 5), Status = "Completed" },
                new Sale { CarId = 3, CustomerId = 2, ManagerId = 2, SalePrice = 4400000, Discount = 100000, PaymentMethod = "Наличные", SaleDate = new DateTime(2024, 12, 10), Status = "Completed" },
                new Sale { CarId = 1, CustomerId = 3, ManagerId = 1, SalePrice = 2850000, Discount = 0, PaymentMethod = "Лизинг", SaleDate = new DateTime(2025, 1, 15), Status = "Completed" },
            };
            Sales.AddRange(sales);

            // Mark VW Tiguan as sold in the tracked entity
            var tiguan = Cars.Local.FirstOrDefault(c => c.Brand == "Volkswagen");
            if (tiguan != null) tiguan.Status = "Sold";
            
            var services = new[]
            {
                new ServiceRecord { CarId = 5, CustomerId = 2, ServiceType = "ТО", Description = "Плановое ТО 10000 км", Cost = 15000, ServiceDate = new DateTime(2025, 1, 10), CompletionDate = new DateTime(2025, 1, 10), Status = "Completed", TechnicianName = "Краснов А.В." },
                new ServiceRecord { CarId = 3, CustomerId = 2, ServiceType = "Ремонт", Description = "Замена тормозных колодок", Cost = 12000, ServiceDate = new DateTime(2025, 2, 1), CompletionDate = new DateTime(2025, 2, 1), Status = "Completed", TechnicianName = "Белов С.Н." },
                new ServiceRecord { CarId = 1, CustomerId = 3, ServiceType = "ТО", Description = "Первое ТО 10000 км", Cost = 18000, ServiceDate = new DateTime(2025, 2, 15), Status = "In Progress", TechnicianName = "Краснов А.В." },
            };
            ServiceRecords.AddRange(services);

            SaveChanges();
        }
    }
}
