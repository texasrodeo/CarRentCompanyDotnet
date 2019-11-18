using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

            IEnumerable<Car> autoPark = autoParkContext.AutoPark;
            //ViewBag.Count = autoPark.Count();
            ViewBag.Count = 3;
            ViewBag.Cars = autoPark;

            return View();
        }

        [HttpGet]
        public ActionResult ShowContracts()
        {
            if (Request.Params["sort"] == "all")
                ViewBag.Requests = getAllContracts(autoParkContext.Contracts);
            else if (Request.Params["sort"] == "requests")
                ViewBag.Requests = getRequests(autoParkContext.Contracts);
            else
                ViewBag.Requests = getApprovedContracts(autoParkContext.Contracts);
            ViewBag.Count = ViewBag.Requests.Count;
            return View();
        }


        [HttpGet]
        public ActionResult SendRequest(int id)
        {
            ViewBag.CarId = id;
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
                return View("Success");
            }
            else
            {

                ViewBag.Ref = "/Home/SendRequest/" + contract.CarId.ToString();
                return View("Error");
            }



        }


        [HttpGet]
        public ActionResult AddCar()
        {
            return View();
        }


        [HttpPost]
        public RedirectResult AddCar(Car car)
        {
            autoParkContext.AutoPark.Add(car);
            autoParkContext.SaveChanges();
            return Redirect("/Home/ShowCars");
        }

        [HttpGet]
        public ActionResult AlterCar(int id)
        {
            ViewBag.Car = autoParkContext.GetCarById(id);
            return View();
        }

        [HttpPost]
        public RedirectResult AlterCar(Car car)
        {
            autoParkContext.AlterCar(car);
            autoParkContext.SaveChanges();
            return Redirect("/Home/ShowCars");
        }


        [HttpGet]
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

    }
}
