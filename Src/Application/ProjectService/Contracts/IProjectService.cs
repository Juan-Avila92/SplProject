using Application.DashboardService.DTOs;
using Application.ProjectService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProjectService.Contracts
{
    public interface IProjectService
    {
        public Task<ProjectResponse> CreateAsync(
            string userId,
            CreateProjectRequest request,
            CancellationToken cancellationToken = default);
    }
}
