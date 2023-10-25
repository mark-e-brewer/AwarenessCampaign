using AwarenessCampaign.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using AwarenessCampaign;
using Microsoft.AspNetCore.Builder;
using System.Runtime.CompilerServices;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7136",
                                "http://localhost:5206")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<AwarenessCampaignDbContext>(builder.Configuration["AwarenessCampaignDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/uservalidate/{uid}", (AwarenessCampaignDbContext db, string uid) =>
{
    var userExists = db.Users.Where(x => x.UID == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});

app.MapGet("/users", (AwarenessCampaignDbContext db) =>
{
    return db.Users.ToList();
});

app.MapGet("/posts", (AwarenessCampaignDbContext db) =>
{
    return db.Posts.ToList();
});

app.MapGet("/post/{id}", (AwarenessCampaignDbContext db, int id) => 
{
    var post = db.Posts.FirstOrDefault(p => p.Id == id);
    if  (post == null) 
    {
        return Results.NotFound();
    }
    return Results.Ok(post);
});

app.MapPost("/post", async (AwarenessCampaignDbContext db, Post post) => 
{
    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/post/{post.Id}", post);
});

app.Run();
