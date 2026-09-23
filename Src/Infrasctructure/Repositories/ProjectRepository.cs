using Application.DashboardService.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectDbContext _context;

        public ProjectRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Project> CreateAsync(
        Project project,
        CancellationToken cancellationToken = default)
        {
            await _context.Projects.AddAsync(
                project,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return project;
        }
    }
}
