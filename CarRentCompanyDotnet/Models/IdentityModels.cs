using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CarRentCompanyDotnet.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string LicenseSeries { get; set; }

        public string LicenseNumber { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }

        public string Info { get; set; }
        public int Price { get; set; }
        public bool Avaliability { get; set; }
    }

    public class Contract
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string ClientId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsApproved { get; set; }

    
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

       

        public DbSet<Car> AutoPark { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public ApplicationDbContext()
            : base("ApplicationDbContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public void removeCarById(int id)
        {
            Car c = AutoPark
                .Where(o => o.Id == id)
                   .FirstOrDefault();
            AutoPark.Remove(c);
        }

        public void AlterCar(Car car)
        {
            Car c = AutoPark
                .Where(o => o.Id == car.Id)
                   .FirstOrDefault();
            c.Info = car.Info;
            c.Price = car.Price;
            c.Brand = car.Brand;
            c.Avaliability = car.Avaliability;
        }

        public Car GetCarById(int id)
        {
            return AutoPark
                .Where(o => o.Id == id)
                   .FirstOrDefault();
        }

        public List<Contract> GetContractsForUser(string id)
        {
            return (from contract in this.Contracts
                   where contract.ClientId == id
                   select contract).ToList();
        }

        public Contract GetContractById(int id)
        {
            return Contracts.Where(o => o.Id == id).FirstOrDefault();
        }

        public void ApproveContractById(int id)
        {
            Contract contract = Contracts.Where(o => o.Id == id).FirstOrDefault();
            contract.IsApproved = true;
        }

        public void RefuseContractById(int id)
        {
            Contract contract = Contracts.Where(o => o.Id == id).FirstOrDefault();
            contract.IsApproved = false;
        }
    }
}