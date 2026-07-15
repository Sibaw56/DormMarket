using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
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

    // Replace "Students" with your actual collection name if it's different.
    var students = database.GetCollection<studentUser>("Students");

    // Looks for a document where Id == 1
    var student = students.Find(s => s.Id == 1).FirstOrDefault();

    if (student != null)
    {
        Console.WriteLine($"✅ Connected!");
        Console.WriteLine($"Student: {student.FirstName} {student.LastName}");
    }
    else
    {
        Console.WriteLine("✅ Connected to MongoDB, but no student with Id = 1 was found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("❌ Connection failed.");
    Console.WriteLine(ex.ToString());
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.Run();
//real real real real real