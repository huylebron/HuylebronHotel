using HotelProject . Api ;
using HotelProject . Application ;
using HotelProject . Domain . Abstractions . ApplicationServices ;
using HotelProject . Infrastructure ;
using HotelProject . Persistence ;
using Microsoft . EntityFrameworkCore ;

var builder = WebApplication . CreateBuilder ( args ) ;

// Add services to the container.

builder . Services . AddControllers ( ) ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder . Services . AddEndpointsApiExplorer ( ) ;
builder . Services . AddSwaggerGen ( ) ;
// add this service to use IHttpContextAcessor in another class
builder.Services.AddHttpContextAccessor();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCORS();


#region Persistence

builder.Services.AddSqlServerPersistence(builder.Configuration);
builder.Services.AddRepositoryUnitOfWork();


#endregion
#region Infrastructure
builder.Services.AddServicesInfrastructure();
builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("JwtOption"));
#endregion  
#region Application
builder.Services.AddServicesApplication();
#endregion





var app = builder . Build ( ) ;

// Configure the HTTP request pipeline.
if ( app . Environment . IsDevelopment ( ) )
{
    app . UseSwagger ( ) ;
    app . UseSwaggerUI ( ) ;
}

app . UseHttpsRedirection ( ) ;

app . UseAuthorization ( ) ;
app.UseStaticFiles();
app.UseAuthentication();

app . MapControllers ( ) ;
InitDatabase(app);
EnsureDirectoriesExist(app);
void EnsureDirectoriesExist(IApplicationBuilder appBuilder)
{
    var webHostEnvironment = appBuilder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
    var webRootPath = webHostEnvironment.WebRootPath;
    
    // Tạo wwwroot nếu không tồn tại
    if (string.IsNullOrEmpty(webRootPath))
    {
        webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(webRootPath);
    }
    
    // Tạo các thư mục con
    var directories = new[]
    {
        Path.Combine(webRootPath, "generalImages"),
        Path.Combine(webRootPath, "hotelImages"),
        Path.Combine(webRootPath, "roomImages"),
        Path.Combine(webRootPath, "roomTypeImages"),
        Path.Combine(webRootPath, "serviceImages")
    };
    
    foreach (var directory in directories)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}

app . Run ( ) ;
void InitDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    var userService = serviceScope.ServiceProvider.GetRequiredService<IUserService>();
    userService.InitializeUserAdminAsync().Wait();
    InitializeTestData.SeedTestData(app).Wait();
}
