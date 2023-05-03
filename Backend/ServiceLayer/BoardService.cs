using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System.Reflection;

public class BoardService
{
    private UserController userController;
    private static BoardController boardController;
    private Dictionary<string, User> users;
    private readonly Dictionary<long, Board> boards;
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    internal BoardService(UserController uc)
    {
        userController = uc;
        users = userController.UserDict;
        boardController = userController.board_controller;
        boards = boardController.Boards;
    }
    /// <summary>
    /// This method limits the number of tasks in a specific column.
    /// </summary>
    /// <param name="email">The email address of the user, must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
    {
        try
        {
            boardController.limitCoulmn(email, boardName, columnOrdinal, limit);
            log.Info("column was limited successfully");
            return "{}";
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method gets the limit of a specific column.
    /// </summary>
    /// <param name="email">The email address of the user, must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <returns>Response with column limit value, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string GetColumnLimit(string email, string boardName, int columnOrdinal)
    {
        try
        {
            string output = boardController.GetColumnLimit(email, boardName, columnOrdinal);
            log.Debug("column limit founded successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method gets the name of a specific column
    /// </summary>
    /// <param name="email">The email address of the user, must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <returns>Response with column name value, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string GetColumnName(string email, string boardName, int columnOrdinal)
    {
        try
        {
            string output = boardController.GetColumnName(email, boardName, columnOrdinal);
            log.Info("found column name successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }




    /// <summary>
    /// This method advances a task to the next column
    /// </summary>
    /// <param name="email">Email of user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    /// updates: we added check if the user who advance task is the assignee of the task.
    public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
    {
        try
        {
            string output = boardController.AdvanceTask(email, boardName, columnOrdinal, taskId);
            log.Info("task was advanced successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method returns a column given it's name
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <returns>Response with  a list of the column's tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string GetColumn(string email, string boardName, int columnOrdinal)
    {
        try
        {
            string output = boardController.GetColumn(email, boardName, columnOrdinal);
            log.Info("column founded successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method adds a board to the specific user.
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="name">The name of the new board</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string AddBoard(string email, string name)
    {
        try
        {
            string output = boardController.AddBoard(email, name, userController.getIdCount(), users[email]);
            log.Info("board was added successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method removes a board to the specific user.
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="name">The name of the board</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string RemoveBoard(string email, string name)
    {
        try
        {
            string output = boardController.RemoveBoard(email, name);
            log.Info("board was removed successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();

        }
    }

    ///<summary>This method loads all persisted data.
    ///<para>
    ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method. 
    ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
    ///</para>
    /// </summary>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string LoadData()
    {
        try
        {
            boardController.LoadData();
            log.Info("Loaded boards data successfully");
            return new Response<string>(null, "").toJson();
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, "").toJson();
        }
    }

    ///<summary>This method deletes all persisted data.
    ///<para>
    ///<b>IMPORTANT:</b> 
    ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.
    ///</para>
    /// </summary>
    ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string DeleteData()
    {
        try
        {
            boardController.Delete();
            log.Info("Deleted users data successfully");
            return new Response<string>(null, "").toJson();

        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, "").toJson();
        }
    }

    internal Dictionary<string, User> getuserslist()
    {
        return users;
    }
    internal BoardController GetBoardController()
    {
        return boardController;
    }
    
}