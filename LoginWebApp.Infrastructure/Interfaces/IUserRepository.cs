using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using LoginWebApp.Core.ViewModel;
namespace LoginWebApp.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        DataTable GetDataTable(string sqlQuery, List<SqlParameter> mySqlParams);
        int ExecuteQuery(string sqlQuery, List<SqlParameter> mySqlParams);
        bool InsertUser(RegisterViewModel user);
        List<SelectListItem> GetNationalities();
        int AuthenticateUser(string userName, string password);
        void CreateTask(TaskViewModel task, int userId, int? assignedUserId);

        List<TaskViewModel> GetTasksForUser(int userId);
        TaskViewModel GetTaskById(int taskId);

        List<SelectListItem> GetUserById();
        //List<SelectListItem> GetUserById();
        //void SoftDeleteTask(int taskId);
        bool SoftDeleteTask(int taskId);
        public void UpdateTask(TaskViewModel task, int userId);

        List<TaskViewModel> GetAssignedTasksForUser(int userId);
    }
}




       
