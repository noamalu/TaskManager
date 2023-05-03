
using System.Text.Json;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class UserDTO : DTO
    {
        public const string UserEmailColumnName = "Email";
        public const string UserPasswordColumnName = "Password";

        JsonController dalController;


        
        private string _email;
        /// <summary>
        /// email field and its getter and setter
        /// </summary>
        public string Email { get => _email; set { _email = value; _controller.Update(_primaryKeys,_primaryVals,UserEmailColumnName, value); } }

        private string _password;
        /// <summary>
        /// password field and its getter and setter
        /// </summary>
        public string Password { get => _password; set { _password = value; _controller.Update(_primaryKeys, _primaryVals, UserPasswordColumnName, value); } }

        public UserDTO(long id, string email, string password) : base(new UserDalController(),new string[]{IDColumnName},new object[]{id})
        {
            
            this.dalController = new JsonController();
            Id = id;
            _email = email;
            _password = password;
        }

         public void Save()
        {
            dalController.Write(Email, ToJson());
        }

        private string ToJson()
        {
            return JsonSerializer.Serialize(this, this.GetType());
        }

    }
}
