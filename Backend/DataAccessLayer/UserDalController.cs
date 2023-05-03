using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Data.SQLite;
using System.IO;
using log4net;
using System.Reflection;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// UserDalController class
    /// responsible for accessing the table Users of our Kanban systen DB
    /// which stores all the users in the system
    /// </summary>
    internal class UserDalController : DalController
    {
        private const string UsersTable = "Users";

        /// <summary>
        /// UserDalController constructor
        /// relates the controller to the table Users of the DB
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserDalController() : base(UsersTable)
        {

        }

        /// <summary>
        /// gets all the users dto
        /// </summary>
        /// <returns>list of all the uers dto</returns>
        public List<UserDTO> SelectAllUsers()
        {
            List<UserDTO> result = Select().Cast<UserDTO>().ToList();
            return result;
        }

        /// <summary>
        /// save user row in the table
        /// </summary>
        /// <param name="user">user we need to save</param>
        /// <returns>true if succeeded, otherwise- false</returns>
          public bool Insert(UserDTO user)
          {
              using (var connection = new SQLiteConnection(_connectionString))
              {
                  int res = -1;
                  SQLiteCommand command = new SQLiteCommand(null, connection);
                  try
                  {
                      connection.Open();
                      command.CommandText = $"INSERT INTO {UsersTable} ({DTO.IDColumnName},{UserDTO.UserEmailColumnName},{UserDTO.UserPasswordColumnName}) " +
                          $"VALUES (@idVal,@emailVal,@passwordVal);";

                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", user.Id);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", user.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter(@"passwordVal", user.Password);

                      command.Parameters.Add(idParam);
                      command.Parameters.Add(emailParam);
                      command.Parameters.Add(passwordParam);
                      command.Prepare();

                      res = command.ExecuteNonQuery();
                  }
                  catch (Exception e)
                  {
                    log.Error(e.Message);
                    throw new Exception("User insertion to kanban.db failed");
                  }
                finally
                  {
                      command.Dispose();
                      connection.Close();
                  }
                  return res > 0;
              }
          }

        
        /// <summary>
        /// conver row to user dto
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>user dto from sql</returns>
        protected override UserDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO result = new UserDTO((long)reader.GetValue(0), reader.GetString(1), reader.GetString(2));
            return result;
        }
    }
}
