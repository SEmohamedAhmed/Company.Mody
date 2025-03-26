using AutoMapper;
using Company.Mody.BLL.Interfaces;
using Company.Mody.BLL.Repositories;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Employee;
using Company.Mody.PL.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Company.Mody.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork,
            IMapper mapper) 
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
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
                                                ? await _unitOfWork.EmployeeRepository.GetAllAsync()
                                                : await _unitOfWork.EmployeeRepository.GetByNameAsync(keyword);

            if (Request.Headers["X-Requested-EmployeeSearch"] == "XMLHttpRequest") // if EmployeeSearch Ajax Call
            {
                // Return the HTML fragment of the employee table for AJAX
                return PartialView("PartialViews/_EmployeeTable", employees);
            }

            return View(employees);
        }




        //[HttpPost] ----> Error 405 , method not allowed from browser!
        // Get Create Employee view
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["Departments"] = departments;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Add employee to the database
        public async Task<IActionResult> Add(EmployeeDto model)
        {
            if (ModelState.IsValid) // checks data annotations validation
            {

                // incase of uploading invalid image file
                if (model.Image is not null && !FileManuplation.IsImage(model.Image))
                {
                    ModelState.AddModelError("", "Please Upload a Valid Image!");
                    return View(model);
                }

                if (model.Image is not null)
                {
                    model.ImageName = FileManuplation.Upload(model.Image, "images");
                }

                Employee employee = _mapper.Map<Employee>(model);
                await _unitOfWork.EmployeeRepository.AddAsync(employee);
                var count = _unitOfWork.Commit();

                if (count > 0)
                {
                    // Cannot use ViewBag as it only lasts for the current request, 
                    // Cannot use ViewData as its lifetime is limited to the current request.
                    // TempData is used instead because it persists across one redirect.


                    TempData["Message"] = "Employee is created";
                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // delete the image if uploaded and employee not created
                    if(model.ImageName is not null) FileManuplation.Delete(model.ImageName,"images");
                }
            }
            return View(model);
        }




        [HttpGet]
        // Get the update details view
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest(); // 400 status code

            var employee = _mapper.Map<EmployeeDto>(await _unitOfWork.EmployeeRepository.GetAsync(id.Value));
            if (employee == null) return NotFound();


            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["Departments"] = departments;

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Updates employee
        public IActionResult Update([FromRoute]int? id, EmployeeDto model)
        {
            if (!id.HasValue) return BadRequest();

            //if (id != model.Id) return BadRequest();

            // incase of uploading invalid image file
            if (model.Image is not null && !FileManuplation.IsImage(model.Image))
            {
                ModelState.AddModelError("", "Please Upload a Valid Image!");
                return View(model);
            }


            // handles if has image or has no image yet
            if(model.ImageName is not null && model.Image is not null)
            {
                FileManuplation.Delete(model.ImageName, "images");
            }

            if(model.Image is not null)
            {
                model.ImageName = FileManuplation.Upload(model.Image, "images");
            }
            


            Employee employee = _mapper.Map<Employee>(model);
            employee.Id = id.Value;

            if (ModelState.IsValid) // checks data annotations validation
            {
                _unitOfWork.EmployeeRepository.Update(employee);
                var count = _unitOfWork.Commit();

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
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        // Delete employee
        public IActionResult Delete([FromRoute] int? id, DeleteEmployeeDto model)
        {
            if (id is null) return BadRequest();

            if (id != model.Id) return BadRequest();

            #region Manual Mapping
            // 
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
            #endregion



            if (ModelState.IsValid)
            {
                // auto mapper
                var employee = _mapper.Map<Employee>(model);
                _unitOfWork.EmployeeRepository.Delete(employee);
                var count = _unitOfWork.Commit();

                if (count > 0)
                {
                    if (model.ImageName is not null)
                    {
                        FileManuplation.Delete(model.ImageName, "Images");
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName="Details")
        {
            if (id is null) return BadRequest(); // 400 status code

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee == null) return NotFound();


            return View(employee);
        }
    }
}
