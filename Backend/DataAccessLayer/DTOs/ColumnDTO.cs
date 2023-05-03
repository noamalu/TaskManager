using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    
    public class ColumnDTO:DTO
    {
       
        public const string LimitColumnName = "Limit";
        public const string NameColumnName = "ColumnName";

        /// <summary>
        /// column limit field and its getter & setter
        /// </summary>
        private long _limit;
        public long Limit { get => _limit; set { _limit = value; _controller.Update(_primaryKeys, _primaryVals, LimitColumnName, value); } }
        /// <summary>
        /// column name field and its getter & setter
        /// </summary>
        private string _name;
        public string Name { get => _name; set { _name = value; _controller.Update(_primaryKeys, _primaryVals, NameColumnName, value); } }
    
        /// <summary>
        /// board id field and its getter
        /// </summary>
        private long _id;
        public long ID { get => _id; }

        /// <summary>
        /// ColumnDTO constructor
        /// </summary>
        /// <param name="limit">column tasks limit</param>
        /// <param name="name">column name</param>
        /// <param name="id">board id</param>
        public ColumnDTO(long id ,long limit, string name):base(new ColumnDalController(),new string[] {IDColumnName,NameColumnName },new object[] {id,name})
        {
            _id = id;
            _name = name;
            _limit = limit;
        }
    }
}
