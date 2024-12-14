using DotVVM.Framework.Hosting;
using DotvvmAuthSample;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder();

builder.Services.AddDotVVM<DotvvmStartup>();

builder.Services.AddAuthentication(sharedOptions =>
	{
		sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
	})
	.AddMicrosoftIdentityWebApp(options =>
	{
		options.Instance = builder.Configuration["AzureAd:Instance"];
		options.ClientId = builder.Configuration["AzureAD:ClientId"];
		options.TenantId = builder.Configuration["AzureAd:Tenant"];
		options.SignedOutRedirectUri = builder.Configuration["AzureAd:PostLogoutRedirectUri"];

		// Uncomment the following section if you need "code" response type
		// options.ResponseType = "code";
		// options.ClientSecret = "Configuration["AzureAd:ClientSecret"];

		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = (builder.Configuration["AzureAd:Tenant"] != "common")
		};

		options.Events = new OpenIdConnectEvents
		{
			OnRemoteFailure = context =>
			{
				context.HandleResponse();
				return DotvvmAuthenticationHelper.ApplyRedirectResponse(context.HttpContext, "/error");
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
				return DotvvmAuthenticationHelper.ApplyRedirectResponse(context.HttpContext, message.BuildRedirectUrl());
			},

			OnRedirectToIdentityProviderForSignOut = context =>
			{
				var message = context.ProtocolMessage;
				if (!string.IsNullOrEmpty(message.State))
				{
					context.Properties.Items[OpenIdConnectDefaults.UserstatePropertiesKey] = message.State;
				}
				context.Properties.Items.Add(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey, message.RedirectUri);
				message.State = context.Options.StateDataFormat.Protect(context.Properties);
				return DotvvmAuthenticationHelper.ApplyRedirectResponse(context.HttpContext, message.BuildRedirectUrl());
			}
		};
	});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseDotVVM<DotvvmStartup>();
app.UseDotvvmHotReload();

app.Run();