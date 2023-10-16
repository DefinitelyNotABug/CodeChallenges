using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public async Task<ReportingStructure> GetReportingStructureByIdAsync(string id)
        {
            Employee employee = null;
            
            // Perform sanity checks
            if (!string.IsNullOrEmpty(id))
            {
                employee = _employeeRepository.GetById(id);
            }
            if (employee == null)
            {
                _logger.LogWarning("No employee found with id: {id}", id);
                return null;
            }

            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = await CountReportsAsync(employee).ConfigureAwait(false),
            };
        }

        public async Task<int> CountReportsAsync(Employee employee)
        {
            // Ensure any direct reports are loaded
            await _employeeRepository.LoadDirectReportsAsync(employee).ConfigureAwait(false);

            // Start with count of this employee's direct reports, then recursively count through the tree
            var totalReports = employee.DirectReports.Count;
            foreach (var directReport in employee.DirectReports)
            {
                totalReports += await CountReportsAsync(directReport).ConfigureAwait(false);
            }

            return totalReports;
        }
    }
}
