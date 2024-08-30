using Microsoft.AspNetCore.Mvc;
using Tickets.Models;
using System.Threading.Tasks;
using YourNamespace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Tickets.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// GET: Ticket/Create
        //public IActionResult Create()
        //{
        //    // Populate Status options for dropdown
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });

        //    return View();
        //}
        // GET: Ticket/Create
        public IActionResult Create()
        {
            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email // or any other user property you want to display
            }).ToList();

            return View();
        }


        //// POST: Ticket/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Ticket ticket)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(ticket);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Re-populate Status options if validation fails
        //    ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
        //                                .Cast<StatusEnum>()
        //                                .Select(e => new SelectListItem
        //                                {
        //                                    Value = e.ToString(),
        //                                    Text = e.ToString()
        //                                });

        //    return View(ticket);
        //}
        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate Status options if validation fails
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });

            return View(ticket);
        }

        // GET: Ticket/Index
        public async Task<IActionResult> Index()
        {

            var tickets = await _context.Tickets
       .Select(t => new
       {
           t.Id,
           t.TicketNo,
           t.Subject,
           t.TicketBody,
           CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
           t.Priority,
           Deadline = t.Deadline.HasValue ? t.Deadline.Value.ToString("yyyy-MM-dd HH:mm:ss") : "No Deadline",
           t.Status,
           t.AssignedUserId
       })
       .ToListAsync();
            return View(await _context.Tickets.ToListAsync());
        }

        // GET: Ticket/PriorityDistribution
        public async Task<IActionResult> PriorityDistribution()
        {
            // Fetch data from the database
            var highPriorityTickets = await _context.Tickets.Where(t => t.Priority == "High").ToListAsync();
            var mediumPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Medium").ToListAsync();
            var lowPriorityTickets = await _context.Tickets.Where(t => t.Priority == "Low").ToListAsync();

            var model = new TicketPriorityViewModel
            {
                HighPriorityCount = highPriorityTickets.Count,
                MediumPriorityCount = mediumPriorityTickets.Count,
                LowPriorityCount = lowPriorityTickets.Count,
                HighPriorityTickets = highPriorityTickets,
                MediumPriorityTickets = mediumPriorityTickets,
                LowPriorityTickets = lowPriorityTickets
            };

            return View(model);
        }

        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Populate Status options for dropdown
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });
            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email // or any other user property you want to display
            }).ToList();



            return View(ticket);
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketNo,Subject,TicketBody,Priority,Status,AssignedUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-populate Status options if validation fails
            ViewBag.StatusOptions = Enum.GetValues(typeof(StatusEnum))
                                        .Cast<StatusEnum>()
                                        .Select(e => new SelectListItem
                                        {
                                            Value = e.ToString(),
                                            Text = e.ToString()
                                        });
            // Populate Users for dropdown
            ViewBag.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email // or any other user property you want to display
            }).ToList();



            return View(ticket);
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        // GET: Ticket/Status
        public IActionResult Status()
        {
            return View();
        }

        // GET: Ticket/GetStatusData
        public JsonResult GetStatusData()
        {
            var statusCounts = _context.Tickets
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            return Json(statusCounts);
        }
        [HttpPost]
        public async Task<IActionResult> SetDeadlines(List<int> ticketIds, int days)
        {
            var tickets = await _context.Tickets
                .Where(t => ticketIds.Contains(t.Id))
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.Deadline = DateTime.Now.AddDays(days);
            }

            _context.Tickets.UpdateRange(tickets);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Or another view
        }
        public IActionResult SetDeadlinesView()
        {
            var tickets = _context.Tickets.ToList();
            return View("SetDeadlines", tickets);
        }

        [HttpGet]
        public IActionResult GetTicketsByStatus(string status)
        {
            // Convert the status from string to StatusEnum
            if (!Enum.TryParse(status, out StatusEnum statusEnum))
            {
                // If parsing fails, return an empty list or handle the error as needed
                return Json(new List<object>());
            }

            var tickets = _context.Tickets
                .Where(t => t.Status == statusEnum) // Use the enum for comparison
                .Select(t => new
                {
                    TicketNo = t.TicketNo,
                    Subject = t.Subject,
                    TicketBody = t.TicketBody,
                    CreatedAt = t.CreatedAt,
                    Priority = t.Priority,
                    Status = t.Status
                })
                .ToList();

            return Json(tickets);
        }

    }
}
