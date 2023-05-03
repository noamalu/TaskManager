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
    /// TaskDalController class
    /// responsible for accessing Tasks table in the DB
    /// which stores all the tasks in our system
    /// </summary>
    class TaskDalController : DalController
    {
        /// <summary>
        /// TaskDalController constructor
        /// relates the controller to Tasks table in DB
        /// </summary>
        /// 
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TaskDalController() : base("Task")
        {

        }

        

        /// <summary>
        /// insert task to row in table
        /// </summary>
        /// <param name="task">the task that will be inserted</param>
        /// <returns>true if succeeded, else false</returns>
        public bool Insert(TaskDTO task)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({TaskDTO.TaskId} ,{TaskDTO.CreationTimeColumnName},{TaskDTO.DueDateColumnName},{TaskDTO.TitleColumnName},{TaskDTO.DescriptionColumnName},{TaskDTO.AssigneeColumnName},{TaskDTO.ColumnOrdinalColumnName},{TaskDTO.BoardIDColumnName}) " +
                        $"VALUES (@idVal,@creationVal,@dueVal,@titleVal,@descriptionVal,@assigneeVal,@colVal,@boardId);";


                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", task.Id);
                    SQLiteParameter creationParam = new SQLiteParameter(@"creationVal", task.CreationTime);
                    SQLiteParameter dueParam = new SQLiteParameter(@"dueVal", task.DueDate);
                    SQLiteParameter titleParam = new SQLiteParameter(@"titleVal", task.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"descriptionVal", task.Description);
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"assigneeVal", task.Assignee);
                    SQLiteParameter columnParam = new SQLiteParameter(@"colVal", task.ColumnOrdinal);
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"boardId", task.BoardId);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(creationParam);
                    command.Parameters.Add(dueParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(assigneeParam);
                    command.Parameters.Add(columnParam);
                    command.Parameters.Add(boardIdParam);
                    
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception("Task insertion to kanban.db failed");
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
        /// for every row from the table gives use task dto.
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>an task dto which suits to the given row</returns>
        protected override TaskDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO((int)reader.GetInt32(0), DateTime.Parse(reader.GetString(1)), DateTime.Parse(reader.GetString(2)),
                reader.GetString(3), reader.GetString(4), reader.GetString(5), (int)reader.GetInt32(6), (long)reader.GetValue(7));
            return result;
        }


        /// <summary>
        /// gets all tasks from column
        /// </summary>
        /// <param name="c">the column</param>
        /// <returns>list of all the tasks in the column</returns>
        internal List<TaskDTO> Select(ColumnDTO c)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int BACKLOG_COL_ORD = 0;
                int IN_PROGRESS_COL_ORD = 1;
                int DONE_COL_ORD = 2;
                int col = BACKLOG_COL_ORD;
                if (c.Name == "in progress")
                {
                    col = IN_PROGRESS_COL_ORD;
                }
                if (c.Name == "done")
                {
                    col = DONE_COL_ORD;
                }
                command.CommandText = $"select * from {_tableName} where BoardID={c.ID} and ColumnOrdinal={col};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception($"Failed to retrive all tasks in column {col} from kanban.db");
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }
            return results.Cast<TaskDTO>().ToList();
        }



        /// <summary>
        /// update a row in the table
        /// </summary>
        /// <param name="primaryKeys">unique keys in the table for each row</param>
        /// <param name="primaryVals">the value for every primary key</param>
        /// <param name="attributeName">the column we want to change</param>
        /// <param name="attributeValue">the value for the rowXcolumn we change</param>
        /// <returns>true if updated,else false</returns>
        public bool UpdateTask(string[] primaryKeys, object[] primaryVals, string attributeName, string attributeValue)
        {
            string whereCond = Where(primaryKeys, primaryVals);
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]='{attributeValue}' where " + whereCond
                };
                try
                {

                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception("update of Task in kanban.db failed");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

    }
}
