using HomeSearch.Server.Models;
using HomeSearch.Server.Repositories;
using HomeSearch.Server.Settings;

var builder = WebApplication.CreateBuilder(args);

// <---  Секция конфигурации сервисов --->
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

// Добавляем конфигурацию базы данных Mongo
builder.Services.AddSingleton<MongoDbConfig>(mongoDbSettings);

// Добавляем маппинг сущности для аутентификации на базу дыннх из конфигурации
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
    (
        mongoDbSettings.ConnectionString, mongoDbSettings.DataBaseName
    );

builder.Services.AddScoped<IHomeRepository, HomeRepositoryMongoDb>();

// Добавляем куки
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// <--- Секция конифгурации PipeLine --->
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    Console.WriteLine("ConnectionString:" + mongoDbSettings.ConnectionString);
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

