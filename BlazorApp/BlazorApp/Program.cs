using BlazorApp.Client.Pages;
using BlazorApp.Components;
using System.Diagnostics;
using System.Net;
using Yarp.ReverseProxy.Forwarder;

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

builder.Services.AddHttpForwarder();
builder.Services.AddAuthorization(); // Added line

var httpClient = new HttpMessageInvoker(new SocketsHttpHandler
{
    UseProxy = false,
    AllowAutoRedirect = true,
    AutomaticDecompression = DecompressionMethods.None,
    UseCookies = false,
    EnableMultipleHttp2Connections = true,
    ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
    ConnectTimeout = TimeSpan.FromSeconds(15),
});

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

var redirectTransformer = new RedirectTransformer();

app.MapForwarder("/About", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 4);
app.MapForwarder("/Contact", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 5);
app.MapForwarder("/Login", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 6);
app.MapForwarder("/Logout", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 7);
app.MapForwarder("/Login.aspx", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 8);
app.MapForwarder("/Logout.aspx", app.Configuration["ProxyTo"], new ForwarderRequestConfig(), redirectTransformer, httpClient)
   .Add(static builder => ((RouteEndpointBuilder)builder).Order = 9);

app.Run();

class RedirectTransformer : HttpTransformer
{
    public override async ValueTask<bool> TransformResponseAsync(HttpContext httpContext,
        HttpResponseMessage? proxyResponse,
        CancellationToken cancellationToken)
    {
        if (proxyResponse == null)
        {
            return false;
        }

        // Handle redirects specially
        if ((int)proxyResponse.StatusCode >= 300 && (int)proxyResponse.StatusCode < 400)
        {
            if (proxyResponse.Headers.Location != null)
            {
                // Get the redirect URL
                var locationUrl = proxyResponse.Headers.Location.ToString();

                // Modify the location if it's from the proxied server to make it relative to our app
                string proxyBase = httpContext.RequestServices.GetRequiredService<IConfiguration>()["ProxyTo"] ?? string.Empty;

                if (locationUrl.StartsWith(proxyBase))
                {
                    // Convert to a relative URL for our app
                    locationUrl = locationUrl.Substring(proxyBase.Length - 1); // -1 to keep the leading /
                }

                // Redirect in our app instead
                httpContext.Response.Redirect(locationUrl);
                return true;
            }
        }

        // For non-redirect responses, use the default transformer
        await base.TransformResponseAsync(httpContext, proxyResponse, cancellationToken);
        return true;
    }
}
