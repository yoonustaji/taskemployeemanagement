using EmployeeService.DTO;

namespace EmployeeService.Repository
{
    public interface IEmployeeRepository
    {
       public  Task<List<EmployeeDTO>> GetAllEmployees();

        public Task<EmployeeDTO> GetEmployeeById(int id);

        public Task<EmployeeDTO> UpdateEmployee(UpdateRequestDTO updateRequestDTO);

    }
}
