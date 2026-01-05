using Microsoft.AspNetCore.Mvc;
using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;


namespace TaskManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string section, int? taskId)
        {
            ViewBag.Section = section ?? "overview";

            ViewBag.TotalTasks = _db.Tasks.Count();
            ViewBag.Completed = _db.Tasks.Count(t => t.Status == "Completed");
            ViewBag.Pending = _db.Tasks.Count(t => t.Status == "Pending");
            ViewBag.InProgress = _db.Tasks.Count(t => t.Status == "In Progress");
            ViewBag.Overdue = _db.Tasks.Count(t => t.DueDate < DateTime.Now && t.Status != "Completed");

            ViewBag.Users = _db.Users
                .Where(u => u.HasLoggedIn)
                .ToList();
                ViewBag.Tasks = _db.Tasks.ToList();

            var tasks = _db.Tasks
                .Include(t => t.AssignedUser)
                .ToList();

            if (section == "edit" && taskId.HasValue)
            {
                var taskToEdit = _db.Tasks.FirstOrDefault(t => t.Id == taskId.Value);
                if (taskToEdit == null) return NotFound();
                ViewBag.EditTask = taskToEdit;
            }
            return View(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TaskItem task)
        {
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard", new { section = "TaskList" }); 
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int taskId, int userId, string status, string assignDate, string dueDate)
        {
            var task = await _db.Tasks.FindAsync(taskId);
            if (task == null) return NotFound();

            task.AssignedToId = userId;
            task.Status = status;
            
            if (!string.IsNullOrEmpty(assignDate) && DateTime.TryParse(assignDate, out var parsedAssignDate))
            {
                task.AssignDate = parsedAssignDate;
            }
            
            if (!string.IsNullOrEmpty(dueDate) && DateTime.TryParse(dueDate, out var parsedDueDate))
            {
                task.DueDate = parsedDueDate;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard", new { section = "list" });
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null) return NotFound();
            
            // var users = await _db.Users.ToListAsync();
            // ViewBag.Users = users;
            
            return View(task);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(TaskItem model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Dashboard", new { section = "edit", taskId = model.Id });
            }
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == model.Id); //Finds the existing task in the database with the same Id as the form submission
            if (task == null)
                return NotFound();

            task.Title = model.Title;
            task.Description = model.Description;
            task.AssignDate = model.AssignDate;
            task.DueDate = model.DueDate;
            task.Priority = model.Priority;
            task.Status = model.Status;
            task.AssignedToId = model.AssignedToId;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard", new { section = "list" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task != null)
            {
                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard", new { section = "list" });
        }
    }
}
