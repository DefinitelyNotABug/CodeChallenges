using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        public Task<Compensation> CreateAsync(Compensation compensation);
        public Task<Compensation> GetByEmployeeIdAsync(string employeeId);
    }
}
