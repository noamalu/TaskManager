using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public abstract class DalController
    {
        protected readonly string _connectionString;
        protected readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DalController(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            Console.WriteLine(Directory.GetCurrentDirectory());
            this._connectionString = $"Data Source={path}; Version=3;";
            this._tableName = tableName;
        }



        /// <summary>
        /// update a row in the table
        /// </summary>
        /// <param name="primaryKeys">unique keys in the table for each row</param>
        /// <param name="primaryVals">the value for every primary key</param>
        /// <param name="attributeName">the column we want to change</param>
        /// <param name="attributeValue">the value for the rowXcolumn we change</param>
        /// <returns>true if updated,else false</returns>
        public bool Update(string[] primaryKeys, object[] primaryVals, string attributeName, string attributeValue)
        {
            string whereCond = Where(primaryKeys, primaryVals);
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where " + whereCond
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
                    throw new Exception($"could not update {attributeName} in kanban.db");
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
        /// a helper function which completes a where condition in sql query 
        /// </summary>
        /// <param name="primaryKeys">unique keys in the table for each row</param>
        /// <param name="primaryVals">the value for every primary key</param>
        /// <returns>an appropriate where condition</returns>
        public string Where(string[] primaryKeys, object[] primaryVals)
        {
            string whereCond = "";
            if (primaryVals[0] is string)
                whereCond += $"{primaryKeys[0]}='{primaryVals[0].ToString()}'";
            else
                whereCond += $"{primaryKeys[0]}={(long)primaryVals[0]}";
            for (int i = 1; i < primaryKeys.Length; i++)
            {
                if (primaryVals[i] is string)
                    whereCond += $" and {primaryKeys[i]}='{primaryVals[i].ToString()}'";
                else
                    whereCond += $" and {primaryKeys[i]}={(long)primaryVals[i]}";
            }
            return whereCond;
        }

        /// <summary>
        /// update a row in the table
        /// </summary>
        /// <param name="primaryKeys">unique keys in the table for each row</param>
        /// <param name="primaryVals">the value for every primary key</param>
        /// <param name="attributeName">the column we want to change</param>
        /// <param name="attributeValue">the value for the rowXcolumn we change</param>
        /// <returns>true if updated,else false</returns>
        public bool Update(string[] primaryKeys, object[] primaryVals, string attributeName, long attributeValue)
        {
            string whereCond = Where(primaryKeys, primaryVals);
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where " + whereCond
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception($"could not update {attributeName} in kanban.db");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

            }
            return res > 0;
        }

       

        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);         
                command.CommandText = $"select * from {_tableName};";
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
                finally
                {
                    if (dataReader != null){
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }
                
            }
            return results;
        }

        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        /// <summary>
        /// remove row from table
        /// </summary>
        /// <param name="dto">the dto we want to delete</param>
        public bool Delete(DTO dto)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where id={dto.Id}"
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
        /// deletes all data 
        /// </summary>
        /// <returns>true if succeeded, false if not</returns>
        public bool DeleteAll()
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw new Exception($"could not delete all from {_tableName} in kanban.db");
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