using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

[assembly: OwinStartup(typeof(DotvvmAuthSample.Startup))]
namespace DotvvmAuthSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

            ConfigureAuth(app);

            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath, options: options =>
            {
                options.AddDefaultTempStorages("temp");
            });
#if !DEBUG
            dotvvmConfiguration.Debug = false;
#endif

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            // we need the Cookie Authentication middleware to persist the authentication token
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                ExpireTimeSpan = TimeSpan.FromHours(12),
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            });

            // set cookie authentication type as default
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // configure Azure AD authentication
            var authority = new Uri("https://login.microsoftonline.com/" + ConfigurationManager.AppSettings["ida:TenantId"] + "/");
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["ida:ClientId"],
                Authority = authority.ToString(),
                MetadataAddress = new Uri(authority, ".well-known/openid-configuration").ToString(),

                TokenValidationParameters = new TokenValidationParameters()
                {
                    // we cannot validate issuer in multi-tenant scenarios
                    ValidateIssuer = (ConfigurationManager.AppSettings["ida:TenantId"] == "common")
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        // redirect to error page if the authentication fails
                        context.OwinContext.Response.Redirect("/error");
                        context.HandleResponse();

                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = context =>
                    {
                        // determines the base URL of the application (useful when the app can run on multiple domains)
                        var appBaseUrl = GetApplicationBaseUrl(context.Request);
                        context.ProtocolMessage.RedirectUri = appBaseUrl;
                        context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                        
                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                        {
                            // we need to handle the redirect to the login page ourselves because redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateAuthenticationRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int)HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }
                        else if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            // we need to handle the redirect to the logout page ourselves because redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateLogoutRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int)HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    },

                    SecurityTokenValidated = context =>
                    {
                        var claimsPrincipal = new ClaimsPrincipal(context.AuthenticationTicket.Identity);

                        // TODO: load custom data and add them in the claims identity (e.g. load user id, roles etc.)

                        // store the identity in the HTTP request context so it can be persisted using the Cookie authentication middleware
                        context.OwinContext.Request.User = claimsPrincipal;

                        return Task.FromResult(0);
                    }
                }
            });
        }

        private static string GetApplicationBaseUrl(IOwinRequest contextRequest)
        {
            return contextRequest.Scheme + "://" + contextRequest.Host + contextRequest.PathBase;
        }
    }
}
