using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;
using EmployeeService.Repository;
using System.Windows.Input;

namespace TaskEmployeeManagement
{
    public class EmployeeWindowViewModel : BindableBase, INotifyPropertyChanged
    {

        #region Properties

        const string DEFAULTSTATUS = "active";

        private string _apiResponseMessage = "Select any employee record to edit (Update) or remove (Delete) ";
        public string ApiResponseMessage 
        { 
            get { return _apiResponseMessage; }
            set { SetProperty(ref _apiResponseMessage, value); }
        }

        private List<Employee> _employees;
        public List<Employee> Employees
        {
            get { return _employees; }
            set { SetProperty(ref _employees, value); }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref _selectedEmployee, value); }
        }

        private bool _isLoadData;
        public bool IsLoadData
        {
            get { return _isLoadData; }
            set { SetProperty(ref _isLoadData, value); }
        }

        private int _page;
        public int Page
        {
            get { return _page; }
            set { SetProperty(ref _page, value); }
        }

        #endregion

        #region [Employee Properties]

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _gender;
        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        #endregion

        #region [Form Properties]

        private bool _isShowForm;

        public bool IsShowForm
        {
            get { return _isShowForm; }
            set { SetProperty(ref _isShowForm, value); }
        }

        private string _showPostMessage = "Fill the form to create an employee!";

        public string ShowPostMessage
        {
            get { return _showPostMessage; }
            set { SetProperty(ref _showPostMessage, value); }
        }

        #endregion

        #region ICommands
        public DelegateCommand GetButtonClicked { get; set; }
        public DelegateCommand SearchButtonClicked { get; set; }
        public DelegateCommand ShowAddEmployeeForm { get; set; }
        public DelegateCommand PostButtonClick { get; set; }
        public DelegateCommand<Employee> PutButtonClicked { get; set; }
        public DelegateCommand<Employee> DeleteButtonClicked { get; set; }
        public DelegateCommand NextButtonClicked { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initalize properties & delegate commands
        /// </summary>
        public EmployeeWindowViewModel()
        {
            SearchButtonClicked = new DelegateCommand(SearchEmployeeDetails);
            GetButtonClicked = new DelegateCommand(GetEmployeeDetails);
            PutButtonClicked = new DelegateCommand<Employee>(UpdateEmployeeDetails);
            DeleteButtonClicked = new DelegateCommand<Employee>(DeleteEmployeeDetails);
            PostButtonClick = new DelegateCommand(CreateNewEmployee);
            ShowAddEmployeeForm = new DelegateCommand(AddEmployee);
            NextButtonClicked = new DelegateCommand(NextPageEmployeeDetails);
        }
        #endregion

        #region CallAPI
        /// <summary>
        /// Make visible Add user form
        /// </summary>
        private void AddEmployee()
        {
            IsShowForm = true;
        }

        /// <summary>
        /// Search Employee
        /// </summary>
        private void SearchEmployeeDetails()
        {

            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    ApiResponseMessage = "Key in name to search!";
                    return;
                }

                var employeeDetails = EmployeeRepository.GetEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees + "?name=" + Name, RESTAPIURI.token);

                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Employees = employeeDetails.Result.Content.ReadAsAsync<List<Employee>>().Result;
                    IsLoadData = true;
                    Page = 1;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Fetches employee details
        /// </summary>
        private void GetEmployeeDetails()
        {

            try
            {
                var employeeDetails = EmployeeRepository.GetEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees, RESTAPIURI.token);

                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Employees = employeeDetails.Result.Content.ReadAsAsync<List<Employee>>().Result;
                    IsLoadData = true;
                    Page = 1;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Fetches next page of employee details
        /// </summary>
        private void NextPageEmployeeDetails()
        {

            try
            {
                Page += 1;
                var employeeDetails = EmployeeRepository.GetEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees + "?page=" + Page, RESTAPIURI.token);

                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Employees = employeeDetails.Result.Content.ReadAsAsync<List<Employee>>().Result;
                    IsLoadData = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Adds new employee
        /// </summary>
        private void CreateNewEmployee()
        {
            Employee newEmployee = new Employee()
            {
                name = Name,
                email = Email,
                gender = Gender,
                status = DEFAULTSTATUS
            };
            var employeeDetails = EmployeeRepository.CreateEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees, RESTAPIURI.token, newEmployee);

            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                ShowPostMessage = newEmployee.name + "'s record added!";
            }
            else
            {
                ShowPostMessage = "Failed to add " + newEmployee.name + "'s record.";
            }
        }


        /// <summary>
        /// Updates employee's record
        /// </summary>
        /// <param name="employee"></param>
        private void UpdateEmployeeDetails(Employee employee)
        {
            try
            {
                if (employee == null || employee.id == 0)
                {
                    ApiResponseMessage = "Select an employee to update";
                    return;
                }

                if (employee.status != "active" && employee.status != "inactive")
                {
                    ApiResponseMessage = "Status must be active or inactive";
                    return;
                }

                if (employee.gender != "male" && employee.gender != "female")
                {
                    ApiResponseMessage = "Gender must be male or female";
                    return;
                }

                if (string.IsNullOrEmpty(employee.name))
                {
                    ApiResponseMessage = "Name cannot be empty!";
                    return;
                }

                if (string.IsNullOrEmpty(employee.email))
                {
                    ApiResponseMessage = "Email cannot be empty!";
                    return;
                }

            }
            catch (Exception)
            {
                ApiResponseMessage = "Select an employee to update";
                return;
            }

            var employeeDetails = EmployeeRepository.UpdateEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees + "/" + employee.id, RESTAPIURI.token, employee);

            if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ApiResponseMessage = employee.name + "'s details has successfully been updated!";
            }
            else
            {
                ApiResponseMessage = "Failed to update " + employee.name + "'s details.";
            }
        }

        /// <summary>
        /// Deletes employee's record
        /// </summary>
        /// <param name="employee"></param>
        private void DeleteEmployeeDetails(Employee employee)
        {
            try
            {
                if (employee == null || employee.id == 0)
                {
                    ApiResponseMessage = "Select an employee to delete";
                    return;
                }
            }
            catch (Exception)
            {
                ApiResponseMessage = "Select an employee to delete";
                return;
            }

            try
            {
                var employeeDetails = EmployeeRepository.DeleteEmployee(RESTAPIURI.baseURI + RESTAPIURI.employees + "/" + employee.id, RESTAPIURI.token);

                if (employeeDetails.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ApiResponseMessage = employee.name + "'s record has successfully been deleted!";
                }
                else
                {
                    ApiResponseMessage = "Failed to remove " + employee.name + "'s record.";
                }
            }
            catch (Exception ex)
            {
                ApiResponseMessage = "Error calling API " + ex.Message;
            }

        }
        #endregion

    }
}
