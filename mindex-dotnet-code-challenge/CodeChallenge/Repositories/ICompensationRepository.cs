using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        public Task<Compensation> CreateAsync(Compensation compensation);
        public Task SaveAsync();
        public Task<Compensation> GetByEmployeeIdAsync(string employeeId);
    }
}
