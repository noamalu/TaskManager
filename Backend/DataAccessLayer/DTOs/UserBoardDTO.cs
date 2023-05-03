using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class UserBoardDTO:DTO
    {

        /// <summary>
        /// column names(UserBoard table):
        /// </summary>
        public const string BoardIDColumnName = "BoardID";
        public const string MemberEmailColumnName = "MemberEmail";


        
        private long _id;
        public long ID { get => _id; }
        /// <summary>
        /// board member email field and its getter and setter
        /// </summary>
        private string memberEmail;
        public string Members { get => memberEmail; set { memberEmail = value; _controller.Update(_primaryKeys, _primaryVals, MemberEmailColumnName, value); } }


        /// <summary>
        /// UserBoardDTO constructor
        /// </summary>
        /// <param name="members">board member email</param>
        /// <param name="id">board id</param>
        public UserBoardDTO(string members, long id) : base(new UserBoardDalController(),new string[] { BoardIDColumnName, MemberEmailColumnName }, new object[] { id, members })
        {
            memberEmail = members;
            _id = id;
        }
    }
}
