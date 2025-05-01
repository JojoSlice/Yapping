using Microsoft.AspNetCore.Identity;
using miniReddit.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.NetworkInformation;

namespace miniReddit.Services
{
    public class MongoDB
    {
        private readonly IMongoCollection<Models.User> _users;
        private readonly IMongoCollection<Models.Post> _posts;
        private readonly IMongoCollection<Models.Category> _categories;
        private readonly IMongoCollection<Models.Comment> _comments;
        public MongoDB(IConfiguration config)
        {
            var settings = MongoClientSettings.FromConnectionString(config["MongoDb:ConnectionString"]);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
            var client = new MongoClient(settings);
            var db = client.GetDatabase(config["MongoDb:Database"]);
            _users = db.GetCollection<Models.User>("Users");
            _posts = db.GetCollection<Models.Post>("Posts");
            _categories = db.GetCollection<Models.Category>("Categories");
            _comments = db.GetCollection<Models.Comment>("Comments");
        }

        //------------------ Category Controller --------------------------||
        public async Task CreateCategory(Models.Category category)
        {
            await _categories.InsertOneAsync(category);
        }

        public async Task<List<Models.Category>> GetCategories()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        public async Task<Models.Category?> GetCategory(string name)
        {
            return await _categories.Find(cat => cat.Name == name).FirstOrDefaultAsync();
        }

        //---------------- Post Controller --------------------------------||

        public async Task CreatePost(Models.Post post)
        {
            await _posts.InsertOneAsync(post);
        }
        public async Task<List<Models.Post>> GetLatestPosts(DateTime? lastPost)
        {
            try
            {

                if (lastPost != null)
                {
                    var filter = Builders<Post>.Filter.Lt(p => p.CreatedAt, lastPost);
                    return await _posts.Find(filter)
                        .SortByDescending(p => p.CreatedAt)
                        .Limit(10)
                        .ToListAsync();
                }
                else
                {
                    return await _posts.Find(_ => true)
                        .SortByDescending(p => p.CreatedAt)
                        .Limit(10)
                        .ToListAsync();
                }
            }
            catch
            {
                return [];
            }
        }

        public async Task<List<Models.Post>> GetPostsInCategory(Models.Category category)
        {
            return await _posts.Find(post => post.CategoryId == category.Id).ToListAsync();
        }

        public async Task<List<Models.Post>> GetUserPosts(Models.User user)
        {
            return await _posts.Find(Post => Post.UserId == user.Id).ToListAsync();
        }

        //------------- Comment Controller --------------------------------||

        public async Task CreateComment(Models.Comment comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        public async Task<List<Models.Comment>> GetCommentsOnPost(Models.Post post)
        {
            return await _comments.Find(comment => comment.PostId == post.Id).ToListAsync();
        }

        public async Task<List<Models.Comment>> GetUserComments(Models.User user)
        {
            return await _comments.Find(comment => comment.UserId == user.Id).ToListAsync();
        }

        //----------- User Controller -------------------------------------||


        public async Task UpdateProfilePic(string userId, string filePath)
        {
                var filter = Builders<Models.User>.Filter.Eq(user => user.Id, userId);
                var update = Builders<Models.User>.Update.Set(user => user.ProfileImg, filePath);

                await _users.UpdateOneAsync(filter, update);
        }
        public async Task<bool> LogIn(string username, string password)
        {
            var user = await _users.Find(user => user.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return false;

            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                return true;
            else
                return false;           
        }
        public async Task<string> GetUserId(string username)
        {
            var user = await _users.Find(user =>user.Username == username).FirstOrDefaultAsync();
            return user.Id;
        }

        public async Task<Models.User> GetUserFromId(string id)
        {
            return await _users.Find(user =>user.Id == id).FirstOrDefaultAsync();
        }

        public async Task RegisterUser(Models.User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            var existingUser = await _users.Find(user => user.Username == username).FirstOrDefaultAsync();
            return existingUser != null;
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            var existingUser = await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
            return existingUser != null;
        }
    }
}
