using CodeChallenge.Models;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "b7839309-3348-463b-a7e3-5de1c168beb3" // Paul
            };
            var effectiveDate = DateTime.UtcNow;
            var compensation = new Compensation()
            {
                Employee = employee,
                EffectiveDate = effectiveDate,
                Salary = 100
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.Id);
            Assert.AreEqual(employee.EmployeeId, newCompensation.Employee.EmployeeId);
            Assert.AreEqual("Paul", newCompensation.Employee.FirstName);
            Assert.AreEqual(100, newCompensation.Salary);
            Assert.AreEqual(effectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public async Task GetByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; // John
            var expectedSalary = 1000000;
            var expectedEffectiveDate = new DateTime(2024, 1, 1);

            // Execute
            var response = await _httpClient.GetAsync($"api/compensation/{employeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var returnedCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employeeId, returnedCompensation.Employee.EmployeeId);
            Assert.AreEqual(expectedSalary, returnedCompensation.Salary);
            Assert.AreEqual(expectedEffectiveDate.ToString(), returnedCompensation.EffectiveDate.ToString());
        }

        [TestMethod]
        public async Task GetByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = "invalidEmployeeId";

            // Execute
            var response = await _httpClient.GetAsync($"api/compensation/{employeeId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
