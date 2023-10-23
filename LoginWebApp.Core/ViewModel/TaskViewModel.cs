using System;
using System.Collections.Generic;
using System.Text;

namespace LoginWebApp.Core.ViewModel
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
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