# Azure Active Directory Authentication Sample for DotVVM

This is a sample app featuring the [Azure Active Directory authentication](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-scenarios) 
sample for [DotVVM](https://github.com/riganti/dotvvm).

### Instructions

1. Sign in to the Azure Portal and open (or create) your Azure Active Directory resource.

2. Open the Overview tab and copy the _Directory ID_ value. Paste it in the `TenantId` application setting in the `web.config` file in OWIN or `appsettings.json` in ASP.NET Core.

3. Open the App registrations tab and open (or create) the app registration.

4. Copy the _Application ID_ value and paste it in the `ClientId` application setting in the `web.config` file.

![IDs section](/screenshots/ids.png?raw=true "IDs section")

5. Make sure that the URL on which the web app is running, is listed in the _Home page URL_ field in the tab Branding and redirect URIs set in  _Redirect URIs_ field in Authentication section. The URLs must match including the HTTP/HTTPS and port.

![Redirect URIs section](/screenshots/redirects.png?raw=true "Redirect URIs section")

6. In ASP.NET Core version, set the `PostLogoutRedirectUri` application setting to the application root URL.

Now the application should work with all accounts from the directory.

Note: If you decide to use `code` response type instead of default `id_token`, generate and set `ClientSecret` in the configuration file as well. The secret can be generated in App registrations, section Authentication.


### Multi-Tenancy

If you want to make the application multi-tenant (to allow the users to sign in even if they are from a different Azure Active Directory tenant), you need to make these changes:

1. In the _Properties_ tab of the _App Registration_, switch the _Multi-tenanted_ field to _Yes_. 

2. In the `web.config` or `appsettings.json`, change the `TenantId` to the value `common`.

