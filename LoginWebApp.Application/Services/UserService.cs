using System;
using System.Data;
using System.Data.SqlClient;
using LoginWebApp.Application.Services;
using LoginWebApp.Infrastructure.Services;
using LoginWebApp.Application.Interfaces;
using System.Collections.Generic;
using LoginWebApp.Core.ViewModel;
using LoginWebApp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;


namespace LoginWebApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public DataTable GetUserData()
        {
            
            return _userRepository.GetDataTable("SELECT * FROM UserData", null);
        }
        public bool RegisterUser(RegisterViewModel user)
        {
            try
            {
                return _userRepository.InsertUser(user);
            }
            catch (Exception)
            {
                
                return false;
            }
        }
        public List<SelectListItem> GetNationalities()
        {
            return _userRepository.GetNationalities();
        }
        public LoginViewModel AuthenticateUser(string userName, string password)
        {
            
            return _userRepository.AuthenticateUser(userName, password);
        }


        public void CreateTask(TaskViewModel task, int userId, int? assignedUserId)
        {
            
                _userRepository.CreateTask(task, userId,assignedUserId);
        }




        public List<TaskViewModel> GetTasksForUser(int userId)
        {
           return _userRepository.GetTasksForUser(userId);
        }

        public List<TaskViewModel> GetAssignedTasksForUser(int userId)
        {
            var tasks = _userRepository.GetAssignedTasksForUser(userId);
            foreach (var task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task.Status))
                {
                    task.Status = TaskStatus.Open.ToString();
                }
                else
                {
                    
                    if (!string.IsNullOrWhiteSpace(task.Status))
                    {
                        task.Status = task.Status;
                    }
                }
            }
            return tasks; 
        }


        public bool SoftDeleteTask(int taskId)
        {
            try
            {
                return _userRepository.SoftDeleteTask(taskId);
            }
            catch (Exception)
            {

                return false;
            }

            
        }
        public TaskViewModel GetTaskById(int taskId)
        {


            return _userRepository.GetTaskById(taskId);
        }

        public void UpdateTask(TaskViewModel task, int userId)
        {
            
            TaskViewModel existingTask = _userRepository.GetTaskById(task.Id);

            if (existingTask != null)
            {
                task.UserId = userId; 
                _userRepository.UpdateTask(task, userId);
            }
            else
            {
                Console.WriteLine("Task not found. Update failed.");
            }
        }

        /* public List<SelectListItem> GetUserById(int userId)
         {
             return _userRepository.GetUserById(userId);
         }*/
        public List<SelectListItem> GetUserById()
        {
            return _userRepository.GetUserById();
        }

      

        public int UpdateUserData(string newData)
        {
            
            return _userRepository.ExecuteQuery("UPDATE UserData SET Data = @newData", new List<SqlParameter>
            {
                new SqlParameter("@newData", newData)
            });
        }
    }
}
