using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DashboardOverview
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = null!;

        public int TotalProjects { get; set; }

        public DateTime LastUpdated { get; set; }

         public List<Project> Projects { get; set; } = new List<Project>();
    }
}
