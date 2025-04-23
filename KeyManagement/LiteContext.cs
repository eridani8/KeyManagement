using LiteDB;

namespace KeyManagement;

public class LiteContext
{
    public ILiteCollection<User> Users { get; }
    public ILiteCollection<UserKey> Keys { get; }
    public ILiteDatabase Db { get; }
    public LiteContext()
    {
        var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db"));
        
        Db = new LiteDatabase(path);
        Users = Db.GetCollection<User>("users");
        Keys = Db.GetCollection<UserKey>("keys");
        
        var users = Users.FindAll().ToList();
        if (users.All(u => u.Username != "root"))
        {
            Users.Insert(new User()
            {
                Id = ObjectId.NewObjectId(),
                Username = "root",
                Password = @"zJx]],>1x'(4j1HM9X:-{^L]X\\>",
                Role = "admin"
            });
        }
    }
}

public class User
{
    public required ObjectId Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Role { get; init; }
}

public class UserKey
{
    public required ObjectId Id { get; init; }
    public required string Login { get; set; }
    public required Guid Key { get; init; }
    public int RequestsCount { get; set; }
    public required DateTime Expires { get; set; }
}