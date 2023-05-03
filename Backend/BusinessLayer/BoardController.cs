using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using log4net;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using Task = IntroSE.Kanban.Backend.BusinessLayer.Task;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class BoardController
    {
        private UserController userController;
        private Dictionary<string, User> users;
        private static readonly Dictionary<long, Board> boards = new();
        public int boardIdCounter;
        private readonly BoardDalController boardDalController = new BoardDalController();
        private readonly UserBoardDalController userBoardDalController = new UserBoardDalController();
        private readonly TaskDalController taskDalController = new TaskDalController();
        private readonly ColumnDalController columnDalController = new ColumnDalController();
        private const int COlUMN_ORDINAL_BACKLOG = 0;
        private const int COlUMN_ORDINAL_INPROCESS = 1;
        private const int COlUMN_ORDINAL_DONE = 2;




        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal TaskDalController TaskDal => taskDalController;

        internal BoardDalController BoardDal => boardDalController;

        public BoardController(UserController uc)
        {
            boardIdCounter = 1;
            userController = uc;
            users = userController.UserDict;
        }

        public Dictionary<long, Board> Boards { get => boards; }

        public int IdCounter { get => boardIdCounter; }



        internal void increaseBoardIdCounter()
        {
            boardIdCounter++;
        }

        /// <summary>
        /// deletes all boards data from the DB
        /// </summary>
        public void Delete()
        {
            IList<BoardDTO> boardsDTO = boardDalController.SelectAllBoards();
            foreach (BoardDTO b in boardsDTO)
            {
                boardDalController.Delete(b);
                userBoardDalController.DeleteBoard(b);
            }

            TaskDal.DeleteAll();
            columnDalController.DeleteAll();

            boards.Clear();
        }

        /// <summary>
        /// loads all boards data from the DB
        /// </summary>
        public void LoadData()
        {
            IList<BoardDTO> boardlist = boardDalController.SelectAllBoards();

            foreach (BoardDTO b in boardlist)
            {
                Board newB = new Board(b.Id, b.BoardName, users[b.OwnerEmail], true);
                IList<UserBoardDTO> members = userBoardDalController.SelectAllBoardsUsers(b);
                foreach (UserBoardDTO user in members)
                {
                    User member = userController.UserDict[user.Members];
                    newB.addMember(member);
                    member.addBoardToList(newB);
                }
                newB.ColumnDal = columnDalController;
                newB.TaskDal = taskDalController;
                newB.LoadData(b);
                boards.Add((int)b.Id, newB);
            }
        }

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        public void limitCoulmn(string email, string boardName, int columnOrdinal, int limit)
        {
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            if (columnOrdinal == COlUMN_ORDINAL_BACKLOG)
            {
                users[email].BoardList[boardId].setMaxBacklog(limit);
                log.Info("backlogs limit set successfully");

            }
            else if (columnOrdinal == COlUMN_ORDINAL_INPROCESS)
            {
                users[email].BoardList[boardId].setMaxInProgress(limit);
                log.Info("inProcess limit set successfully");

            }
            else if (columnOrdinal == COlUMN_ORDINAL_DONE)
            {
                users[email].BoardList[boardId].setMaxDone(limit);
                log.Info("dones limit set successfully");

            }
            else
            {
                throw new Exception("no such column");
            }
        }

        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A string with the column's limit,unless an error occurs.</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            int output = -1;
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            if (columnOrdinal == COlUMN_ORDINAL_BACKLOG)
            {
                output = users[email].BoardList[boardId].MaxBacklog;
                log.Info("backlogs limit retrived successfully");

            }
            else if (columnOrdinal == COlUMN_ORDINAL_INPROCESS)
            {
                output = users[email].BoardList[boardId].MaxInProgress;
                log.Info("inprocess limit retrived successfully");

            }
            else if (columnOrdinal == COlUMN_ORDINAL_DONE)
            {
                output = users[email].BoardList[boardId].MaxDone;
                log.Info("dones limit retrived successfully");

            }
            else
            {
                throw new Exception("no such column");
            }
            return new Response<int>("", output).toJson();
        }

        /// <summary>
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A string with the column's name unless an error occurs.
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            string output = "";
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            if (!boards.ContainsKey(userController.getBoardByNameEmail(boardName, email).BoardId))
            {
                throw new Exception("no such board");
            }
            if (columnOrdinal == COlUMN_ORDINAL_BACKLOG)
            {
                output += "backlog";
                log.Info("backlogs name retrived successfully");

            }
            else if (columnOrdinal == COlUMN_ORDINAL_INPROCESS)
            {
                output += "in progress";
                log.Info("inprocess name retrived successfully");
            }
            else if (columnOrdinal == COlUMN_ORDINAL_DONE)
            {
                output += "done";
            }
            else
            {
                throw new Exception("no such column");
                log.Info("dones name retrived successfully");
            }
            return new Response<string>(null, output).toJson();
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>An Empty string unless an error occurs.
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {

            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }

            if (dueDate.CompareTo(DateTime.Now)<=0)
            {
                throw new Exception("due date must be after creation time");
            }

            Board destinationBoard = userController.getBoardByNameEmail(boardName, email);

            users[email].createTask(destinationBoard.TaskIdCounter, title, description, dueDate,DateTime.Now, destinationBoard.BoardId);
            string output = "";
            return new Response<string>(null, output).toJson();
        }


        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>An "{}", unless an error occurs.
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            if (columnOrdinal < 0 | columnOrdinal > 2)
            {
                throw new Exception("no such column");
            }
            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            Task taskToPass = users[email].BoardList[boardId].getTask(taskId);
            if (!taskToPass.Assignee.Equals("") && 
                !taskToPass.Assignee.Equals(users[email].Email))
            {
                throw new Exception("user is not assigned to this task");
            }
            if (columnOrdinal == 0)
            {
                users[email].BoardList[boardId].passToInProgress(taskToPass);
            }
            else if (columnOrdinal == 1)
            {
                users[email].BoardList[boardId].passToDone(taskToPass);
            }
            else if (columnOrdinal == 2)
            {
                throw new Exception("no such movement");
            }
            else
            {
                throw new Exception("no such column");
            }

            taskToPass.CoulmnOrdinal = taskToPass.CoulmnOrdinal + 1;
            taskToPass.taskDTO.ColumnOrdinal = taskToPass.CoulmnOrdinal;
            string[] keys = { "BoardId", "Id" };
            object[] vals = { boardId, (long)taskId };
            TaskDal.Update(keys, vals, "ColumnOrdinal", taskToPass.CoulmnOrdinal);

            return "{}";
        }

        /// <summary>
        /// This method returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>list of the column's tasks, unless an error occurs.
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            List<Task> output = new List<Task>();
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            output = users[email].BoardList[boardId].GetColumn(columnOrdinal);
            return new Response<List<Task>>(null, output).toJson();
        }

        /// <summary>
        /// This method adds a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <param name="id">boards id</param>
        /// <param name="owner">The board's owner</param>
        /// <returns>An "{}" string, unless an error occurs
        public string AddBoard(string email, string name, int id, User owner)
        {
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            if (name == null || name == "" || name == " ")
            {
                throw new Exception("board name can not be null nor empty");
            }
            foreach (KeyValuePair<long, Board> board in boards)
            {
                if (board.Value.BoardName == name && owner.Equals(board.Value.Owner))
                {
                    throw new Exception("board named " + name + " already exists");
                }
            }
            Board newBoard = new Board(id, name, owner);
            newBoard.TaskDal = TaskDal;
            newBoard.loadCol(columnDalController);
            newBoard.ColumnDal = columnDalController;
            newBoard.BoardId = id;
            boards.Add(id, newBoard);
            users[email].addBoardToList(newBoard);
            newBoard.addMember(users[email]);
            boardDalController.Insert(new BoardDTO(newBoard.BoardId, owner.Email, name));
            userController.increaseIdCount();
            userBoardDalController.Insert(new UserBoardDTO(email, id));
            log.Info("board was added successfully");
            return "{}";
        }

        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in and an owner of the board.</param>
        /// <param name="name">The name of the board</param>
        /// <returns>An "{}" string, unless an error occurs
        public string RemoveBoard(string email, string name)
        {
            if (!users.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!users[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = userController.getBoardByNameEmail(name, email).BoardId;
            if (!users[email].BoardList.ContainsKey(boardId))
            {
                throw new Exception("board was not found");
            }
            if (!users[email].BoardList[boardId].Owner.Equals(users[email]))
            {
                throw new Exception("access denied. only the board owner can delete board");
            }
            Board boardToRemove = userController.getBoardByNameEmail(name, email);
            if (!boards.ContainsValue(boardToRemove))
            {
                log.Error("board does not exist");
                throw new ArgumentException("board does not exist");
            }
            boards.Remove(boardToRemove.BoardId);
            users[email].removeBoardFromList(boardToRemove);
            IList<BoardDTO> boardDTOs = boardDalController.SelectAllBoards();
            foreach(BoardDTO boardDTO in boardDTOs)
            {
                if (boardDTO.Id == boardId)
                {
                    boardDalController.Delete(boardDTO);
                    boardToRemove.TaskDal.Delete(boardDTO);
                    columnDalController.Delete(boardDTO);
                    userBoardDalController.DeleteBoard(boardDTO);
                }
            }
            boardToRemove.Backlog.Clear();
            boardToRemove.InProgress.Clear();
            boardToRemove.Done.Clear();

            return "{}";
        }

        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        public void updateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
           
            User user = userController.GetUser(email);
            if (!user.LoggedIn)
            {
                throw new Exception("user isnt logged in ");
            }

            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            Board board = userController.GetUser(email).BoardList[boardId];

            if (board.getTask(taskId).CoulmnOrdinal != columnOrdinal)
            {
                log.Warn("wrong input for coulmn");
                throw new Exception("task coulmn's is wrong");
            }

            if (isDone(email, boardName, taskId))
            {
                log.Debug("A task that is done can't be changed");
                throw new Exception("A task that is done can't be changed");
            }

            
            Task task = board.getTask(taskId);
            task.updateTaskDueDate(dueDate, email);
            string[] keys = { "BoardId", "Id" };
            object[] vals = { boardId, (long)taskId };
            TaskDal.UpdateTask(keys, vals, "DueDate", dueDate.ToString());
        }

        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        public void updateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {

           
            User user = userController.GetUser(email);
            if (!user.LoggedIn)
            {
                throw new Exception("user isnt logged in ");
            }
            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            Board board = userController.GetUser(email).BoardList[boardId];

            if (board.getTask(taskId).CoulmnOrdinal != columnOrdinal)
            {
                log.Warn("wrong input for coulmn");
                throw new Exception("task coulmn's is wrong");
            }

            if (isDone(email, boardName, taskId) == true)
                throw new Exception("A task that is not done can be changed");
            
            
            Task task = board.getTask(taskId);
            task.changeTitle(title, email);
            string[] keys = { "BoardId", "Id" };
            object[] vals = { boardId, (long)taskId };
            TaskDal.UpdateTask(keys, vals, "Title", title);
        }


        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        public void updateTaskDescription(string email, string boardName, int columnOrdinal, int taskId,
            string description)
        {

            User user = userController.GetUser(email);
            if (!user.LoggedIn)
            {
                throw new Exception("user isn't logged in ");
            }

            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            Board board = userController.GetUser(email).BoardList[boardId];

            if(board.getTask(taskId).CoulmnOrdinal!=columnOrdinal)
            {
                log.Warn("wrong input for coulmn");
                throw new Exception("task coulmn's is wrong");
            }

            if (isDone(email, boardName, taskId) == true)
            {
                log.Warn("Task that is not done can not be changed");
                throw new Exception("A task that is not done can be changed");
            }

            Task task = board.getTask(taskId);
            task.changeDescription(description, email);
            string[] keys = { "BoardId", "Id" };
            object[] vals = { boardId, (long)taskId };
            TaskDal.UpdateTask(keys, vals, "Description", description);
        }






        /// <summary>
        /// This method checks if task is done.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns> true if the task is done,else return false
        public Boolean isDone(string email, string boardName, int taskId)
        {

            long boardId = userController.getBoardByNameEmail(boardName, email).BoardId;
            Board board = userController.GetUser(email).BoardList[boardId];

            Task task = board.getTask(taskId);

            List<Task> doneList = board.GetColumn(2);

            if (doneList.Contains(task))
                return true;
            return false;

        }

        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="id">The board's ID</param>
        public void joinBoard(string email, long id)
        {
            userBoardDalController.Insert(new UserBoardDTO(email, id));

        }


        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="id">The board's ID</param>
        public void leaveBoard(string email, long id)
        {
            userBoardDalController.Delete(new UserBoardDTO(email, id));

        }

    }
}