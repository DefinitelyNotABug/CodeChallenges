using CodeChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const string EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";
        private const string COMPENSATION_SEED_DATA_FILE = "resources/CompensationSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if (!_employeeContext.Employees.Any())
            {
                var employees = LoadJsonSeedData<Employee>(EMPLOYEE_SEED_DATA_FILE, UpdateDirectReportReferences);
                _employeeContext.Employees.AddRange(employees);
                await _employeeContext.SaveChangesAsync();
            }
            if (!_employeeContext.Compensations.Any())
            {
                var compensations = LoadJsonSeedData<Compensation>(COMPENSATION_SEED_DATA_FILE, UpdateEmployeeReferences);
                _employeeContext.Compensations.AddRange(compensations);
                await _employeeContext.SaveChangesAsync();
            }
        }

        private List<T> LoadJsonSeedData<T>(string seedDataFilePath, Action<List<T>> additionalSetup = null)
        {
            using (FileStream fs = new FileStream(seedDataFilePath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<T> seedData = serializer.Deserialize<List<T>>(jr);
                if (additionalSetup != null)
                {
                    additionalSetup(seedData);
                }

                return seedData;
            }
        }

        private void UpdateEmployeeReferences(List<Compensation> compensations)
        {
            foreach (var compensation in compensations) 
            {
                var employee = _employeeContext.Employees.Find(compensation.Employee.EmployeeId);
                if (employee != null)
                {
                    compensation.Employee = employee;
                }
            }
        }

        private void UpdateDirectReportReferences(List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                select new { Id = employee.EmployeeId, EmployeeRef = employee };

            employees.ForEach(employee =>
            {
                
                if (employee.DirectReports != null)
                {
                    var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                    employee.DirectReports.ForEach(report =>
                    {
                        var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                        referencedEmployees.Add(referencedEmployee);
                    });
                    employee.DirectReports = referencedEmployees;
                }
            });
        }
    }
}
