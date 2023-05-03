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
   
    public class ColumnDalController:DalController
    {
        

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ColumnDalController():base("Columns")
        {

        }

        /// <summary>
        /// gets all columns from board
        /// </summary>
        /// <param name="b">Board which we wish to get his columns</param>
        /// <returns>list of all columns from boards</returns>
        public List<ColumnDTO> SelectAllColumns(BoardDTO b)
        {
            List<ColumnDTO> result = Select(b).Cast<ColumnDTO>().ToList();
            return result;
        }


        /// <summary>
        /// inserts a column to the table
        /// </summary>
        /// <param name="column">column to be persisted</param>
        /// <returns>true if not failed,else false</returns>
        public bool Insert(ColumnDTO column)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {_tableName} ({ColumnDTO.IDColumnName},[{ColumnDTO.LimitColumnName}],{ColumnDTO.NameColumnName}) " +
                        $"VALUES (@idVal,@limitVal,@nameVal);";

                     SQLiteParameter idParam = new SQLiteParameter(@"idVal", column.ID);
                    SQLiteParameter limitParam = new SQLiteParameter(@"limitVal", column.Limit);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", column.Name);             

                    command.Parameters.Add(idParam);
                    command.Parameters.Add(limitParam);
                    command.Parameters.Add(nameParam);       
                    command.Prepare();

                    res = command.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    log.Error(e.Message);

                    throw new Exception("Column insertion to kanban.db failed");
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
        /// for every row from the table gives use column dto.
        /// </summary>
        /// <param name="reader">record to be converted</param>
        /// <returns>an column dto which suits to the given row</returns>
        protected override ColumnDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO((long)reader.GetValue(0), (long)reader.GetValue(1), reader.GetString(2));
            return result;
        }


        /// <summary>
        /// gets all the columns from board
        /// </summary>
        /// <param name="b">board which we wish to get its columns</param>
        /// <returns>list of all columns of the given board</returns>
        private List<ColumnDTO> Select(BoardDTO b)
        {
            List<ColumnDTO> results = new List<ColumnDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName} where Id={b.Id};";
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
                catch(Exception e)
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
        /// deletes all columns of a given board
        /// </summary>
        /// <param name="DTOObj">board which is being deleted and we wish to remove its columns</param>
        /// <returns>true if succeeded, false if not</returns>
        public bool DeleteBoard(BoardDTO DTOObj)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where {ColumnDTO.IDColumnName}={DTOObj.Id}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new Exception($"could not delete board {DTOObj} columns from kanban.db");
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
