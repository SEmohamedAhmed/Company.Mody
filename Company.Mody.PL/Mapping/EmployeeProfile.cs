using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.Employee;

namespace Company.Mody.PL.Mapping
{
    public class EmployeeProfile:Profile
    {
        public EmployeeProfile()
        {
            CreateMap<DeleteEmployeeDto,Employee>().ReverseMap();
            CreateMap<EmployeeDto, Employee>().ReverseMap();

        }
    }
}
