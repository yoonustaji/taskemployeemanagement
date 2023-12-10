using AutoMapper;
using EmployeeService.DTO;
using EmployeeService.Models;

namespace EmployeeService.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee,EmployeeDTO>().ReverseMap();
        }
    }
}
