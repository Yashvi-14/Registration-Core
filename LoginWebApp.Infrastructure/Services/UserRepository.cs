using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LoginWebApp.Infrastructure.Interfaces;
using LoginWebApp.Infrastructure.DataHelpers;
using LoginWebApp.Core.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LoginWebApp.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DBHelper _dbHelper;
        private readonly IConfiguration _configuration;

        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DBHelper dbHelper, IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _dbHelper = dbHelper;
            _configuration = configuration;
            _logger = logger; // Add this line to inject the logger
        }


        public DataTable GetDataTable(string sqlQuery, List<SqlParameter> mySqlParams)
        {
            using (SqlConnection conn = _dbHelper.GetConnection())
            {
                DataTable dataTable = new DataTable();

                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                if (mySqlParams != null && mySqlParams.Count > 0)
                {
                    foreach (SqlParameter param in mySqlParams)
                    {
                        cmd.Parameters.Add(param);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
                conn.Close();

                return dataTable;
            }
        }

        public int ExecuteQuery(string sqlQuery, List<SqlParameter> mySqlParams)
        {
            using (SqlConnection conn = _dbHelper.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                if (mySqlParams != null && mySqlParams.Count > 0)
                {
                    foreach (SqlParameter param in mySqlParams)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                int nRowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
                return nRowsAffected;
            }
        }


        public List<SelectListItem> GetNationalities()
        {
            string connectionString = _configuration.GetConnectionString("DBConnectionStr"); 
            string query = "SELECT IdCountry, CountryName FROM CountryMaster";

            List<SelectListItem> nationalities = new List<SelectListItem>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nationalities.Add(new SelectListItem
                                {
                                    Value = reader["IdCountry"].ToString(),
                                    Text = reader["CountryName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return nationalities;
        }

        public bool InsertUser(RegisterViewModel user)
        {
            //GetNationalities();
            try
            {
                string query = "INSERT INTO UserMaster_Yashvi (FirstName, MiddleName, LastName, Email, DateOfBirth, PhoneNumber, Nationality, UserName, Password, CreatedBy, CreatedOn)" +
                    "                              VALUES (@FirstName, @MiddleName, @LastName, @Email, @DateOfBirth, @PhoneNumber, @Nationality, @UserName, @Password, @CreatedBy, @CreatedOn)";

                var mySqlParams = new List<SqlParameter>
                {
                    new SqlParameter("@FirstName", user.FirstName),
                    new SqlParameter("@MiddleName", string.IsNullOrEmpty(user.MiddleName) ? DBNull.Value : (object)user.MiddleName),
                    new SqlParameter("@LastName", user.LastName),
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@DateOfBirth", user.DateOfBirth),
                    new SqlParameter("@PhoneNumber", user.PhoneNumber),
                    new SqlParameter("@Nationality", user.Nationality),
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@Password", user.Password),
                    new SqlParameter("@CreatedBy", 111),
                    new SqlParameter("@CreatedOn", DateTime.Now)
                };

                int affectedRows = ExecuteQuery(query, mySqlParams);

                return affectedRows > 0;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public int AuthenticateUser(string userName, string password)
        {
            int userId = 0;
            string query = "SELECT IdUser FROM UserMaster_Yashvi WHERE UserName = @UserName AND Password = @Password";
            var mySqlParams = new List<SqlParameter>
    {
        new SqlParameter("@UserName", userName),
        new SqlParameter("@Password", password)
    };

            DataTable result = GetDataTable(query, mySqlParams);

            if (result.Rows.Count > 0)
            {
                userId = Convert.ToInt32(result.Rows[0]["IdUser"]);
                return userId;
            }
            else
            {
                return -1;
            }
        }

        public void CreateTask(TaskViewModel task, int userId, int? assignedUserId)
        {
            try
            {
               

                string status = (assignedUserId == null) ? "New" : "Assigned";

                string query = "INSERT INTO Task_Yashvi (Title, Description, Priority, EstimatedHours, Status, IdUser, AssignedUserId) " +
                               "VALUES (@Title, @Description, @Priority, @EstimatedHours, @Status, @IdUser, @AssignedUserId)";


                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Title", task.Title),
            new SqlParameter("@Description", task.Description),
            new SqlParameter("@Priority", task.Priority),
            new SqlParameter("@EstimatedHours", task.EstimatedHours),
            new SqlParameter("@Status", status), 
            new SqlParameter("@IdUser", userId),
            new SqlParameter("@AssignedUserId", assignedUserId ?? (object)DBNull.Value)
        };

                ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Error in CreateTask: " + ex.Message);
            }
        }




        public List<TaskViewModel> GetTasksForUser(int userId)
        {
            string query = "SELECT * FROM Task_Yashvi WHERE IdUser = @UserId AND DeleteAt IS NULL";
            var mySqlParams = new List<SqlParameter>
    {
        new SqlParameter("@UserId", userId)
    };

            DataTable result = GetDataTable(query, mySqlParams);

            List<TaskViewModel> tasks = new List<TaskViewModel>();

            foreach (DataRow row in result.Rows)
            {
                int? assignedUserId = row["AssignedUserId"] == DBNull.Value ? (int?)null : (int?)Convert.ToInt32(row["AssignedUserId"]);

                TaskViewModel task = new TaskViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    Priority = row["Priority"].ToString(),
                    EstimatedHours = Convert.ToInt32(row["EstimatedHours"]),
                    Status = row["Status"].ToString(),
                    AssignedUserId = assignedUserId,
                    UserId = userId
                };

                tasks.Add(task);
            }

            return tasks;
        }


        public List<SelectListItem> GetUserById()
        {
            string query = "SELECT IdUser, UserName FROM UserMaster_Yashvi";
            var mySqlParams = new List<SqlParameter>();

            DataTable result = GetDataTable(query, mySqlParams);

            List<SelectListItem> userSelectList = new List<SelectListItem>();

            foreach (DataRow row in result.Rows)
            {
                RegisterViewModel user = new RegisterViewModel
                {
                    IdUser = Convert.ToInt32(row["IdUser"]),
                    UserName = row["UserName"].ToString(),
                };

                userSelectList.Add(new SelectListItem
                {
                    Value = user.IdUser.ToString(),
                    Text = user.UserName
                });
            }

            return userSelectList;
        }


       
        public TaskViewModel GetTaskById(int taskId)
        {
            try
            {
                string query = "SELECT * FROM Task_Yashvi WHERE Id = @Id";
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Id", taskId)
        };

                DataTable result = GetDataTable(query, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];

                    TaskViewModel task = new TaskViewModel
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Title = row["Title"].ToString(),
                        Description = row["Description"].ToString(),
                        Priority = row["Priority"].ToString(),
                        EstimatedHours = Convert.ToInt32(row["EstimatedHours"]),
                        //Status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Open",
                        Status = row["Status"].ToString(),
                        UserId = Convert.ToInt32(row["IdUser"]),
                       // AssignedUserId = Convert.ToInt32(row["AssignedUserId"])
                         AssignedUserId = row["AssignedUserId"] != DBNull.Value ? Convert.ToInt32(row["AssignedUserId"]) : (int?)null
                    };

                    return task;
                }
                else
                {
                   
                    return null;
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        public void UpdateTask(TaskViewModel task, int userId)
        {
            try
            {
                TaskViewModel existingTask = GetTaskById(task.Id);

                if (existingTask != null)
                {
                    string query = "UPDATE Task_Yashvi SET Title = @Title, Description = @Description, Priority = @Priority, EstimatedHours = @EstimatedHours, Status = @Status, AssignedUserId = @AssignedUserId WHERE Id = @Id";

                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", task.Id),
                new SqlParameter("@Title", task.Title),
                new SqlParameter("@Description", task.Description),
                new SqlParameter("@Priority", task.Priority),
                new SqlParameter("@EstimatedHours", task.EstimatedHours),
                //new SqlParameter("@Status", string.IsNullOrWhiteSpace(task.Status) ? "Assigned" : task.Status),
                new SqlParameter("@Status", string.IsNullOrWhiteSpace(task.Status) ? (object)"Open" : task.Status),
                new SqlParameter("@AssignedUserId", task.AssignedUserId ?? (object)DBNull.Value)
            };

                    int affectedRows = ExecuteQuery(query, parameters);

                    if (affectedRows > 0)
                    {
                        Console.WriteLine("Task updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Task update failed. No rows affected.");
                    }
                }
                else
                {
                    Console.WriteLine("Task not found. Update failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public List<TaskViewModel> GetAssignedTasksForUser( int userId)
        {

            string query = "SELECT * FROM Task_Yashvi WHERE AssignedUserId = @UserId";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable result = GetDataTable(query, parameters);


            List<TaskViewModel> assignedTasks = new List<TaskViewModel>();

            foreach (DataRow row in result.Rows)
            {
                /*string statusFromDatabase = row["Status"].ToString();
                string status = string.IsNullOrWhiteSpace(statusFromDatabase) ? "open" : statusFromDatabase;*/
                TaskViewModel task = new TaskViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    Priority = row["Priority"].ToString(),
                    EstimatedHours = Convert.ToInt32(row["EstimatedHours"]),
                    Status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Open"

                };
               
                assignedTasks.Add(task);
            }

            return assignedTasks;
        }






        public bool SoftDeleteTask(int taskId)
        {
            try
            {
                string softDeleteTaskQuery = "UPDATE  Task_Yashvi SET DeleteAt = GETDATE() WHERE Id = @Id AND Status = 'New'";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", taskId)
                };

                int affectedRows = ExecuteQuery(softDeleteTaskQuery, parameters);

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in SoftDeleteTask: {ex.Message}");
                throw;
            }
        }

    }
}
