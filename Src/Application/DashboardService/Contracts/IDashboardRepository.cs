using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DashboardService.Contracts
{
    public interface IDashboardRepository
    {
        Task<DashboardOverview?> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }
}
