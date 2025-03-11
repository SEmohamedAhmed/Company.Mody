using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var departments = _departmentRepository.GetAll();
            return View(departments);
        }

        //[HttpPost] ----> Error 405 , method not allowed from browser!
        // Get Create Department view
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        // Add department to the database
        public IActionResult Add(Department department)
        {
            if (ModelState.IsValid) // checks data annotations validation
            {
                var count = _departmentRepository.Add(department);
                if (count > 0)
                {
                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(department);
        }


        [HttpGet]
        // Get the update details view
        public IActionResult Update(int? id)
        {
            if (id is null) return BadRequest();

            var dept = _departmentRepository.Get(id.Value);
            if (dept == null) return NotFound();

            return View(dept);
        }

        [HttpPost]
        // Prevent any request from other clients other than the same application
        // tested using postman post request!
        [AutoValidateAntiforgeryToken]
        // Updates department
        // we get the id from the segment/route as we dont gurantee the client as it can sends id,
        // also we need an id if we use a dto 
        // check priority of the request params binding

        public IActionResult Update([FromRoute] int? id, Department department)

        {


            try
            {
                if (id != department.Id) return BadRequest("Invalid Operation");

                if (ModelState.IsValid) // checks data annotations validation
                {
                    var count = _departmentRepository.Update(department);
                    if (count > 0)
                    {
                        return RedirectToAction(nameof(Index));
                        // Try another time to redirect to details of current dept
                    }
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(String.Empty, ex.Message);
            }

            return View(department);
        }

        [HttpGet]
        // get deatils to delete department
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            var department = _departmentRepository.Get(id.Value);

            if (department == null) return NotFound();

            return View(department);
        }


        // Deletes department
        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult Delete([FromRoute] int? id, Department department)
        {
            if (id != department.Id) return BadRequest("Invalid Operation");

            // checks data annotations validation
            if (ModelState.IsValid)
            {
                var count = _departmentRepository.Delete(department);
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(department);
        }

        public IActionResult Details(int? id)
        {
            if (id is null) return BadRequest(); // 400 status code

            var dept = _departmentRepository.Get(id.Value);
            if (dept == null) return NotFound();


            return View(dept);
        }
    }
}