using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TaskManagement.Models
{
    public class Users : IdentityUser<int> 
    {
        public string Name { get; set; } = string.Empty;
        // public string Role { get; set; } = string.Empty;
        public bool HasLoggedIn { get; set; } = false;
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}
