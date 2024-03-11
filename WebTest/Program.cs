using Microsoft.AspNetCore.Mvc;
using WebTest.Boot.Configure;
using WebTest.Boot.Register;
using WebTest.Jobs;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.AddAppServices();
builder.AddTimeServices();
builder.AddExceptionServices();
builder.AddDbServices();
builder.AddLocalServices();
builder.AddSmtpClient();
builder.AddSwagger();
builder.AddCronJob<DeleteExpiredTokens>("* * * * *");
var app = builder.Build();

//app.RequestEnableBuffering();
app.UseRouting();
app.MapControllers();
app.UseExceptionHandler();
app.UseAuthorization();
app.DatabaseMigrate();
app.GenerateSwagger();

app.MapGet("/", () => "App started!");

app.Run();

public partial class Program { }
