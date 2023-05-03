using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class UserController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<long, Board> boards;
        private static readonly Dictionary<string, User> userDict = new();
        private readonly int PASSWORD_MIN_LEN = 6;
        private readonly int PASSWORD_MAX_LEN = 20;
        private readonly int LOCAL_PART_MAX_LEN = 64;
        private readonly int DOMAIN_MAX_LEN = 255;
        private readonly UserDalController userDalController;
        private static BoardController boardController;

        internal BoardController board_controller{ get => boardController; }
        internal Dictionary<string, User> UserDict { get => userDict; }
        internal Dictionary<long, Board> BoardList { get => boards; }


        public UserController()
        {
            userDalController = new UserDalController();
            boardController = new BoardController(this);
            boards = boardController.Boards;

        }


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <remarks>
        /// Checking if the email and password are valid, if true, create a new user and adding it to
        /// the userController users.
        /// </remarks>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The password of the user</param>
        /// <exception cref="System.ArgumentException">Thrown when one of the param are null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when email isn't legal .</exception>
        /// <exception cref="System.ArgumentException">Thrown when email entered has already an exsiting user.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the password isn't legal .</exception>
        public void register(string email, string password)
        {
            if (userDict.ContainsKey(email))
            {
                log.Error("already registered email");
                throw new ArgumentException("email address already registerd");
            }
            if (email == null)
            {
                log.Error("Regiser got a null email as input");
                throw new ArgumentException("The email you entered is null (empty)");
            }

            if (password == null)
            {
                log.Error("Regiser got a null password as input");
                throw new ArgumentException("The password you entered is null (empty)");
            }
            IsValidEmail(email);
            checkPassword(password);
            if (userDict.ContainsKey(email))
            {
                log.Error($"Regiser got an existing email as input: {email}");
                throw new ArgumentException("There is already an account with the email you entered");
            }
            User newUser = new User(email, password);
            userDict.Add(email, newUser);
            userDalController.Insert(new UserDTO(userDict.LongCount(), email, password));
            newUser.login();
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <remarks>
        /// Checking if the email and password are valid, if true, changing the loggedIn value 
        /// of the user to true.
        /// </remarks>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>The User with the email&password entered.</returns>
        /// <exception cref="System.ArgumentException">Thrown when one of the param are null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when email entered doesn't exists in the userDict.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the User is already logged in.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the password is incorrect (doesn't match) .</exception>
        public void login(string email, string password)
        {

            if (email == null)
            {
                log.Error("login got a null email as input");
                throw new ArgumentException("The email you entered is null (empty)");
            }

            if (password == null)
            {
                log.Error("login got a null password as input");
                throw new ArgumentException("The password you entered is null (empty)");
            }
            IsValidEmail(email);
            checkPassword(password);
            if (!userDict.ContainsKey(email))
            {
                log.Error("user does not exist");
                throw new ArgumentException("user does not exist");
            }
            if (GetUser(email).Password != password)
            {
                log.Error("entered wrong password");
                throw new ArgumentException("password is incorrect");
            }
            if (GetUser(email).LoggedIn)
            {
                log.Error("already logged in");
                throw new ArgumentException("user is already logged in");
            }
            GetUser(email).login();
        }

        /// <summary>
        /// This method checks if the password is legit.
        /// </summary>
        /// <param name="password">password the user inserted.</param>
        public void checkPassword(string password)
        {
            if (password.Length < PASSWORD_MIN_LEN || password.Length > PASSWORD_MAX_LEN)
            {

                log.Error("password entered is not within the allowed lenght range");
                throw new ArgumentException("Password length must be 6-20 characters");

            }
            if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsNumber))
            {
                log.Error("password does not contain all that is requierd");
                throw new ArgumentException("Password must contain:" +
                    "\n-At least one uppercase letter." +
                    "\n-At least one lowercase letter." +
                    "\n-At least one number.");
            }

        }


        /// <summary>
        /// Gets a specific User
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <exception cref="System.ArgumentException">Thrown when the email entered is null.</exception>
        /// <returns>The user with the required email.</returns>
        internal User GetUser(string email)
        {
            if (email == null)
            {
                log.Error("entered invalid email");
                throw new ArgumentException("The email you entered is null (empty)");
            }
            if (!userDict.ContainsKey(email))
            {
                log.Error("email is not registered");
                throw new ArgumentException("There is not an account with the email you entered");
            }
            return userDict[email];
        }



        /// <summary>
        /// Checks if the email address entered is legal, by a regex pattern.
        /// </summary>
        /// <param name="email">The email address that needs to be checked</param>
        /// <returns>The limit of the column.</returns>
        /// <exception cref="System.ArgumentException">Thrown when email isn't legal.</exception>
        private void IsValidEmail(string email)
        {
            const string emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                log.Error($"An illegal email adress attempted Registeration: {email}");
                throw new ArgumentException("The email address you entered is not legal");
            }
            string localPart = email.Split('@')[0];
            if (localPart.Length > LOCAL_PART_MAX_LEN)
            {
                log.Error($"An illegal email adress attempted Registeration: {email}");
                throw new ArgumentException("The email address you entered is not legal");
            }

            string domain = email.Split('@')[1];
            if (domain.Length > DOMAIN_MAX_LEN)
            {
                log.Error($"An illegal email adress attempted Registeration: {email}");
                throw new ArgumentException("The email address you entered is not legal");
            }

        }

        internal Board getBoardByNameEmail(string name, string email)
        {
            Board output = null;

            foreach (KeyValuePair<long, Board> myKeyValue in boards)
            {
                Boolean ismember = false;

                if (myKeyValue.Value.Members.Contains(userDict[email]))
                {
                    ismember = true;
                }
                else
                {
                    throw new ArgumentException("this user cant access such board");
                }
                if (myKeyValue.Value.BoardName.Equals(name) && ismember)
                {
                    output = myKeyValue.Value;
                    break;
                }
            }
            if (output == null || !boards.ContainsKey(output.BoardId))
            {
                throw new Exception("no such board");
            }
            
            return output;
        }

        internal int getIdCount()
        {
            return boardController.boardIdCounter;
        }

        internal void increaseIdCount()
        {
            boardController.increaseBoardIdCounter();
        }

        /// <summary>
        /// deletes all users data from the DB
        /// </summary>
        public void Delete()
        {
            boardController.Delete();
            IList<UserDTO> users = userDalController.SelectAllUsers();
            foreach (UserDTO u in users)
            {
                userDalController.Delete(u);
            }           
            userDict.Clear();

        }

        /// <summary>
        /// loads all users data from the DB
        /// </summary>
        public void LoadData()
        {
            IList<UserDTO> users = userDalController.SelectAllUsers();
            foreach (UserDTO u in users)
            {
                userDict.Add(u.Email, new User(u.Email, u.Password));
            }
            boardController.LoadData();
        }

        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        public void joinBoard(string email, int boardID)
        {

            if (!userDict.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!userDict[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            if (!boards.ContainsKey(boardID))
            {
                throw new Exception("board was not found");
            }
            if (boards[boardID].Members.Contains(userDict[email]))
            {
                throw new Exception("user is already board member");
            }
            ICollection<Board> usersBoards = userDict[email].BoardList.Values;
            foreach(Board board in usersBoards)
            {
                if (board.BoardName == boards[boardID].BoardName)
                {
                    throw new Exception($"user already has a board named {boards[boardID].BoardName}");
                }
            }

            userDict[email].addBoardToList(boards[boardID]);
            boards[boardID].addMember(userDict[email]);
            
        }

        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="boardID">The board's ID</param>
        public void LeaveBoard(string email, int boardID)
        {

            if (!userDict.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!userDict[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            if (!boards.ContainsKey(boardID))
            {
                throw new Exception("board was not found");
            }
            if (!userDict[email].BoardList.ContainsKey(boardID))
            {
                throw new Exception($"user is not a member of the board");
            }
            if (userDict[email].Equals(boards[boardID].Owner))
            {
                throw new Exception("the board owner cannot leave the board");
            }
            List<Task> alltasks = new List<Task>();
            foreach (Task task in boards[boardID].GetColumn(0))
            {
                if (task.Assignee.Equals(email))
                {
                    task.Assignee = "unAssigned";
                }
            }
            foreach (Task task in boards[boardID].GetColumn(1))
            {
                if (task.Assignee.Equals(email))
                {
                    task.Assignee = "unAssigned";
                }
            }
            foreach (Task task in boards[boardID].GetColumn(2))
            {
                if (task.Assignee.Equals(email))
                {
                    task.Assignee = "unAssigned";
                }
            }

            userDict[email].removeBoardFromList(boards[boardID]);
            boards[boardID].removeMember(userDict[email]);
            List<Task> toUnassign = boards[boardID].Backlog;
            foreach (Task task in toUnassign)
            {
                task.Assignee = "unAssigned";
            }
            toUnassign = boards[boardID].InProgress;
            foreach (Task task in toUnassign)
            {
                task.Assignee = "unAssigned";
            }
            toUnassign = boards[boardID].Done;
            foreach (Task task in toUnassign)
            {
                task.Assignee = "unAssigned";
            }
            boardController.leaveBoard(email,boardID);

        }

        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            if (!userDict.ContainsKey(currentOwnerEmail))
            {
                throw new Exception("owner was not found");
            }
            User currentOwner = userDict[currentOwnerEmail];

            if (!userDict.ContainsKey(newOwnerEmail))
            {
                throw new Exception("owner to be was not found");
            }
            User newOwner = userDict[newOwnerEmail];

            if (!currentOwner.LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = this.getBoardByNameEmail(boardName, currentOwnerEmail).BoardId;
            if (!currentOwner.BoardList.ContainsKey(boardId))
            {
                throw new Exception("board was not found");
            }
            Board board = currentOwner.BoardList[boardId];

            if (!board.Owner.Equals(currentOwner))
            {
                throw new Exception("board ownership can be changed only by the board owner");
            }
            if (!board.Members.Contains(newOwner))
            {
                throw new Exception("board ownership can be changed only to one of the board members");
            }
            board.Owner = newOwner;

            string[] keys = { "Id"};
            object[] vals = { boardId};
            boardController.BoardDal.Update(keys, vals, "OwnerEmail", newOwnerEmail);
        }


        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        public void AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {

            if (!userDict.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!userDict.ContainsKey(emailAssignee))
            {
                throw new Exception("assignee user was not found");
            }
            User user = userDict[email];

            if (!userDict.ContainsKey(emailAssignee))
            {
                throw new Exception("user is not board member");
            }
            string assignee = emailAssignee;

            if (!user.LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            long boardId = this.getBoardByNameEmail(boardName, email).BoardId;
            if (!user.BoardList.ContainsKey(boardId))
            {
                throw new Exception("board was not found");
            }

            Board board = user.BoardList[boardId];

            Task task = board.getTask(taskID);
            if(task.Assignee != "unAssigned" && email != task.Assignee)
            {
                throw new Exception("task is already assigned to a different member");
            }
            if (!board.Members.Contains(user))

            {
                throw new Exception("task assignment can be changed only by one of the board members");
            }
            if (!board.Members.Contains(userDict[emailAssignee]))
            {
                throw new Exception("task assignment can only be changed to one of the board members");
            }
            if(board.getTask(taskID).CoulmnOrdinal!=columnOrdinal)
                throw new Exception("wrong column");
            

            board.getTask(taskID).Assignee = assignee;

            string[] keys = { "BoardId", "Id" };
            object[] vals = { boardId, (long)taskID };

            boardController.TaskDal.UpdateTask(keys, vals, "Assignee", assignee);
        }

        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="email"></param>
        public List<long> GetUserBoards(string email)
        {

            if (!userDict.ContainsKey(email))
            {
                throw new Exception("user was not found");
            }
            if (!userDict[email].LoggedIn)
            {
                throw new Exception("user is logged out");
            }
            List<long> boardIds = userDict[email].BoardList.Keys.ToList();
            return boardIds;
        }
    }
}
