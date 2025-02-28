using DnsClient;

namespace ProfileService.Models
{
    public class User
    {
        public bool enabled { set; get; }
        public string id { set; get; }
        public string login { set; get; }
        public string name { set; get; }
        public List<string> permissions { set; get; }

        public User(bool enabled, string id, string login, string name, List<string> permissions)
        {
            this.enabled = enabled;
            this.id = id;
            this.login = login;
            this.name = name;
            this.permissions = permissions;
        }
    }

   
}
