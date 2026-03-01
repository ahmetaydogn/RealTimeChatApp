using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Token Implementation
builder.Services.AddScoped<IAuthService, AuthProvider>();
builder.Services.AddScoped<IAppUserDal, EfAppUserDal>();
builder.Services.AddScoped<IRoomDal, EfRoomDal>();

// Chat Service and Manager Implementation
builder.Services.AddScoped<IChatService, ChatManager>();
builder.Services.AddScoped<IMessageDal, EfMessageDal>();


// To provide jwt token authentication, we need to add the authentication services and configure the JWT bearer options.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


// To provide auto migration, we need to add the DbContext to the services container.
builder.Services.AddDbContext<RealTimeChatAppContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// To provide redis cache, we need to add the IConnectionMultiplexer to the services container and configure it to connect to the Redis server using the connection string from the configuration.
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));


// To provide sse support, we need to add response compression services and configure it to include the "text/event-stream" MIME type, which is used for server-sent events.
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["text/event-stream"]);
});

var app = builder.Build();


// To provide auto migration, we need to create a scope and get the DbContext from the service provider,
// then call the Migrate method on the database.
// This will apply any pending migrations to the database when the application starts.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<RealTimeChatAppContext>();
        db.Database.Migrate();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// JWT Token Implementation
app.UseAuthentication();
app.UseAuthorization();

// To provide SSE support, we need to use response compression middleware in the request pipeline. This will enable compression for the server-sent events, which can help reduce bandwidth usage and improve performance.
app.UseResponseCompression();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
