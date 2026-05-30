using System;

namespace TodoApp.WinForms.Models
{
    public sealed class TodoTaskEntity : BaseEntity
    {
        public string Description { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int PriorityId { get; set; }
        public string PriorityName { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
