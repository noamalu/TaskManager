using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class User
    {
        private readonly string email;
        private string password;
        private Dictionary<long, Board> boardList;
        private Boolean loggedIn;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int PASSWORD_MIN_LEN = 6;
        private readonly int PASSWORD_MAX_LEN = 20;

        public Boolean LoggedIn { get => loggedIn; }
        public string Email { get => email; }
        public string Password { get => password; }
        public Dictionary<long, Board> BoardList { get => boardList; }


        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.boardList = new Dictionary<long, Board>();
            this.Persist();//save to file
            loggedIn = false;
        }

        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        public void logout()
        {
            if (loggedIn)
            {
                loggedIn = false;
            }
            else
            {
                throw new ArgumentException("user already logged out");
            }
        }

        /// <summary>
        /// This method returns all in-progress tasks of a user.
        /// </summary>
        /// <returns>A list of the in-progress tasks of the user, unless an error occurs
        public List<Task> showInProgress()
        {
            if (!this.loggedIn)
            {
                throw new Exception("user is logged out");
            }

            List<Task> inProgress = new List<Task>();
            foreach (KeyValuePair<long, Board> board in boardList)
            {
                foreach (Task task in board.Value.InProgress)
                {
                    inProgress.Add(task);
                }
            }
            return inProgress;
        }


        /// <summary>
        /// This method create and add a new task.
        /// </summary>
        /// <param name="id">Email of the user. The user must be logged in.</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <param name="creationtime">The due date if the new task</param>
        /// <param name="boardID">The name of the board</param>
        public void createTask(int id, String title, String description, DateTime dueDate,DateTime creationtime,long boardID)
        {

            if (!loggedIn)
            {
                log.Error("user is logged out cannot create a board");

                throw new ArgumentException("user is logged out cannot create a board");
            }
            if (!boardList.ContainsKey(boardID))
            {
                log.Error("board does not exist");

                throw new ArgumentException("board does not exist");

            }
            Task newtask = new Task(dueDate, title, description, boardID);
            newtask.Assignee="unAssigned";

            Board board = boardList[boardID];
            board.TaskDal.Insert(new TaskDTO(id, creationtime, dueDate, title, description, newtask.Assignee, 0, boardID));
            board.addToBacklog(newtask);
        }

        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        public void login()
        {
            loggedIn = true;
        }


        /// <summary>
        /// This method add board to user's board list.
        /// </summary>
        /// <param name="board">The name of the board</param>
        public void addBoardToList(Board board)
        {
            boardList.Add(board.BoardId, board);
        }


        /// <summary>
        /// This method remove board to user's board list.
        /// </summary>
        /// <param name="board">The name of the board</param>
        public void removeBoardFromList(Board board)
        {
            boardList.Remove(board.BoardId);
        }


        /// <summary>
        /// This method remove's the user from the board's members.
        /// </summary>
        /// <param name="board">The name of the board</param>
        public void leaveBoard(Board board)
        {
            board.removeMember(this);
        }

        private void Persist()
        {
            ToDalObject().Save();
        }
        internal UserDTO ToDalObject()
        {
            return new UserDTO(0, email, password);//currently unavailable
        }
    }

}