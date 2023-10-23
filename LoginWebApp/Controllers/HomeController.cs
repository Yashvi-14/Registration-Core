using LoginWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using LoginWebApp.Core.ViewModel;
using LoginWebApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;


namespace LoginWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUserService _userService; 

        public HomeController(IUserService userService, ILogger<HomeController> logger)
        {
           
            _logger = logger;
            _userService = userService;
        }
        /*    [HttpGet]
            public IActionResult Index()
            {
                string userIdString = HttpContext.Session.GetString("IdUser");

                if (int.TryParse(userIdString, out int userId))
                {
                    List<TaskViewModel> userTasks = _userService.GetTasksForUser(userId);


                    return View(userTasks);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }*/
        [HttpGet]
        public IActionResult Index()
        {
            string userIdString = HttpContext.Session.GetString("IdUser");

            if (int.TryParse(userIdString, out int userId))
            {
               
                List<TaskViewModel> userTasks = _userService.GetTasksForUser(userId);
               
                List<TaskViewModel> assignedTasks = _userService.GetAssignedTasksForUser(userId);

                List<TaskViewModel> allTasks = userTasks.Concat(assignedTasks).ToList();

                return View(allTasks);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Register()
        {
            ViewBag.Nationalities = _userService.GetNationalities();
            return View();
        }


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Nationalities = _userService.GetNationalities();
                return View(model);
            }

            bool registrationResult = _userService.RegisterUser(model);

            if (registrationResult)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Nationalities = _userService.GetNationalities();
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                return View(model);
            }
        }
    
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            int authenticationResult = _userService.AuthenticateUser(model.UserName, model.Password);

            if (authenticationResult > 0)
            {
                
                HttpContext.Session.SetString("IdUser", authenticationResult.ToString());
                TempData["LoginSuccess"] = "Login Successful";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Authentication failed. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Task()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Task(TaskViewModel task,int? assignedUserId)
        {
            string userIdString = HttpContext.Session.GetString("IdUser");
            if (int.TryParse(userIdString, out int userId))
            {
                task.Status = TaskStatus.New.ToString();
                _userService.CreateTask(task, userId, assignedUserId);

                return RedirectToAction("Index");
            }
            else
            {
               
                return RedirectToAction("Login");
            }
        }






        /*  [HttpGet]
          public IActionResult Edit(int taskId)
          {
              var task = _userService.GetTaskById(taskId);

              if (task == null)
              {
                  return NotFound();
              }

              var users = _userService.GetUserById();
              ViewBag.Users = users.Select(user => new SelectListItem
              {
                  Value = user.Value.ToString(),
                  Text = user.Text
              }).ToList();

              return View(task);
          }*/

        [HttpGet]
        public IActionResult Edit(int taskId)
        {
            var task = _userService.GetTaskById(taskId);

            if (task == null)
            {

                return NotFound();
            }


            var users = _userService.GetUserById();
            ViewBag.Users = new SelectList(users, "Value", "Text");


            return View(task);
        }

        [HttpPost]
        public IActionResult Edit(TaskViewModel editedTask)
        {
            try
            {
                TaskViewModel task = _userService.GetTaskById(editedTask.Id);

                if (task != null)
                {

                    int assignedUserId = editedTask.AssignedUserId ?? 0;

                    int loggedInUserId = int.Parse(HttpContext.Session.GetString("IdUser"));
                    if (task.AssignedUserId != assignedUserId)
                    {
                        task.AssignedUserId = assignedUserId;
                        task.Status = "Open";
                    }

                   
                        task.Status = editedTask.Status;
                    

                    task.AssignedUserId = assignedUserId;
                    _userService.UpdateTask(task, loggedInUserId);

                    ViewBag.TaskID = editedTask.Id;
                    ViewBag.AssignToUserId = assignedUserId;


                    return RedirectToAction("Index");
                  
                }
                else
                {
                    return RedirectToAction("TaskNotFound");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);
                return RedirectToAction("DeleteError");
            }
        }






        public IActionResult Delete(int taskId)
        {
            bool deletionResult = _userService.SoftDeleteTask(taskId);

            if (deletionResult)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "Error while deleting"; 
            return View("Index");

        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
