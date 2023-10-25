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

//User Endpoints

//Get All Users
app.MapGet("/users", (AwarenessCampaignDbContext db) =>
{
    return db.Users.ToList();
});

// Get User by id
app.MapGet("/api/users/{id}", (AwarenessCampaignDbContext db, int id) =>
{
    var user = db.Users.SingleOrDefault(u => u.Id == id);
    if (user == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(user);
    }
});

// Category Endpoints

// Create Category
app.MapPost("/api/category", (AwarenessCampaignDbContext db, Category category) =>
{
    db.Category.Add(category);
    db.SaveChanges();
    return Results.Created($"/api/categories/{category.Id}", category);
});

// Get All Categories
app.MapGet("/api/categories", (AwarenessCampaignDbContext db) =>
{
    var categories = db.Category.ToList();
    return Results.Ok(categories);
});

// Get Category by id
app.MapGet("/api/categories/{id}", (AwarenessCampaignDbContext db, int id) =>
{
    var category = db.Category.SingleOrDefault(c => c.Id == id);
    if (category == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(category);
    }
});

// Update Category by id
app.MapPut("/api/categories/{id}", (AwarenessCampaignDbContext db, int id, Category updatedCategory) =>
{
    var existingCategory = db.Category.SingleOrDefault(c => c.Id == id);
    if (existingCategory == null)
    {
        return Results.NotFound();
    }

    existingCategory.Name = updatedCategory.Name;
    // Update other properties as needed...

    db.SaveChanges();
    return Results.Ok();
});

// Delete Category by id
app.MapDelete("/api/categories/{id}", (AwarenessCampaignDbContext db, int id) =>
{
    var category = db.Category.SingleOrDefault(c => c.Id == id);
    if (category == null)
    {
        return Results.NotFound();
    }

    db.Category.Remove(category);
    db.SaveChanges();
    return Results.NoContent();
});

// Join Table Endpoints

// Associate Category with Post
app.MapPost("/api/posts/{postId}/categories/{categoryId}", (AwarenessCampaignDbContext db, int postId, int categoryId) =>
{
    try
    {
        // Retrieve the post from the database
        Post post = db.Posts.FirstOrDefault(p => p.Id == postId);
        if (post == null)
            return Results.NotFound("Post not found.");

        // Retrieve the category from the database
        Category category = db.Category.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
            return Results.NotFound("Category not found.");

        // Ensure the post's Categories collection is initialized
        if (post.Categories == null)
            post.Categories = new List<Category>();

        // Add the category to the post
        post.Categories.Add(category);

        // Save changes to the database
        db.SaveChanges();

        return Results.Ok("Category associated with the post successfully.");
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while associating the category with the post.", ex.Message);
    }
});

// Dissociate Category from Post
app.MapDelete("/api/posts/{postId}/categories/{categoryId}", (AwarenessCampaignDbContext db, int postId, int categoryId) =>
{
    // Retrieve the post from the database
    Post post = db.Posts.FirstOrDefault(p => p.Id == postId);
    if (post == null)
        return Results.NotFound("Post not found.");

    // Retrieve the category from the database
    Category category = db.Category.FirstOrDefault(c => c.Id == categoryId);
    if (category == null)
        return Results.NotFound("Category not found.");

    // Check if the category is associated with the post
    if (post.Categories.Contains(category))
    {
        // Remove the category from the post
        post.Categories.Remove(category);

        // Save changes to the database
        db.SaveChanges();

        return Results.Ok("Category dissociated from the post successfully.");
    }
    else
    {
        return Results.NotFound("Category is not associated with the post.");
    }
});

app.Run();
