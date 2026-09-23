using Application.DashboardService.Contracts;
using Domain.Entities;
using Infrasctructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DashboardDbContext _context;

        public DashboardRepository(DashboardDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverview?> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DashboardOverviews
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);
        }
    }
}
