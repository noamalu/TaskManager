using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    public class BoardDTO : DTO
    {
        public const string OwnerEmailColumnName = "OwnerEmail";
        public const string BoardNameColumnName = "BoardName";



        private string _ownerEmail;
        /// <summary>
        /// creator email field and its getter and setter
        /// </summary>
        public string OwnerEmail { get => _ownerEmail; private set { _controller.Update(_primaryKeys, _primaryVals, OwnerEmailColumnName, value); } }

        private string _boardName;
        /// <summary>
        /// board name field and its getter and setter
        /// </summary>
        public string BoardName { get => _boardName; private set { _boardName = value; _controller.Update(_primaryKeys, _primaryVals, BoardNameColumnName, value); } }

        public BoardDTO(long id, string creatorEmail, string boardName) : base(new BoardDalController(), new string[] { IDColumnName }, new Object[] { id })
        {
            this.Id = id;
            _ownerEmail = creatorEmail;
            _boardName = boardName;
        }
    }
}