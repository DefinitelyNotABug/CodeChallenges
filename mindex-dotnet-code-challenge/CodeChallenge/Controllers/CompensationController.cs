using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger<CompensationController> _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Compensation compensation)
        {
            _logger.LogDebug("Received compensation create request.");

            await _compensationService.CreateAsync(compensation).ConfigureAwait(false);
            
            return CreatedAtRoute("getByEmployeeId", new { employeeId = compensation.Employee.EmployeeId }, compensation);
        }

        [HttpGet("{employeeId}", Name = "getByEmployeeId")]
        public async Task<IActionResult> GetByEmployeeId(string employeeId) 
        {
            _logger.LogDebug("Received compensation get request for compensation with employee id: {employeeId}.", employeeId);

            var compensation = await _compensationService.GetByEmployeeIdAsync(employeeId).ConfigureAwait(false);
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
