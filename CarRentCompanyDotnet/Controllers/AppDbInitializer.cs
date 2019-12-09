using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using CarRentCompanyDotnet.Models;

namespace CarRentCompanyDotnet.Controllers
{
    public class AppDbInitializer: DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);

            // создаем пользователей
            var admin = new ApplicationUser { Email = "kublenko_p_v@sc.vsu.ru", UserName = "kublenko_p_v@sc.vsu.ru" };
            string password = "22p03v00K!";
            var result = userManager.Create(admin, password);

            // если создание пользователя прошло успешно
            if (result.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(admin.Id, role2.Name);
            }
            context.AutoPark.Add(new Car { Brand = "Volvo", Info = "Надежная машина 150 л.с", Price = 2200, Avaliability = true });
            context.AutoPark.Add(new Car { Brand = "Lada", Info = "Бюджетная машина 90 л.с", Price = 1000, Avaliability = true });
            context.AutoPark.Add(new Car { Brand = "Maybach", Info = "Машина класса люкс 600 л.с", Price = 10000, Avaliability = false });

            base.Seed(context);
        }
    }
}