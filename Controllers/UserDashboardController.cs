using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Controllers
{
[Authorize(Roles = "User")]
public class UserDashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<Users> _userManager;

    public UserDashboardController(ApplicationDbContext db, UserManager<Users> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string section, int? taskId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // Stats
        ViewBag.TotalTasks = _db.Tasks.Count(t => t.AssignedToId == user.Id);
        ViewBag.Completed = _db.Tasks.Count(t => t.AssignedToId == user.Id && t.Status == "Completed");
        ViewBag.Pending = _db.Tasks.Count(t => t.AssignedToId == user.Id && t.Status == "Pending");
        ViewBag.InProgress = _db.Tasks.Count(t => t.AssignedToId == user.Id && t.Status == "In Progress");
        ViewBag.Overdue = _db.Tasks.Count(t => t.AssignedToId == user.Id && t.DueDate < DateTime.Now && t.Status != "Completed");

        var userTasks = await _db.Tasks
            .Include(t => t.AssignedUser)
            .Where(t => t.AssignedToId == user.Id)
            .ToListAsync();

        ViewBag.Section = section ?? "overview";
        ViewBag.UserTasks = userTasks;
        ViewBag.CurrentUser = user;

        if (section == "details" && taskId.HasValue)
        {
            var task = userTasks.FirstOrDefault(t => t.Id == taskId.Value);
            if (task != null)
            {
                ViewBag.SelectedTask = task;
            }
        }

        return View(userTasks);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int taskId, string status, string remarks)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToId == user.Id);
        if (task == null) return NotFound();

        task.Status = status;
        task.Remarks = remarks;
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", new { section = "tasks" });
    }
}

}
