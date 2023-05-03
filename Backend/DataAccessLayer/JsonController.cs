using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class JsonController
    {
        protected const string basePath = "persistance";
        protected const string subDirectory = "users"; //You can always use the obj.GetType().Name; we used last week instead
        internal string Read(string filename)
        {
            return File.ReadAllText( filename);
        }
        internal void Write(string filename, string contents)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleanFileName = filename;
            foreach (char c in invalidChars)
            {
                cleanFileName = cleanFileName.Replace(c.ToString(), "");
            }
            cleanFileName += ".json";
            string path = Path.Combine(Directory.GetCurrentDirectory(), basePath, subDirectory);
            string fullPath = Path.Combine(path, cleanFileName);
            Directory.CreateDirectory(path);
            File.WriteAllText(fullPath, contents);

        }
        private string[] GetUserNames()//can be used to get list of user in system. Or you can use another file for that.
        {
            if (Directory.Exists(Path.Combine(basePath, subDirectory)))
            {
                return Directory.GetFiles(Path.Combine(basePath, subDirectory));
            }
            return new List<string>().ToArray();
        }

        internal List<User> LoadAllUsers()
        {
            List<User> ans = new List<User>();
            foreach (var email in this.GetUserNames())
            {
                User u = this.ImportUser(email);
                ans.Add(u);                
            }
            return ans;
        }

        public User ImportUser(string path)
        {
            return FromJson(Read(path));
        }

        private User FromJson(string json)
        {
            return JsonSerializer.Deserialize<User>(json);
        }
    }
}
