using EmployeeService.DTO;
using EmployeeService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
        {
            var employees = await employeeRepository.GetAllEmployees();

            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployeeById(int id )
        {
            var employees = await employeeRepository.GetEmployeeById(id);

            return Ok(employees);
        }

        [HttpPut]

        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> UpdateEmployee([FromBody] UpdateRequestDTO updateRequestDTO)
        {
            var employees = await employeeRepository.UpdateEmployee(updateRequestDTO);

            return Ok(employees);
        }
    }
}
