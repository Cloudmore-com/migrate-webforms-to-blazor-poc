using BlazorApp.Components;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options => {
        options.RegisterKey<bool>("UserInitialized");
        options.RegisterKey<string>("SessionID");
        options.RegisterKey<DateTime>("VisitTime");
        options.RegisterKey<string>("BlazorString");
        // Add authentication-related session keys
        options.RegisterKey<string>("UserName");
        options.RegisterKey<bool>("IsAuthenticated");
    })
    .AddRemoteAppClient(options =>
    {
        options.RemoteAppUrl = new(builder.Configuration["ProxyTo"]);
        options.ApiKey = builder.Configuration["RemoteAppApiKey"];
    })
    .AddSessionClient()
    .AddAuthenticationClient(true);



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddWebOptimizer(pipeline => {
    pipeline.AddJavaScriptBundle("/bundles/jquery",
        "Scripts/jquery-3.3.1.js").UseContentRoot();

    pipeline.AddJavaScriptBundle("/bundles/jqueryval",
        "Scripts/jquery.validate.*").UseContentRoot();

    pipeline.AddJavaScriptBundle("/bundles/modernizr",
        "Scripts/modernizr-*").UseContentRoot();

    pipeline.AddJavaScriptBundle("/bundles/bootstrap",
        "Scripts/bootstrap.js",
        "Scripts/respond.js").UseContentRoot();

    pipeline.AddCssBundle("/Content/css",
        "Content/bootstrap.css",
        "Content/custom.css",
        "Content/base.css",
        "Content/site.css").UseContentRoot();
});

builder.Services.AddReverseProxy(); // Changed line
builder.Services.AddAuthorization(); // Added line

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();

app.UseSystemWebAdapters();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly)
    .RequireSystemWebAdapterSession();

app.MapForwarder("/{**catch-all}", app.Services.GetRequiredService<IOptions<RemoteAppClientOptions>>().Value.RemoteAppUrl.OriginalString)
    // Ensures this route has the lowest priority (runs last)
    .WithOrder(int.MaxValue)
    // Skips remaining middleware when this route matches
    .ShortCircuit();

app.Run();
