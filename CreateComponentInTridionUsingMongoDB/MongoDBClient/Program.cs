using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDBToTridion.BAL;
using MongoDBToTridion.BAL.Model;



namespace MongoDBClient
{
    class Program
    {        
        static void Main(string[] args)
        {            
            CallMain(args).Wait();
            Console.ReadLine();
        }
        static async Task CallMain(string[] args)
        {
            /// MongoDB connection String 
            var conString = "mongodb://localhost:27017";
            var Client = new MongoClient(conString);

            /// get Database object
            var DB = Client.GetDatabase("customerDatabase");

            ///get BsonDocument "article"
            var collection = DB.GetCollection<BsonDocument>("article");
           
            var list = await collection.Find(new BsonDocument()).ToListAsync();
            List<Article> _article = new List<Article>();            
            foreach (var docs in list)
            {
                Article a = new Article();
                a.title = docs["title"].ToString();
                a.description = docs["description"].ToString();
                a.imageurl = docs["imageurl"].ToString();
                _article.Add(a);
                
            }
            Generation.process(_article);

        }


    } 
}
