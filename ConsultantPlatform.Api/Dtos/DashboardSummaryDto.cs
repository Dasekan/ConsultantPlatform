namespace ConsultantPlatform.Api.Dtos.Dashboard
{
    public class DashboardSummaryDto
    {
        public CustomerStatsDto Customers { get; set; } = new();
        public ProjectStatsDto Projects { get; set; } = new();
        public List<UpcomingProjectDto> UpcomingDeadlines { get; set; } = new();
        public List<RecentActivityDto> RecentActivity { get; set; } = new();
    }

    public class CustomerStatsDto
    {
        public int Total { get; set; }
        public int Lead { get; set; }
        public int Active { get; set; }
        public int Completed { get; set; }
        public int ProjectClient { get; set; }

        // Hvis du har "Inactive" i dit system (som på screenshot), kan du også vise den:
        public int Inactive { get; set; }
    }

    public class ProjectStatsDto
    {
        public int Total { get; set; }
        public int Lead { get; set; }
        public int Active { get; set; }
        public int Completed { get; set; }
        public int Invoiced { get; set; }
    }

    public class UpcomingProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "";
    }

    public class RecentActivityDto
    {
        public string EntityType { get; set; } = "";
        public int EntityId { get; set; }
        public int? RelatedCustomerId { get; set; }
        public string Action { get; set; } = "";
        public string Summary { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public DateTime CreatedUtc { get; set; }
    }
}
