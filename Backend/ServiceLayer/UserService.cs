using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

public class UserService
{
    private static readonly UserController userController = new UserController();
    private readonly Dictionary<string, User> users = userController.UserDict;
    private readonly Dictionary<long, Board> boards = userController.BoardList;

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public UserService()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        log.Info("Starting");
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
            userController.LoadData();
            log.Info("Loaded users data successfully");
            return new Response<string>(null, "").toJson();
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, "").toJson();
        }
    }

    /// <summary>
    /// This method registers a new user to the system.
    /// </summary>
    /// <param name="email">The user email address, used as the username for logging the system.</param>
    /// <param name="password">The user password.</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Register(string email, string password)
    {
        try
        {
            userController.register(email, password);
            log.Info("registerd user successfully");
            return "{}";

        }
        catch (Exception e)
        {
            return new Response<string>(e.Message, "").toJson();
        }
    }


    /// <summary>
    ///  This method logs in an existing user.
    /// </summary>
    /// <param name="email">The email address of the user to login</param>
    /// <param name="password">The password of the user to login</param>
    /// <returns>Response with user email, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Login(string email, string password)
    {

        try
        {
            userController.login(email, password);
            log.Info("logged in user successfully");
            return new Response<string>(null, email).toJson();

        }
        catch (Exception e)
        {
            return new Response<string>(e.Message, "").toJson();
        }


    }


    /// <summary>
    ///  This method checks if an existing user is logged in.
    /// </summary>
    /// <param name="email">The email address of the user to check</param>
    /// <returns>Returns true if the user is logged in and false otherwise</returns>
    public bool IsLoggedIn(string email)
    {
        bool result = userController.GetUser(email).LoggedIn;
        if (result)
        {
            log.Info($"user {email} is logged in");
        }
        else log.Info($"user {email} is logged out");

        return result;
    }

    /// <summary>
    /// This method logs out a logged in user. 
    /// </summary>
    /// <param name="email">The email of the user to log out</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Logout(string email)
    {
        try
        {
            userController.GetUser(email).logout();
            log.Info("User logged out successfully");
            return "{}";
        }
        catch (Exception ex)
        {
            return new Response<string>(ex.Message, "").toJson();
            log.Debug(ex.Message);


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
            userController.Delete();
            log.Info("Deleted users data successfully");
            return new Response<string>(null, "").toJson();

        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, "").toJson();
        }


    }


    /// <summary>
    /// This method returns all the In progress tasks of the user.
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <returns>Response with  a list of the in progress tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string InProgressTasks(string email)
    {
        //22. As a user, I want to be able to list my 'in progress’ tasks (that I am assigned to) from all of
        // my boards, so that I can plan my schedule.
        //test- only assignee.


        string error = "";
        List<Task> listTasks = new List<Task>();
        try
        {
            listTasks = userController.GetUser(email).showInProgress();
            log.Info("got all in progress tasks successfully");
            return new Response<List<Task>>(null, listTasks).toJson();
        }
        catch (Exception ex)
        {
            return new Response<string>(ex.Message, null).toJson();
            log.Debug(ex.Message);

        }

    }

    public UserController GetUserController()
    {
        return userController;
    }


    /// <summary>
    /// This method adds a user as member to an existing board.
    /// </summary>
    /// <param name="email">The email of the user that joins the board. Must be logged in</param>
    /// <param name="boardID">The board's ID</param>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string JoinBoard(string email, int boardID)
    {
        try
        {
            userController.joinBoard(email, boardID);
            log.Info($"user {email} joind board {boardID} successfuly");
            return "{}";
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method removes a user from the members list of a board.
    /// </summary>
    /// <param name="email">The email of the user. Must be logged in</param>
    /// <param name="boardID">The board's ID</param>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string LeaveBoard(string email, int boardID)
    {
        try
        {
            userController.LeaveBoard(email, boardID);
            log.Info($"user {email} left board {boardID} successfuly");
            return "{}";
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method transfers a board ownership.
    /// </summary>
    /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
    /// <param name="newOwnerEmail">Email of the new owner</param>
    /// <param name="boardName">The name of the board</param>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
    {
        try
        {
            userController.TransferOwnership(currentOwnerEmail, newOwnerEmail, boardName);
            log.Info("board ownership changed successfully");
            return "{}";
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method assigns a task to a user
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
    /// <param name="taskID">The task to be updated identified a task ID</param>        
    /// <param name="emailAssignee">Email of the asignee user</param>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
    {
        try
        {
            userController.AssignTask(email, boardName, columnOrdinal, taskID, emailAssignee);
            log.Info("task assignment changed successfully");
            return "{}";
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method returns a list of IDs of all user's boards.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string GetUserBoards(string email)
    {
        try
        {
            Response<List<long>> response = new Response<List<long>>("", userController.GetUserBoards(email));
            log.Info($"user {email} boards were returned successfuly");
            return response.toJson();
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }

    }

    internal Board getBoard(string email, long boardId)
    {
        return users[email].BoardList[boardId];
    }

    internal Task getTask(string email, long boardId, int taskId)
    {
        return users[email].BoardList[boardId].getTask(taskId);
    }
    /*
        internal Board getBoardByIndex(string name, string email, int index)
        {
            return userController.getBoardByNameEmail(name, email);
        }
    */

}