using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Reflection;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;

public class TaskService
{

    private UserController uc;
    private BoardController bc;
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    internal TaskService(UserController uc, BoardController bc)
    {
        this.uc = uc;
        this.bc = bc;
    }


    /// <summary>
    /// This method updates the due date of a task
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="dueDate">The new due date of the column</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    //public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate);
    public String UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
    {

        //20. A task that is not done can be changed by its assignee only.
        try
        {
            bc.updateTaskDueDate(email, boardName, columnOrdinal, taskId, dueDate);
            log.Info("task due date changed successfuly");
            return "{}";
        }
        catch (Exception e)
        {
            return new Response<string>(e.Message, null).toJson();

        }

    }

    /// <summary>
    /// This method updates task title.
    /// </summary>
    /// <param name="email">Email of user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="title">New title for the task</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
    {       //20. A task that is not done can be changed by its assignee only.


        try
        {
            bc.updateTaskTitle(email, boardName, columnOrdinal, taskId, title);
            log.Info("task title changed successfuly");
            return "{}";


        }
        catch (Exception e)
        {
            return new Response<string>(e.Message, null).toJson();
        }
    }

    /// <summary>
    /// This method updates the description of a task.
    /// </summary>
    /// <param name="email">Email of user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="description">New description for the task</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId,
        string description)
    {


        try
        {       


            bc.updateTaskDescription(email, boardName, columnOrdinal, taskId, description);
            log.Info("task description changed successfuly");
            return "{}";

        }
        catch (Exception e)
        {
            return new Response<string>(e.Message, null).toJson();

        }
    }


    public Boolean isDone(string email, string boardName, int taskId)
    {

        long boardId = uc.getBoardByNameEmail(boardName, email).BoardId;
        Board board = uc.GetUser(email).BoardList[boardId];

        Task task = board.getTask(taskId);

        List<Task> doneList = board.GetColumn(2);

        if (doneList.Contains(task))
            return true;
        return false;

    }

    /// <summary>
    /// This method adds a new task.
    /// </summary>
    /// <param name="email">Email of the user. The user must be logged in.</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="title">Title of the new task</param>
    /// <param name="description">Description of the new task</param>
    /// <param name="dueDate">The due date if the new task</param>
    /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
    {   //default - no assignee
        try
        {
            string output = bc.AddTask(email, boardName, title, description, dueDate);
            log.Info("task was added successfully");
            return output;
        }
        catch (Exception e)
        {
            log.Debug(e.Message);
            return new Response<string>(e.Message, null).toJson();
        }
    }
}
