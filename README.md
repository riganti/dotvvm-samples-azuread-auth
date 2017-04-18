# Azure Active Directory Authentication Sample for DotVVM

This is a sample app featuring the [Azure Active Directory authentication](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-scenarios) 
sample for [DotVVM](https://github.com/riganti/dotvvm).

### Instructions

1. Sign in to the Azure Portal and open (or create) your Azure Active Directory resource.

2. Open the Properties tab and copy the _Directory ID_ value. Paste it in the `ida:TenantId` application setting in the `web.config` file.

3. Open the App registrations tab and open (or create) the app registration.

4. Copy the _Application ID_ value and paste it in the `ida:ClientId` application setting in the `web.config` file.

5. Make sure that the URL on which the web app is running, is listed in the _Home Page_ field or in the _Reply URLs_ section. The URL must match including the HTTP/HTTPS and port.

Now the application should work with all accounts from the directory.

### Multi-Tenancy

If you want to make the application multi-tenant (to allow the users to sign in even if they are from a different Azure Active Directory tenant), you need to make these changes:

1. In the _Properties_ tab of the _App Registration_, switch the _Multi-tenanted_ field to _Yes_. 

2. In the `web.config`, change the `ida:TenantId` to the value `common`.

<br />

We are still working on the ASP.NET Core version of this sample. The principles are the same, however there are some issues with the Open ID Connect library.
