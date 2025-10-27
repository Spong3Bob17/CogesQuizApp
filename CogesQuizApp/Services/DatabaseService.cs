using CogesQuizApp.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace CogesQuizApp.Services
{
    public class DatabaseService
    {
        private readonly IMongoDatabase _database;

        public DatabaseService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Test> Tests => _database.GetCollection<Test>("tests");
        public IMongoCollection<Result> Results => _database.GetCollection<Result>("results");

        public List<Test> GetAllTests() => Tests.Find(_ => true).ToList();
        public Test GetTestById(string id) => Tests.Find(t => t.Id == id).FirstOrDefault();
        
        public void SaveResult(Result result)
        {
            Results.InsertOne(result);
        }

        public List<Result> GetAllResults()
        {
            return _database.GetCollection<Result>("results")
                .Find(_ => true)
                .SortByDescending(r => r.Date)
                .ToList();
        }

    }
}