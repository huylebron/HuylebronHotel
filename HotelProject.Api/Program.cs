using HotelProject . Infrastructure ;
using HotelProject . Persistence ;

var builder = WebApplication . CreateBuilder ( args ) ;

// Add services to the container.

builder . Services . AddControllers ( ) ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder . Services . AddEndpointsApiExplorer ( ) ;
builder . Services . AddSwaggerGen ( ) ;

#region Persistence

builder.Services.AddSqlServerPersistence(builder.Configuration);
builder.Services.AddRepositoryUnitOfWork();


#endregion
#region Infrastructure
builder.Services.AddServicesInfrastructure();
builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("JwtOption"));
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

app . MapControllers ( ) ;

app . Run ( ) ;