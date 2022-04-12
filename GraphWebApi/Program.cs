

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

string MyAllowSpecificOrigins = "_AppAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd")
                    .EnableTokenAcquisitionToCallDownstreamApi()
                        .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
                        .AddInMemoryTokenCaches();

// Allowing CORS for react application.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                                   builder =>
                                   {
                                      builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
                                       .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                      // builder.SetIsOriginAllowed(isOriginAllowed: _ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                   });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
