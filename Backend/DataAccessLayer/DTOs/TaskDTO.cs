using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class TaskDTO : DTO
    {

        public const string TaskId = "ID";
        public const string CreationTimeColumnName = "CreationTime";
        public const string DueDateColumnName = "DueDate";
        public const string TitleColumnName = "Title";
        public const string DescriptionColumnName = "Description";
        public const string AssigneeColumnName = "Assignee";
        public const string ColumnOrdinalColumnName = "ColumnOrdinal";
        public const string BoardIDColumnName = "BoardID";



        /// <summary>
        /// task id field and its getter & setter
        /// </summary>
        public long Id { get; private set; } = -1;
        /// <summary>
        /// task creation date field and its getter & setter
        /// </summary>
        private DateTime _creationTime;
        public DateTime CreationTime { get => _creationTime; private set { _creationTime = value; _controller.Update(_primaryKeys, _primaryVals, CreationTimeColumnName, value.ToString()); } }
        /// <summary>
        /// task due date field and its getter & setter
        /// </summary>
        private DateTime _dueDate;
        public DateTime DueDate { get => _dueDate; set { _dueDate = value; _controller.Update(_primaryKeys, _primaryVals, DueDateColumnName, value.ToString()); } }
        /// <summary>
        /// task title field and its getter & setter
        /// </summary>
        private string _title;
        public string Title { get => _title; set { _title = value; _controller.Update(_primaryKeys, _primaryVals, TitleColumnName, value); } }
        /// <summary>
        /// task description field and its getter & setter
        /// </summary>
        private string _description;
        public string Description { get => _description; set { _description = value; _controller.Update(_primaryKeys, _primaryVals, DescriptionColumnName, value); } }
        /// <summary>
        /// task assignee field and its getter & setter
        /// </summary>
        private string _assignee;
        public string Assignee { get => _assignee; set { _assignee = value; _controller.Update(_primaryKeys, _primaryVals, AssigneeColumnName, value); } }

        /// <summary>
        /// column ordinal field and its getter & setter
        /// </summary>
        private long _columnOrdinal;
        public long ColumnOrdinal { get => _columnOrdinal; set { _columnOrdinal = value; _controller.Update(_primaryKeys, _primaryVals, ColumnOrdinalColumnName, value); } }
        /// <summary>
        /// board id field and its getter
        /// </summary>
        private long _boardId;
        public long BoardId { get => _boardId; }


        /// <summary>
        /// TaskDTO constructor
        /// </summary>
        /// <param name="ID">task id</param>
        /// <param name="creationTime">task creation time</param>
        /// <param name="dueDate">task due date</param>
        /// <param name="title">task title</param>
        /// <param name="description">task description</param>
        /// <param name="assignee">task assignee</param>
        /// <param name="columnOrdinal">column ordinal(0-backlog,1-in progress,2-done)</param>
        /// <param name="boardId">board id</param>
        public TaskDTO(int ID, DateTime creationTime, DateTime dueDate, string title, string description, string assignee, int columnOrdinal, long boardId) : base(new TaskDalController(), new string[] { BoardIDColumnName, TaskDTO.TaskId }, new object[] { boardId, TaskId })
        {
            Id = ID;
            _creationTime = creationTime;
            _dueDate = dueDate;
            _title = title;
            _description = description;
            _assignee = assignee;
            _columnOrdinal = columnOrdinal;
            _boardId = boardId;
        }

    }
}
