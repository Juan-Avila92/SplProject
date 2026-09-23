using Application.DashboardService.Contracts;
using Application.DashboardService.DTOs;
using Application.ProjectService.Contracts;
using Application.ProjectService.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProjectService.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectResponse> CreateAsync(
            string userId,
            CreateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            var project = new Project
            {
                UserId = userId,
                Name = request.Name,
                Description = request.Description
            };

            var createdProject = await _projectRepository.CreateAsync(
                project,
                cancellationToken);

            return new ProjectResponse
            {
                Id = createdProject.Id,
                Name = createdProject.Name,
                Description = createdProject.Description,
                CreatedAt = createdProject.CreatedAt,
                UpdatedAt = createdProject.UpdatedAt
            };
        }
    }
}
