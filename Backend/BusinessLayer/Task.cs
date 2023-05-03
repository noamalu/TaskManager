using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using Microsoft.VisualBasic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Task
    {
        internal readonly TaskDTO taskDTO;


        
        private int id;
        private DateTime creationTime;
        private DateTime dueDateTime;
        private string title;
        private string description;
        private string assignee;
        private int coulmnOrdinal;
             

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       
        public int Id { get => id; set => id = value; }
        public DateTime CreationTime { get => creationTime; set => creationTime = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public DateTime DueDate { get => dueDateTime; set => dueDateTime = value; }
        
        [JsonIgnore]
        public string Assignee { get => assignee; set => assignee = value; }
        [JsonIgnore]
        public int CoulmnOrdinal { get => coulmnOrdinal; set => coulmnOrdinal = value; }


        public Task(DateTime dueDate, string title, string description, long boardId)
        {

            if (!checkTitle(title))
            {
                log.Error("entered invalid title");
                throw new Exception("title should be max. 50 characters, not empty");
            }
            if (!checkDescription(description))
            {
                log.Error("entered invalid description");
                throw new Exception("description should have max. 300 characters, optional");
            }
            if (dueDate == null || dueDate < DateTime.Now)
            {
                log.Error("entered useless date");
                throw new Exception("due-date time is not valid");
            }

            this.Title = title;
            this.Description = description;
            this.DueDate = dueDate;
            creationTime = DateTime.Now;
            assignee = "unAssigned";

            

            this.taskDTO = new TaskDTO(id, creationTime, DueDate, title, description, assignee, -1, boardId);

        }


        /// <summary>
        /// Set new duedate for the task
        /// </summary>
        /// /// <param name="dueDate">the new title of the task</param>
        /// <returns>set the new duedate , throw Exception if not</returns>
        public void updateTaskDueDate(DateTime dueDate,string email)
        {
            if (!email.Equals(assignee))
            {
                throw new Exception("a task can be changed by the asignee only");
            }
            if (dueDate == null || dueDate.CompareTo(DateTime.Now) < 0)
            {
                log.Error("entered invalid date");

                throw new Exception("due-date time is not valid");
            }
            else
                this.DueDate = dueDate;

            this.taskDTO.DueDate = dueDate;

        }

        /// <summary>
        /// Set new title for the task
        /// </summary>
        /// /// <param name="title">the new title of the task</param>
        /// <returns>set the new title , throw Exception if not</returns>
        public void changeTitle(string title,string email)
        {
            if (!email.Equals(assignee))
            {
                throw new Exception("a task can be changed by the asignee only");
            }
            if (!checkTitle(title))
            {
                log.Error("entered invalid title");
                throw new Exception("title must be max. 50 characters, not empty");
            }
            else
                this.Title = title;
            this.taskDTO.Title = title;

        }

        /// <summary>
        /// Set new description for the task
        /// </summary>
        /// /// <param name="description">the new title of the task</param>
        /// <returns>set the new description , throw Exception if not</returns>
        public void changeDescription(string description,string email)
        {
            if (!email.Equals(assignee))
            {
                throw new Exception("a task can be changed by the asignee only");
            }

            if (!checkDescription(description))
            {
                log.Error("entered invalid description");
                throw new Exception("description should have max. 300 characters, optional");
            }
            else
                this.Description = description;
            this.taskDTO.Description = description;

        }


        /// <summary>
        /// Check if the title is valid
        /// </summary>
        /// /// <param name="title">the title to check if valid</param>
        /// <returns>true if valid , false else</returns>
        public Boolean checkTitle(string title)
        {
            if (title != null && (title.Length > 0 & title.Length < 51))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if the description is valid
        /// </summary>
        /// /// <param name="description">the title to check if valid</param>
        /// <returns>true if valid , false else</returns>
        public Boolean checkDescription(string description)
        {
            if (description != null && description.Length <= 300)
                return true;
            else
                return false;
        }

        
    }



}
