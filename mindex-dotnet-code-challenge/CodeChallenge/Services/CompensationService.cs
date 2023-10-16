using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ILogger<CompensationService> _logger;
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _logger = logger;
            _compensationRepository = compensationRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Compensation> CreateAsync(Compensation compensation)
        {
            if (compensation != null)
            {
                if (compensation.Employee == null)
                {
                    throw new InvalidOperationException("Employee must be specified.");
                }
                else
                {
                    var existingEmployee = _employeeRepository.GetById(compensation.Employee?.EmployeeId);
                    if (existingEmployee != null)
                    {
                        var existingCompensation = await _compensationRepository.GetByEmployeeIdAsync(existingEmployee.EmployeeId).ConfigureAwait(false);
                        if (existingCompensation != null)
                        {
                            throw new InvalidOperationException($"A compensation already exists for employee: {existingEmployee.FirstName} {existingEmployee.LastName}.");
                        }
                        compensation.Employee = existingEmployee;
                    }
                }
                compensation = await _compensationRepository.CreateAsync(compensation).ConfigureAwait(false);
                await _compensationRepository.SaveAsync().ConfigureAwait(false);
            }
            return compensation;
        }

        public async Task<Compensation> GetByEmployeeIdAsync(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                _logger.LogError("Invalid employee id specified.");
                return null;
            }

            return await _compensationRepository.GetByEmployeeIdAsync(employeeId).ConfigureAwait(false);
        }
    }
}
