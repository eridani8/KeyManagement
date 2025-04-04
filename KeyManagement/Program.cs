using System.Runtime.InteropServices;
using KeyManagement;
using KeyManagement.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;


try
{
    var builder = WebApplication.CreateBuilder(args);
    
    const string logs = "logs";
    var logsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logs));

    if (!Directory.Exists(logsPath))
    {
        Directory.CreateDirectory(logsPath);
    }
    
    const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(LogEventLevel.Information, outputTemplate: outputTemplate)
        .WriteTo.File($"{logsPath}/.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
        .CreateLogger();
    
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();

    builder.WebHost.ConfigureKestrel(o =>
    {
        o.ListenAnyIP(6739);
    }).UseUrls();
    
    builder.Host.UseSerilog(Log.Logger);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton<LiteContext>();
    
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(o =>
        {
            o.Cookie.Name = "token";
            o.LoginPath = "/login";
            o.Cookie.MaxAge = TimeSpan.FromHours(24);
            o.AccessDeniedPath = "/access-denied";
        });
    builder.Services.AddAuthorization();
    builder.Services.AddCascadingAuthenticationState();

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        builder.Host.UseWindowsService();
    }

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();
    // app.UseHttpsRedirection();
    app.UseAntiforgery();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapStaticAssets();
    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Сервис не смог запуститься");
}
finally
{
    await Log.CloseAndFlushAsync();
}