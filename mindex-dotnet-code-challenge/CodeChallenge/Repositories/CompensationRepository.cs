using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _logger = logger;
            _employeeContext = employeeContext;
        }

        public async Task<Compensation> CreateAsync(Compensation compensation)
        {
            compensation.Id = Guid.NewGuid().ToString();
            await _employeeContext.AddAsync(compensation).ConfigureAwait(false);
            return compensation;
        }

        public async Task SaveAsync()
        {
            await _employeeContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Compensation> GetByEmployeeIdAsync(string employeeId)
        {
            return await _employeeContext.Compensations.Include(c => c.Employee).SingleOrDefaultAsync(c => c.Employee.EmployeeId == employeeId).ConfigureAwait(false);
        }
    }
}
