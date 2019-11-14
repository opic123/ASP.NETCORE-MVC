using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    // [Route("Home")]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /* [Route("")]
        [Route("Index")]
        [Route("~/")] */
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        /*
        public JsonResult Details()
        {
            var model = _employeeRepository.GetEmployee(1);
            return Json(model);
        }*/

        // [Route("Details/{Id?}")]
        public ViewResult Details(int? Id)
        {
            // Employee model = _employeeRepository.GetEmployee(1);
            // ViewBag.Employee = model;
            // ViewBag.Title = "Employee Details";
            // ViewData["Employee"] = model;
            // ViewData["Title"] = "Employee Details";

            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(Id ?? 1),
                PageTitle = "Employee Details"
            };
            return View(model);
        }

        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee newEmployee = _employeeRepository.Add(employee);
                return RedirectToAction("details", new { newEmployee.Id }); // same as new { Id = newEmployee.Id };
            }

            return View();
            
        }
    }
}