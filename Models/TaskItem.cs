using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; }= string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime AssignDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }= string.Empty;
        public string Status { get; set; }= string.Empty;
        public string Remarks { get; set; }= string.Empty;  
        public int? AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public Users? AssignedUser { get; set; }
    }
}
