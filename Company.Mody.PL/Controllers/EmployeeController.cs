using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var employees = _employeeRepository.GetAll();
            return View(employees);
        }

        //[HttpPost] ----> Error 405 , method not allowed from browser!
        // Get Create Employee view
        [HttpGet]
        public IActionResult Add()
        {
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
            if(id is null) return BadRequest();

            var employee = _employeeRepository.Get(id.Value);
            if (employee == null) return NotFound();

            return View(employee);
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
            if (id is null) return BadRequest();

            var employee = _employeeRepository.Get(id.Value);

            if (employee == null) return NotFound();

            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        // Delete employee
        public IActionResult Delete([FromRoute] int? id, Employee employee)
        {
            if (id is null) return BadRequest();

            if (id != employee.Id) return BadRequest();

            // checks data annotations validation
            if (ModelState.IsValid)
            {
                var count = _employeeRepository.Delete(employee);
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(employee);
        }

        public IActionResult Details(int? id)
        {
            if (id is null) return BadRequest(); // 400 status code

            var employee = _employeeRepository.Get(id.Value);
            if (employee == null) return NotFound();


            return View(employee);
        }
    }
}
