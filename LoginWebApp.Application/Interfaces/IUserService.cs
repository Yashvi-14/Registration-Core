using System.Data;
using LoginWebApp.Core.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LoginWebApp.Application.Interfaces
{
    public interface IUserService
    {
        DataTable GetUserData();
        int UpdateUserData(string newData);

        bool RegisterUser(RegisterViewModel model);
        List<SelectListItem> GetNationalities();
        int AuthenticateUser(string userName, string password);
        void CreateTask(TaskViewModel task, int userId, int? assignedUserId);
        List<TaskViewModel> GetTasksForUser(int userId);
       
        List<SelectListItem> GetUserById();
        TaskViewModel GetTaskById(int taskId);
        void UpdateTask(TaskViewModel task, int userId);

        bool SoftDeleteTask(int taskId);
        List<TaskViewModel> GetAssignedTasksForUser(int userId);



    }
}