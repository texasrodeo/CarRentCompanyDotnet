using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using CarRentCompanyDotnet.Models;

namespace CarRentCompanyDotnet.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext autoParkContext = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ShowCars()
        {
            
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                                           .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                IList<string> roles = new List<string>();
                roles = userManager.GetRoles(user.Id);
                if (roles.Contains("admin"))
                    ViewBag.Role = "admin";
                else
                    ViewBag.Role = "user";
            }
            else
            {
                ViewBag.Role = "noname";
            }
            
            
                IEnumerable<Car> autoPark = autoParkContext.AutoPark;
                ViewBag.Count = autoPark.Count();
                ViewBag.Cars = autoPark;
                
                return View();
            
        }

        

        [HttpGet]
        public ActionResult ShowCarInfo(int id)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                                           .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
            {
                IList<string> roles = new List<string>();
                roles = userManager.GetRoles(user.Id);
                if (roles.Contains("admin"))
                    ViewBag.Role = "admin";
                else
                    ViewBag.Role = "user";
            }
            else
            {
                ViewBag.Role = "noname";
            }

          
            Car car = autoParkContext.GetCarById(id);
            ViewBag.Id = id;
            ViewBag.Brand = car.Brand;
            ViewBag.Info = car.Info;
            ViewBag.Price = car.Price;
           

            return View();
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public ActionResult ShowContracts()
        {
            if (Request.Params["sort"] == "all")
                ViewBag.Requests = getAllContracts(autoParkContext.Contracts);
            else if (Request.Params["sort"] == "requests")
                ViewBag.Requests = getRequests(autoParkContext.Contracts);
            else
                ViewBag.Requests = getApprovedContracts(autoParkContext.Contracts);
            ViewBag.Count = ViewBag.Requests.Count;
            ViewBag.Role = "admin";
            
            return View();
        }

        [HttpGet]
        [Authorize(Roles="admin")]
        public ActionResult ApproveContract(int id)
        {
            autoParkContext.ApproveContractById(id);
            autoParkContext.SaveChanges();
            ViewBag.Role = "admin";
            ViewBag.Url = "/Home/ShowContracts";
            return View("Success");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RefuseContract(int id)
        {
            autoParkContext.RefuseContractById(id);
            autoParkContext.SaveChanges();
            ViewBag.Role = "admin";
            ViewBag.Url = "/Home/ShowContracts";
            return View("Success");
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        public ActionResult ShowUserContracts(string id)
        {
            if (Request.Params["sort"] == "all")
                ViewBag.Requests = getAllContractsForUser(autoParkContext.Contracts, id);
            else if (Request.Params["sort"] == "requests")
                ViewBag.Requests = getRequestsForUser(autoParkContext.Contracts, id);
            else
                ViewBag.Requests = getApprovedContractsForUser(autoParkContext.Contracts, id);
            ViewBag.Count = ViewBag.Requests.Count;
            ViewBag.Role = "user";
            return View("ShowContracts");
        }

        [Authorize(Roles = "user")]
        public ActionResult Details(int id)
        {
            Car c = autoParkContext.AutoPark.FirstOrDefault(com => com.Id == id);
            if (c != null)
                return PartialView(c);
            return HttpNotFound();
        }


        [Authorize]
        [HttpGet]
        public ActionResult SendRequest(int id)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                                          .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            ViewBag.CarId = id;
            ViewBag.UserId = user.Id;
            return View();
        }



        [HttpPost]
        public ActionResult SendRequest(Contract contract)
        {

            if (DateTime.Compare(contract.Start, contract.End) <= 0)
            {
                

                // добавляем информацию о покупке в базу данных
                autoParkContext.Contracts.Add(contract);
                // сохраняем в бд все изменения
                autoParkContext.SaveChanges();
                ViewBag.Url = "/Home/Index";
                return View("Success");
            }
            else
            {

                ViewBag.Ref = "/Home/SendRequest/" + contract.CarId.ToString();
                return View("Error");
            }



        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddCar()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public RedirectResult AddCar(Car car)
        {
            autoParkContext.AutoPark.Add(car);
            autoParkContext.SaveChanges();
            return Redirect("/Home/ShowCars");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AlterCar(int id)
        {
            ViewBag.Car = autoParkContext.GetCarById(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public RedirectResult AlterCar(Car car)
        {
            autoParkContext.AlterCar(car);
            autoParkContext.SaveChanges();
            return Redirect("/Home/ShowCars");
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public RedirectResult DeleteCar(int id)
        {
            autoParkContext.removeCarById(id);
            autoParkContext.SaveChanges();
            return Redirect("/Home/ShowCars");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private IEnumerable<Contract> getRequests(IEnumerable<Contract> contracts)
        {
            List<Contract> result = new List<Contract>();
            foreach (Contract c in contracts)
            {
                if (!c.IsApproved)
                    result.Add(c);
            }
            return result;
        }
        private IEnumerable<Contract> getApprovedContracts(IEnumerable<Contract> contracts)
        {
            List<Contract> result = new List<Contract>();
            foreach (Contract c in contracts)
            {
                if (c.IsApproved)
                    result.Add(c);
            }
            return result;
        }

        private IEnumerable<Contract> getAllContracts(IEnumerable<Contract> contracts)
        {
            return contracts.ToList();
        }

        private IEnumerable<Contract> getRequestsForUser(IEnumerable<Contract> contracts, string id)
        {
            List<Contract> result = new List<Contract>();
            foreach (Contract c in contracts)
            {
                if (!c.IsApproved && c.ClientId == id)
                    result.Add(c);
            }
            return result;
        }
        private IEnumerable<Contract> getApprovedContractsForUser(IEnumerable<Contract> contracts, string id)
        {
            List<Contract> result = new List<Contract>();
            foreach (Contract c in contracts)
            {
                if (c.IsApproved && c.ClientId == id)
                    result.Add(c);
            }
            return result;
        }

        private IEnumerable<Contract> getAllContractsForUser(IEnumerable<Contract> contracts, string id)
        {
            List<Contract> result = new List<Contract>();
            foreach (Contract c in contracts)
            {
                if (c.ClientId == id)
                    result.Add(c);
            }
            return result;
        }

        private void ApproveContract(Contract contract)
        {
            contract.IsApproved = true;
        }

        private void RefuseContract(Contract contract)
        {
            contract.IsApproved = false;
        }

    }
}
