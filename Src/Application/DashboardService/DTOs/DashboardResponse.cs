using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DashboardService.DTOs
{
    public class DashboardResponse
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public int TotalProjects { get; set; }

        public DateTime LastUpdated { get; set; }

        public List<ProjectResponse> Projects { get; set; } = new();
    }
}
