using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string keyword)
        {
            IEnumerable<Department> departments = string.IsNullOrEmpty(keyword)
                                    ?await _unitOfWork.DepartmentRepository.GetAllAsync()
                                    : await _unitOfWork.DepartmentRepository.GetByNameAsync(keyword);

            if (Request.Headers["X-Requested-DepartmentSearch"] == "XMLHttpRequest") // if Ajax Call
            {
                // Return the HTML fragment of the employee table for AJAX
                return PartialView("PartialViews/_DepartmentTable", departments);
            }
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
        public async Task<IActionResult> Add(Department department)
        {
            if (ModelState.IsValid) // checks data annotations validation
            {
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                var count = _unitOfWork.Commit();
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
        public async Task<IActionResult> Update(int? id)
        {
            return await Details(id, "Update");
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
                    _unitOfWork.DepartmentRepository.Update(department);
                    var count = _unitOfWork.Commit();

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
        public Task<IActionResult> Delete(int? id)
        {
            return Details(id, "Delete");
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
                _unitOfWork.DepartmentRepository.Delete(department);
                var count = _unitOfWork.Commit();

                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(department);
        }

        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null) return BadRequest(); // 400 status code

            var dept = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (dept == null) return NotFound();


            return View(dept);
        }
    }
}