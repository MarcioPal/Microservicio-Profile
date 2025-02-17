namespace ProfileService.Models
{
    public class History
    {
         public int Id { get; set; }
        string id_user { get; set; }
        string article_id { get; set; }
        DateTime date {  get; set; }

    }

    public class Tag { 
        public string id { get; set; }
        public string name { get; set; }
        public List<string> articles { get; set; }
    }
}
