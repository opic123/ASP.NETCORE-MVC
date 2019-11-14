using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

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

        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        public ViewResult Details(int? Id)
        {

            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(Id ?? 1),
                PageTitle = "Employee Details"
            };
            return View(model);
        }

        [HttpGet]
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
                    uniqueFileName = ProcessUploadPhoto(model);
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);

            EmployeeEditViewModel model = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    uniqueFileName = ProcessUploadPhoto(model);

                    // delete old photo
                    if (model.ExistingPhotoPath != null)
                    {
                        string oldFilePath = Path.Combine(_webhostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(oldFilePath);
                    }


                }
                else
                {
                    uniqueFileName = model.ExistingPhotoPath;
                }

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                employee.PhotoPath = uniqueFileName;

                _employeeRepository.Update(employee);

                return RedirectToAction("details", new { employee.Id }); // same as new { Id = newEmployee.Id };
            }

            return View(model);

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _employeeRepository.Delete(id);

            return RedirectToAction("index");
        }


        // EmployeeCreateViewModel is the parent of EmployeeEditViewModel
        public string ProcessUploadPhoto(EmployeeCreateViewModel model)
        {
            string uploadsFolder = Path.Combine(_webhostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            // using disposes the filestream after execution
            using (var filestream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(filestream);
            }


            return uniqueFileName;
        }

    }
}