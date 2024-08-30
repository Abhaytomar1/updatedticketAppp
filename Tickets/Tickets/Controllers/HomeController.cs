using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Tickets.Models;
using YourNamespace.Data;

namespace Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Add this field

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Dashboard()
        {
            // Check if the user is logged in by verifying the session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch ticket data from the database
            var tickets = _context.Tickets.ToList();

            // Group tickets by priority
            var highPriorityCount = tickets.Count(t => t.Priority == "High");
            var mediumPriorityCount = tickets.Count(t => t.Priority == "Medium");
            var lowPriorityCount = tickets.Count(t => t.Priority == "Low");

            // Group tickets by status
            var statusChartData = tickets
                .GroupBy(t => t.Status)
                .Select(g => new StatusChartData
                {
                    Status = g.Key.ToString(),  // Convert status enum to string
                    Count = g.Count()
                })
                .ToList();

            // Group tickets by creation date
            var ticketsCreatedByDate = tickets
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new TicketsCreatedByDate
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();
            // Prepare the view model with the ticket data
            var dashboardViewModel = new DashboardViewModel
            {
                HighPriorityTickets = highPriorityCount,
                MediumPriorityTickets = mediumPriorityCount,
                LowPriorityTickets = lowPriorityCount,
                StatusChartData = statusChartData,  // Set the status chart data
                TicketsCreatedByDate = ticketsCreatedByDate
            };

            return View(dashboardViewModel);
        }

        // GET: /Home/TeamLeadDashboard
        public IActionResult TeamLeadDashboard()
        {
            // Team Lead dashboard logic here
            return View();
        }

        
        public async Task<IActionResult> UserDashboard(int Id)
        {
            
             var tickets = await _context.Tickets
            .Where(t => t.AssignedUserId == Id)  // Adjust this if the type is different
            .ToListAsync();

        return View(tickets);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
