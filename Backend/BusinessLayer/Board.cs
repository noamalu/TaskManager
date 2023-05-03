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
    public class Board
    {
        private String boardName;
        private User owner;
        private List<User> members; 

        private long boardId;
        private int taskIdCounter;

        private List<Task> backlog;
        private List<Task> inProgress;
        private List<Task> done;

        private int maxBacklog;
        private int maxInProgress;
        private int maxDone;

        private  TaskDalController taskDalController;
        private ColumnDalController columnDalController;


        //getters & setters
        public int TaskIdCounter { get => taskIdCounter; set => taskIdCounter = value; }
        public long BoardId { get => boardId; set => boardId = value; }
        public string BoardName { get => BoardName1; set => BoardName1 = value; }
        internal User Owner { get => owner; set => owner = value; }
        public string OwnerEmail { get => owner.Email; }
        public int MaxBacklog { get => maxBacklog;}
        public int MaxInProgress { get => maxInProgress;}
        public int MaxDone { get => maxDone;}
        internal List<User> Members { get => members; set => members = value; }

        internal List<Task> Backlog { get => backlog; set => backlog = value; }
        internal List<Task> InProgress { get => inProgress; set => inProgress = value; }
        internal List<Task> Done { get => done; set => done = value; }
        internal TaskDalController TaskDal { get => taskDalController; set => taskDalController = value; }
        internal ColumnDalController ColumnDal { get => columnDalController; set => columnDalController = value; }
        public string BoardName1 { get => boardName; set => boardName = value; }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ///Board constructor
        ///
        ///input: string name, string userMail
        ///output: none
        internal Board(long id, string name, User owner, bool load=false)
        {
            this.BoardName1 = name;
            boardId = id;
            this.owner = owner;
            members = new List<User>();
            backlog = new List<Task>();
            inProgress = new List<Task>();
            done = new List<Task>();
            maxBacklog = -1;
            maxInProgress = -1;
            maxDone = -1;
            taskIdCounter = 0;




        }


        /// <summary>
        /// This method load the columns
        /// </summary>
        /// <param name="columnDalController">the columnDalController uses to insert the columns.</param>
        public void loadCol(ColumnDalController columnDalController)
        {
            columnDalController.Insert(new ColumnDTO(boardId, -1, "backlog"));
            columnDalController.Insert(new ColumnDTO(boardId, -1, "in progress"));
            columnDalController.Insert(new ColumnDTO(boardId, -1, "done"));
        }



        ///adding task to backlog
        ///
        ///input: Task
        ///output: none
        internal void addToBacklog(Task task)
        {
            if (maxBacklog != -1 & backlog.Count == maxBacklog)
            {
                log.Error("cant add to full coulmn");

                throw new Exception("backlog column is full");
            }
            backlog.Add(task);
            task.Id = taskIdCounter;
            taskIdCounter++;
        }

        ///moving task from backlog to inProgress
        ///
        ///input: Task
        ///output: none
        internal void passToInProgress(Task task)
        {
            if (maxInProgress != -1 & inProgress.Count == maxInProgress)
            {
                log.Error("cant add to full coulmn");

                throw new Exception("in progress column is full");
            }
            backlog.Remove(task);
            inProgress.Add(task);
        }

        internal void LoadData(BoardDTO b)
        {
            IList<ColumnDTO> columns = columnDalController.SelectAllColumns(b);

            foreach (ColumnDTO c in columns)
            {
                if (c.ID == boardId)
                {
                    if (c.Name == "backlog")
                    {
                        maxBacklog = (int)(c.Limit);
                        TaskDalController taskDalController = new TaskDalController();
                        List<TaskDTO> allTasks = taskDalController.Select(c);
                        foreach(TaskDTO task in allTasks)
                        {
                            Task taski = new Task(task.DueDate, task.Title, task.Description, task.BoardId);
                            taski.Id = (int)task.Id;

                            backlog.Add(taski);
                        }
                    }
                    else if (c.Name == "in progress")
                    {
                        maxInProgress = (int)(c.Limit);
                        TaskDalController taskDalController = new TaskDalController();
                        List<TaskDTO> allTasks = taskDalController.Select(c);
                        foreach (TaskDTO task in allTasks)
                        {
                            Task taski = new Task(task.DueDate, task.Title, task.Description, task.BoardId);
                            taski.Id = (int)task.Id;

                            inProgress.Add(taski);
                        }
                    }
                    else
                    {
                        maxDone = (int)(c.Limit);
                        TaskDalController taskDalController = new TaskDalController();
                        List<TaskDTO> allTasks = taskDalController.Select(c);
                        foreach (TaskDTO task in allTasks)
                        {
                            Task taski = new Task(task.DueDate, task.Title, task.Description, task.BoardId);
                            taski.Id = (int)task.Id;

                            done.Add(taski);
                        }
                    }
                }
            }
        }

       
       

        ///moving task from inProgress to done
        ///
        ///input: Task
        ///output: none
        internal void passToDone(Task task)
        {
            if (maxDone != -1 & done.Count == maxDone)
            {
                log.Error("cant add to full coulmn");

                throw new Exception("done column is full");
            }
            inProgress.Remove(task);
            done.Add(task);
        }


        /// <summary>
        /// This method set max backlog limit
        /// </summary>
        /// <param name="max">the limit we want for backlog.</param>
        public void setMaxBacklog(int max)
        {
            if (max < -1)
            {
                log.Error("entered bad input(negative)");
                throw new Exception("max tasks cant be negative");
            }
            if (max < backlog.Count)
            {
                log.Error("entered number smaller than list length");
                throw new Exception("a larger amount of tasks is already exist");
            }
            maxBacklog = max;
        }

        /// <summary>
        /// This method set max in progress limit
        /// </summary>
        /// <param name="max">the limit we want for in progress.</param>
        public void setMaxInProgress(int max)
        {
            if (max < -1)
            {
                log.Error("entered bad input(negative)");
                throw new Exception("max tasks cant be negative");
            }
            if (max < inProgress.Count)
            {
                log.Error("entered number smaller than list length");
                throw new Exception("a larger amount of tasks is already exist");
            }
            maxInProgress = max;
        }

        /// <summary>
        /// This method set max done limit
        /// </summary>
        /// <param name="max">the limit we want for done.</param>
        public void setMaxDone(int max)
        {
            if (max < -1)
            {
                log.Error("entered bad input(negative)");
                throw new Exception("max tasks cant be negative");
            }
            if (max < done.Count)
            {
                log.Error("entered number smaller than list length");
                throw new Exception("a larger amount of tasks is already exist");
            }
            maxDone = max;
        }


        ///get task
        ///
        ///input: int taskID
        ///output: Task
        internal Task getTask(int taskId)
        {
            foreach (Task task in backlog)
            {
                if (task.Id == taskId)
                {
                    return task;
                }
            }
            foreach (Task task in inProgress)
            {
                if (task.Id == taskId)
                {
                    return task;
                }
            }
            foreach (Task task in done)
            {
                if (task.Id == taskId)
                {
                    return task;
                }
            }
            log.Error("task id couldnt be found");
            throw new Exception("task id is not found.");
        }

        ///get column
        ///
        ///input: int columnOrdinal
        ///output: List<Task>
        internal List<Task> GetColumn(int columnOrdinal)
        {
            if (columnOrdinal > 2 | columnOrdinal < 0)
            {
                log.Error("bad input for coulmnordinal, should be 0-2");
                throw new Exception("coulmn must be between 0-2");
            }
            else
            {
                if (columnOrdinal == 0)
                    return backlog;
                if (columnOrdinal == 1)
                    return inProgress;
                else if (columnOrdinal == 2)
                    return done;
            }

            return null;

        }


        internal void addMember(User newMember)
        {
            members.Add(newMember);
        }

        internal void removeMember(User newMember)
        {
            members.Remove(newMember);
        }


    }
}
