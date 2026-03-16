using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarDealership.Models
{
    public class Car
    {
        public int Id { get; set; }
        
        [Required]
        public string Brand { get; set; } = "";
        
        [Required]
        public string Model { get; set; } = "";
        
        public int Year { get; set; }
        
        public string Color { get; set; } = "";
        
        public decimal Price { get; set; }
        
        public int Mileage { get; set; }
        
        public string FuelType { get; set; } = "";
        
        public string Transmission { get; set; } = "";
        
        public string BodyType { get; set; } = "";
        
        public string EngineVolume { get; set; } = "";
        
        public string VIN { get; set; } = "";
        
        public string Status { get; set; } = "Available"; // Available, Reserved, Sold
        
        public string Description { get; set; } = "";
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
    }

    public class Customer
    {
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; } = "";
        
        [Required]
        public string LastName { get; set; } = "";
        
        public string MiddleName { get; set; } = "";
        
        public string Phone { get; set; } = "";
        
        public string Email { get; set; } = "";
        
        public string Address { get; set; } = "";
        
        public DateTime DateOfBirth { get; set; }
        
        public string PassportNumber { get; set; } = "";
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        public string Notes { get; set; } = "";
        
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Car> Cars { get; set; } = new List<Car>();
        
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }

    public class Sale
    {
        public int Id { get; set; }
        
        public int CarId { get; set; }
        public Car? Car { get; set; }
        
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        
        public int? ManagerId { get; set; }
        public Manager? Manager { get; set; }
        
        public decimal SalePrice { get; set; }
        
        public decimal Discount { get; set; }
        
        public string PaymentMethod { get; set; } = ""; // Cash, Credit, Leasing
        
        public DateTime SaleDate { get; set; } = DateTime.Now;
        
        public string Notes { get; set; } = "";
        
        public string Status { get; set; } = "Completed"; // Pending, Completed, Cancelled
    }

    public class Manager
    {
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; } = "";
        
        [Required]
        public string LastName { get; set; } = "";
        
        public string Phone { get; set; } = "";
        
        public string Email { get; set; } = "";
        
        public string Position { get; set; } = "";
        
        public DateTime HireDate { get; set; } = DateTime.Now;
        
        public bool IsActive { get; set; } = true;
        
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        
        public string FullName => $"{LastName} {FirstName}".Trim();
    }

    public class ServiceRecord
    {
        public int Id { get; set; }
        
        public int CarId { get; set; }
        public Car? Car { get; set; }
        
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        
        public string ServiceType { get; set; } = "";
        
        public string Description { get; set; } = "";
        
        public decimal Cost { get; set; }
        
        public DateTime ServiceDate { get; set; } = DateTime.Now;
        
        public DateTime? CompletionDate { get; set; }
        
        public string Status { get; set; } = "In Progress"; // In Progress, Completed, Cancelled
        
        public string TechnicianName { get; set; } = "";
    }
}
