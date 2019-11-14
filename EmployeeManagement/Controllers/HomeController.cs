using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    // [Route("Home")]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webhostEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webhostEnvironment)
        {
            _employeeRepository = employeeRepository;
            _webhostEnvironment = webhostEnvironment;
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
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_webhostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);

                return RedirectToAction("details", new { newEmployee.Id }); // same as new { Id = newEmployee.Id };
            }

            return View();
            
        }
    }
}