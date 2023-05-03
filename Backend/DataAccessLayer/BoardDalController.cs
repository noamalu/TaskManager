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
    class BoardDalController : DalController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BoardDalController() : base("Boards")
        {

        }
        ///<summary>
        ///show all boards.
        ///</summary>
        ///<param name="board">the BoardDTO.</param>
        /// <returns>A list with all the boardsDTO</returns>
        public List<BoardDTO> SelectAllBoards()
        {
            return Select().Cast<BoardDTO>().ToList();
        }

        ///<summary>
        ///insert board to database.
        ///</summary>
        ///<param name="board">the BoardDTO.</param>
        /// <returns>true if added,else false</returns>
        public bool Insert(BoardDTO boardDTO)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({DTO.IDColumnName},{BoardDTO.OwnerEmailColumnName},{BoardDTO.BoardNameColumnName}) " +
                            $"VALUES (@idVal, @creatorEmailVal, @boardNameVal);";
                        

                    SQLiteParameter creatorEmailParam = new SQLiteParameter(@"creatorEmailVal", boardDTO.OwnerEmail);
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", boardDTO.Id);
                    SQLiteParameter boardNameParam = new SQLiteParameter(@"boardNameVal", boardDTO.BoardName);

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(creatorEmailParam);
                    command.Parameters.Add(boardNameParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception("Board insertion to kanban.db failed");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }

        protected override BoardDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardDTO result = new BoardDTO((long)reader.GetValue(0), reader.GetString(1), reader.GetString(2));
            return result;

        }
    }
}