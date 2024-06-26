using BBS.Common;
using BBS.Data;
using BBS.IRepository;
using BBS.IService;
using BBS.Repository;
using BBS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "azure-diagnostics-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});
builder.Services.Configure<AzureBlobLoggerOptions>(options =>
{
    options.BlobName = "log.txt";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "frontend",
                      policy =>
                      {
                          policy.WithOrigins("https://victorious-cliff-0fe836900.5.azurestaticapps.net", "https://hy-rf.github.io", "http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                      });
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Environment.IsDevelopment() ? builder.Configuration.GetValue<string>("JWT")! : Environment.GetEnvironmentVariable("JWT")))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                string path = ctx.HttpContext.Request.Path;
                if (path != "/chat")
                {
                    ctx.Token = ctx.Request.Headers.Authorization;
                }
                else
                {
                    ctx.Token = ctx.Request.Query["access_token"];
                }
                return Task.CompletedTask;
            }
        };
    });



//builder.Services.AddDbContextPool<AppDbContext>(options =>
//{
//    options.UseSqlite(builder.Configuration.GetConnectionString("LocalDB"));
//});
builder.Services.AddDbContext<ForumContext>(options =>
{
    options.UseSqlServer(builder.Environment.IsDevelopment() ? builder.Configuration.GetConnectionString("AzureDB") : Environment.GetEnvironmentVariable("AzureDB"));
});

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPostTagRepository, PostTagRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IReplyService, ReplyService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();



builder.Services.AddTransient<IUserIdProvider, MyUserIdProvider>();

var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<BBS.Hubs.Notification>("/notification");
app.MapHub<BBS.Hubs.ChatRoom>("/chat");


app.Run();