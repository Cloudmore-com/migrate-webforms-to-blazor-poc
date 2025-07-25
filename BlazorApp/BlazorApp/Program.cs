using BlazorApp.Client.Pages;
using BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSystemWebAdapters()
    .AddJsonSessionSerializer(options => {
        options.RegisterKey<bool>("UserInitialized");
        options.RegisterKey<string>("SessionID");
        options.RegisterKey<DateTime>("VisitTime");
        options.RegisterKey<string>("BlazorString");
    })
    .AddRemoteAppClient(options =>
    {
        options.RemoteAppUrl = new(builder.Configuration["ProxyTo"]);
        options.ApiKey = builder.Configuration["RemoteAppApiKey"];
    })
    .AddSessionClient();

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

//builder.Services.AddSystemWebAdapters();
builder.Services.AddHttpForwarder();

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

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly)
    .RequireSystemWebAdapterSession();

app.UseHttpsRedirection();
app.MapForwarder("/Scripts/{**catchAll}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = 1);
app.MapForwarder("/Content/{**catchAll}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = 2);
app.MapForwarder("/bundles/{**catchAll}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = 3);
app.MapForwarder("/About", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = 4);
app.MapForwarder("/Contact", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = 5);

app.Run();
