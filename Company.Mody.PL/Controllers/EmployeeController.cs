using AutoMapper;
using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Employee;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(string? keyword)
        {
             
            

            #region Controller Storage(ViewData, ViewBag, TempData )
            // Razor View Storage is in form of One Pool Dictionary that can accessed through...
            // ...3 properties inherited from Controller Class

            // 1. ViewData  : Transfer Extra information from controller (action) to view
            //ViewData["Message"] = "Hello From View Data";


            // 2. ViewBag   : Transfer Extra information from controller (action) to view
            // more flexable than view data
            //ViewBag.Message = new { Message = "Hello From ViewBag.Message" };


            // 3. TempData  : it lasts for one redirection request 
            #endregion



            IEnumerable<Employee> employees = string.IsNullOrEmpty(keyword)
                                                ? _employeeRepository.GetAll()
                                                : _employeeRepository.GetByName(keyword);

            if (Request.Headers["X-Requested-EmployeeSearch"] == "XMLHttpRequest") // if Ajax Call
            {
                // Return the HTML fragment of the employee table for AJAX
                return PartialView("PartialViews/_EmployeeTable", employees);
            }

            return View(employees);
        }

        //[HttpPost] ----> Error 405 , method not allowed from browser!
        // Get Create Employee view
        [HttpGet]
        public IActionResult Add()
        {
            var departments = _departmentRepository.GetAll();
            ViewData["Departments"] = departments;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Add employee to the database
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid) // checks data annotations validation
            {
                var count = _employeeRepository.Add(employee);
                if (count > 0)
                {
                    // Cannot use ViewBag as it only lasts for the current request, 
                    // Cannot use ViewData as its lifetime is limited to the current request.
                    // TempData is used instead because it persists across one redirect.


                    TempData["Message"] = "Employee is created";
                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employee);
        }


        [HttpGet]
        // Get the update details view
        public IActionResult Update(int? id)
        {
            var departments = _departmentRepository.GetAll();
            ViewData["Departments"] = departments;
            return Details(id, "Update");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Updates employee
        public IActionResult Update([FromRoute]int? id, Employee employee)
        {
            if (id is null) return BadRequest();

            if(id != employee.Id) return BadRequest();

            if (ModelState.IsValid) // checks data annotations validation
            {
                var count = _employeeRepository.Update(employee);
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                    // Try another time to redirect to details of current dept
                }
            }
            return View(employee);
        }

        [HttpGet]
        // get deatils to delete employee
        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        // Delete employee
        public IActionResult Delete([FromRoute] int? id, DeleteEmployeeDto employeeDto)
        {
            if (id is null) return BadRequest();

            if (id != employeeDto.Id) return BadRequest();

            // Manual Mapping
            //Employee employee = new Employee()
            //{
            //    Id = employeeDto.Id,
            //    Name = employeeDto.Name,
            //    Address = employeeDto.Address,
            //    Age = employeeDto.Age,
            //    CreationDate = employeeDto.CreationDate,
            //    DepartmentId = employeeDto.DepartmentId,
            //    Email = employeeDto.Email,
            //    HiringDate = employeeDto.HiringDate,
            //    IsActive = employeeDto.IsActive,
            //    IsDeleted = employeeDto.IsDeleted,
            //    PhoneNumber = employeeDto.PhoneNumber,
            //    Salary = employeeDto.Salary,
            //};

            // auto mapper


            if (ModelState.IsValid)
            {
                var employee = _mapper.Map<Employee>(employeeDto);
                var count = _employeeRepository.Delete(employee);
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employeeDto);
        }

        public IActionResult Details(int? id, string viewName="Details")
        {
            if (id is null) return BadRequest(); // 400 status code

            var employee = _employeeRepository.Get(id.Value);
            if (employee == null) return NotFound();


            return View(viewName, employee);
        }
    }
}
