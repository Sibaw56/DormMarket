using MongoDB.Driver; 
public class MongoDBService 
    { 
        private readonly IMongoDatabase _database; 
        public MongoDBService(IConfiguration configuration) 
        { 
            var client = new MongoClient( configuration["MongoDB:ConnectionURI"]); 
            _database = client.GetDatabase( configuration["MongoDB:DatabaseName"]); 
        } 
    public IMongoCollection<studentUser> Students => _database.GetCollection<studentUser>("Students"); 

}
