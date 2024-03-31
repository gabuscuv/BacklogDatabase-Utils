namespace GameListDB.DTO
{
    public class Config
    {
        public Backloggd? BACKLOGGD { get; set; }
        public string? IGDB_CLIENT_ID { get; set; }
        public string? IGDB_CLIENT_SECRET { get; set; }
        public string? LIST_DEFAULT_OUTPUT { get; set; }
    }

    public class Backloggd 
    {
        public string BACKLOGGD_ID { get; set; }
        public string COOKIE_TOKEN { get; set; }
        public string CSRF_TOKEN { get; set; }
        public string BACKLOGGD_SESSION { get; set; }
        public string REMEMBER_USER_TOKEN { get; set; }

        
    } 
}
