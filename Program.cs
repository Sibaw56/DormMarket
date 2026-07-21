using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<MongoDBService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

try
{
    var client = new MongoClient(builder.Configuration["MongoDB:ConnectionURI"]);

    var database = client.GetDatabase("DorMap");
    var students = database.GetCollection<studentUser>("Students");
    var student = students.Find(s => s.Id == 1).FirstOrDefault();

    if (student != null)
    {
        Console.WriteLine("Connected to MongoDB! yay");
        Console.WriteLine($"Student: {student.FirstName} {student.LastName} {student.Email} {student.Phone} {student.DateJoined}");
    }
    else
    {
        Console.WriteLine("Connected to MongoDB! yay");
        Console.WriteLine("No student with Id = 1 was found but still connected.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Connection failed. u stinky");
    Console.WriteLine(ex.ToString());
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.Run();
//real real real real real