using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksManagmentAPI.Enums;

namespace TasksManagmentAPI.Models
{
    public class TaskEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime TodoDate { get; set; }
        public Priority TaskPriority { get; set; }
        public Status TaskStatus{ get; set; }
    }
}
