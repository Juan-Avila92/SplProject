using Application.DashboardService.Contracts;
using Application.DashboardService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(
        string userId,
        string userName,
        string role,
        CancellationToken cancellationToken = default);
}

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IProjectRepository _projectRepository;

    public DashboardService(
        IDashboardRepository dashboardRepository,
        IProjectRepository projectRepository)
    {
        _dashboardRepository = dashboardRepository;
        _projectRepository = projectRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        string userId,
        string userName,
        string role,
        CancellationToken cancellationToken = default)
    {
        var dashboardTask = _dashboardRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        var projectsTask = _projectRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        await Task.WhenAll(dashboardTask, projectsTask);

        var dashboard = await dashboardTask;
        var projects = await projectsTask;

        return new DashboardResponse
        {
            Id = dashboard?.Id ?? Guid.NewGuid(),
            UserId = userId,
            UserName = userName ?? string.Empty,
            Role = role ?? string.Empty,
            TotalProjects = projects.Count,
            LastUpdated = dashboard?.LastUpdated ?? DateTime.UtcNow,

            Projects = projects
                .Select(project => new ProjectResponse
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    UpdatedAt = project.UpdatedAt
                })
                .ToList()
        };
    }
}
