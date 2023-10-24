using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace LoginWebApp.Core.ViewModel
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "EstimatedHours is required.")]
        public double EstimatedHours { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public int? AssignedUserId { get; set; }
    }
}
public enum TaskStatus
{
    New,
    Open,
    Assigned,
    InProgress,
    Completed,
    Closed
}