using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DashboardService.Contracts
{
    public interface IProjectRepository
    {
        public Task<List<Project>> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default);

        public  Task<Project> CreateAsync(
        Project project,
        CancellationToken cancellationToken = default);
    }
}
