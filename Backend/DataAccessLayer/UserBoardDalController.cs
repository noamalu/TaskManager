using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class UserBoardDalController:DalController
    {
        //protected readonly string _membersCol= "Members";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserBoardDalController() : base("UserBoard")
        {
        }


        /// <summary>
        /// gets all user-boards of a specific board
        /// </summary>
        /// <param name="b">board we get the user-board from</param>
        /// <returns>a list of user-board from this board</returns>
        public List<UserBoardDTO> SelectAllBoardsUsers(BoardDTO b)
        {
            List<UserBoardDTO> result = Select(b).Cast<UserBoardDTO>().ToList();
            
            return result;
        }

        /// <summary>
        /// insert user-board to row in table
        /// </summary>
        /// <param name="boardMember">the user-board that will be inserted</param>
        /// <returns>true if succeeded, else false</returns>
        public bool Insert(UserBoardDTO boardMember)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({UserBoardDTO.BoardIDColumnName},{UserBoardDTO.MemberEmailColumnName}) " +
                        $"VALUES (@idVal,@membersVal);";
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardMember.ID);
                    SQLiteParameter membersParam = new SQLiteParameter(@"membersVal", boardMember.Members);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(membersParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception("Boards user insertion to kanban.db failed");
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
        /// remove user-board  row from table
        /// </summary>
        /// <param name="dto">the dto that will be removed</param>
        /// <returns>true if succeeded, else false</returns>
        public bool Delete(UserBoardDTO dto)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where MemberEmail = '{dto.Members}';"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }

        /// <summary>
        /// for every row from the table gives use user-board dto.
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>an user-board dto which suits to the given row</returns>
        protected override UserBoardDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserBoardDTO result = new UserBoardDTO((string)reader.GetValue(1), (long)reader.GetValue(0));
            return result;
        }

        /// <summary>
        /// gets all the user-boards of a specific board
        /// </summary>
        /// <param name="b">board which we want to get its members</param>
        /// <returns>list of all user-board from the board</returns>
        private List<UserBoardDTO> Select(BoardDTO b)
        {
            List<UserBoardDTO> results = new List<UserBoardDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where BoardID={b.Id};";
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
                    throw new Exception($"could not retrive board {b} columns from kanban.db");
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
            return results;
        }

        /// <summary>
        /// deletes all members of a board.
        /// </summary>
        /// <param name="dto">board which is being deleted and we want to delete its members</param>
        /// <returns>true if succeede, false if not</returns>
        public bool DeleteBoard(BoardDTO dto)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where BoardID={dto.Id}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception("Failed to delete board members of the relevant board from DB");
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
