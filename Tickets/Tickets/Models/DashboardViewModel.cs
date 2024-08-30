namespace Tickets.Models
{
    public class DashboardViewModel
    {
        public int HighPriorityTickets { get; set; }
        public int MediumPriorityTickets { get; set; }
        public int LowPriorityTickets { get; set; }

        public List<StatusChartData> StatusChartData { get; set; }

        public List<TicketsCreatedByDate> TicketsCreatedByDate { get; set; }
    }

    public class StatusChartData
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class TicketsCreatedByDate
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
