using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TasksManagmentAPI.Models;

namespace TasksManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private const string ERROR_TEMPLATE = "Error while executing {0} api.";
        private const string ERROR_MESSAGE_NO_DATA = "No data";
        private const string CACHE_ENTRY_KEY_TASKS_DATA = "tasks";
        private const string BAD_REQUEST_MESSAGE_TASKENTITY = "One or more fileds on the request invalid. Details : {0}";
        private const string VALIDATION_MESSAGE_TASK_ID = "The task field 'id' is invalid";
        private const string VALIDATION_MESSAGE_TASK_NAME = "The task field 'name' is unset or empty";        
        private const string VALIDATION_MESSAGE_TASK_PRIORITY = "The task field 'Priority' is invalid";
        private const string VALIDATION_MESSAGE_TASK_STATUS = "The task field 'Status' is invalid";
        private const string VALIDATION_MESSAGE_TASK_TODODATE = "The task field 'TodoDate' is invalid";

        private readonly IMemoryCache _memoryCache;
        public TasksController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
       
        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TaskEntity> Get()
        {
            try
            {
                var tasks = RetrieveTaskListFromMemory();
                if (tasks!=null)
                {
                    return Ok(tasks);
                }

                return Ok(new List<TaskEntity>());
            }
            catch (Exception ex)
            {

                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Get]"),ex);
            }
        }

        // GET: api/Tasks/5
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TaskEntity> Get(int id)
        {
            try
            {
                var tasks = RetrieveTaskListFromMemory();
                if (tasks == null)
                {
                    throw new Exception(ERROR_MESSAGE_NO_DATA);
                }
                var task = tasks.FirstOrDefault<TaskEntity>(t => t.Id == id);
                if (task == null)
                {
                    return NotFound();
                }
                return task;
            }
            catch (Exception ex)
            {

                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Get(int id)]"), ex);
            }

        }

        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TaskEntity>> Post([FromBody] TaskEntity task)
        {
            try
            {
                if (!TryValidateTaskModel(task, out string errorMessages))
                {
                    return BadRequest(errorMessages);
                }
                var tasks = RetrieveTaskListFromMemory();
                int newTaskId = 0;
                if (tasks != null && tasks.Any())
                {
                    int currentTaskId = tasks.Max(t => t.Id);
                    newTaskId = currentTaskId + 1;                                       
                }
                else {
                    newTaskId = 1;
                    tasks = new List<TaskEntity>(); 
                }
                task.Id = newTaskId;
                tasks.Add(task);
                tasks = GetOrderedList(tasks);
                _memoryCache.Set<List<TaskEntity>>(CACHE_ENTRY_KEY_TASKS_DATA, tasks, GetMemoryCacheEntryOptions());
                //tasks = RetrieveTaskListFromMemory();
                return CreatedAtAction(nameof(Get), null, tasks);
            }
            catch (Exception ex)
            {

                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Post]"), ex);
            }

        }


        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TaskEntity>> Put(int id, [FromBody] TaskEntity task)
        {
           
            try
            {
                if (!TryValidateTaskModel(task, out string errorMessages))
                {
                    return BadRequest(errorMessages);
                }
                
                var tasks = RetrieveTaskListFromMemory();
                if (tasks == null)
                {
                    throw new Exception(ERROR_MESSAGE_NO_DATA);
                }
                var taskToUpdate = tasks.FirstOrDefault(t => t.Id == id);
                if (taskToUpdate == null)
                {
                    return NotFound();
                }
                taskToUpdate.Name = task.Name;
                taskToUpdate.TaskPriority = task.TaskPriority;
                taskToUpdate.TaskStatus = task.TaskStatus;
                taskToUpdate.TodoDate = task.TodoDate;
                tasks = GetOrderedList(tasks);
                _memoryCache.Set<List<TaskEntity>>(CACHE_ENTRY_KEY_TASKS_DATA, tasks, GetMemoryCacheEntryOptions());
                return Ok(tasks);
                
            }
            catch (Exception ex)
            {

                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Put]"), ex);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TaskEntity>> Delete(int id)
        {
            try
            {

                var tasks = RetrieveTaskListFromMemory();
                if (tasks == null)
                {
                    throw new Exception(ERROR_MESSAGE_NO_DATA);
                }
                var taskToDelete = tasks.FirstOrDefault(t => t.Id == id);
                if (taskToDelete == null)
                {
                    return NotFound();
                }
                tasks.Remove(taskToDelete);
                tasks = GetOrderedList(tasks);
                _memoryCache.Set<List<TaskEntity>>(CACHE_ENTRY_KEY_TASKS_DATA, tasks, GetMemoryCacheEntryOptions());
                return Ok(tasks);
            }
            catch (Exception ex)
            {

                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Delete]"), ex);
            }
        }

        // Patch: api/ApiWithActions/5
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<TaskEntity>> Patch(int id)
        {
            try
            {

                var tasks = RetrieveTaskListFromMemory();
                if (tasks == null)
                {
                    throw new Exception(ERROR_MESSAGE_NO_DATA);
                }
                var taskToPatch = tasks.FirstOrDefault(t => t.Id == id);
                if (taskToPatch == null)
                {
                    return NotFound();
                }
                taskToPatch.TaskStatus = Enums.Status.Close;
                tasks = GetOrderedList(tasks);
                _memoryCache.Set<List<TaskEntity>>(CACHE_ENTRY_KEY_TASKS_DATA, tasks, GetMemoryCacheEntryOptions());
                return Ok(tasks);

            }
            catch (Exception ex)
            {
                throw new AggregateException(string.Format(ERROR_TEMPLATE, "api/Tasks[Patch]"), ex);
            }
        }
        #region Private methods
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions() { Size = 1024, SlidingExpiration = TimeSpan.FromDays(1) };
        }
        private List<TaskEntity> RetrieveTaskListFromMemory()
        {
            if (_memoryCache.TryGetValue(CACHE_ENTRY_KEY_TASKS_DATA, out List<TaskEntity> result))
            {
                return result;
            }
            return null;
        }
        private bool TryValidateTaskModel(TaskEntity task, out string errorMessages)
        {
            var t = new TaskEntity();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            bool valid = true;
            if (task.Id < 0)
            {
                valid = false;
                sb.Append(VALIDATION_MESSAGE_TASK_ID);
                sb.AppendLine();
            }
            if (string.IsNullOrEmpty(task.Name))
            {
                valid = false;
                sb.Append(VALIDATION_MESSAGE_TASK_NAME);
                sb.AppendLine();
            }
            //Bacause enum represented by integer and in case empty or not found in desrialization  the value is 0.
            if (!Enum.TryParse(typeof(Enums.Priority), task.TaskPriority.ToString(), out object p) || p.ToString() == "0")
            {
                valid = false;
                sb.Append(VALIDATION_MESSAGE_TASK_PRIORITY);
                sb.AppendLine();
            }
            //Bacause enum represented by integer and in case empty or not found in desrialization  the value is 0.
            if (!Enum.TryParse(typeof(Enums.Status), task.TaskStatus.ToString(), out object s) || s.ToString() == "0")
            {
                valid = false;
                sb.Append(VALIDATION_MESSAGE_TASK_STATUS);
                sb.AppendLine();
            }
            if (task.TodoDate == DateTime.MinValue)
            {
                valid = false;
                sb.Append(VALIDATION_MESSAGE_TASK_TODODATE);
                sb.AppendLine();
            }
            errorMessages = sb.ToString();
            return valid;
        }

        private List<TaskEntity> GetOrderedList(List<TaskEntity> tasks)
        {
            return tasks.OrderBy(t => (int)t.TaskStatus).ThenBy(t => (int)t.TaskPriority).ThenBy(t => t.TodoDate).ToList();
        }



        #endregion
    }
}
