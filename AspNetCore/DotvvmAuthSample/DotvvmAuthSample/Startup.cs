using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using DotVVM.Framework.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace DotvvmAuthSample
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public Startup(IWebHostEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();
            services.AddDotVVM();
            
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = Configuration["AzureAD:ClientId"];
                options.Authority = string.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]);
                options.SignedOutRedirectUri = Configuration["AzureAd:PostLogoutRedirectUri"];
                //options.ResponseType = "code";

                if (options.ResponseType == "code")
                {
                    options.ClientSecret = Configuration["AzureAd:ClientSecret"];
                }

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = (Configuration["AzureAd:Tenant"] != "common")
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/error");
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        var message = context.ProtocolMessage;

                        if (!string.IsNullOrEmpty(message.State))
                        {
                            context.Properties.Items[OpenIdConnectDefaults.UserstatePropertiesKey] = message.State;
                        }
                        
                        context.Properties.Items.Add(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey, message.RedirectUri);
                        message.State = context.Options.StateDataFormat.Protect(context.Properties);
                        DotvvmAuthenticationHelper.ApplyRedirectResponse(context.HttpContext, message.BuildRedirectUrl());
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();
			
            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
            
            // use static files
            app.UseStaticFiles();
        }
    }
}
