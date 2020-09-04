# Azure Active Directory Authentication Sample for DotVVM

This is a sample app featuring the [Azure Active Directory authentication](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-scenarios) 
sample for [DotVVM](https://github.com/riganti/dotvvm).

## Prerequisites

1. Sign in to the _Azure Portal_ and open (or create) your _Azure Active Directory_ resource.

2. Open the _Overview_ tab and copy the _Directory ID_ value. Paste it in the `TenantId` application setting in the `web.config` file in OWIN or `appsettings.json` in ASP.NET Core.

3. Open the _App registrations_ tab and open (or create) the app registration.

4. Copy the _Application ID_ value and paste it in the `ClientId` application setting in the `web.config` or `appsettings.json` file.

5. Make sure that the URL on which the web app is running, is listed in the _Home Page_ field or in the _Reply URLs_ section. The URL must match including the HTTP/HTTPS and port.

6. In ASP.NET Core version, set the `PostLogoutRedirectUri` application setting to the application root URL.

Now the application should work with all accounts from the directory.

Note: If you decide to use `code` response type instead of default `id_token`, generate and set `ClientSecret` in the configuration file as well. The secret can be generated in _App registrations_ page in the _Authentication_ section.

### Multi-Tenancy Option

If you want to make the application multi-tenant (to allow the users to sign in even if they are from a different Azure Active Directory tenant), you need to make these changes:

1. In the _Properties_ tab of the _App Registration_, switch the _Multi-tenanted_ field to _Yes_. 

2. In the `web.config` or `appsettings.json`, change the `TenantId` to the value `common`.

## How to run the sample

1. [Open the GitHub repo in Visual Studio](git-client://clone/?repo=https%3A%2F%2Fgithub.com%2Friganti%2Fdotvvm-samples-azuread-auth)
or 
`git clone https://github.com/riganti/dotvvm-samples-dotvvm-samples-azuread-auth.git`

2. Open `AspNetCore/DotvvmAuthSample/DotvvmAuthSample.sln` (ASP.NET Core) or `Owin/DotvvmAuthSample/DotvvmAuthSample.sln` (.NET Framework with OWIN)

3. Right-click the `DotvvmAuthSample` project and select **View > View in Browser**

## What you can learn in the sample

* How to configure _Azure Active Directory_ authentication in a DotVVM application (see [Authentication](https://www.dotvvm.com/docs/tutorials/advanced-authentication-authorization/latest) page in the DotVVM Docs to find more info)

---

## Other resources

* [Gitter Chat](https://gitter.im/riganti/dotvvm)
* [DotVVM Official Website](https://www.dotvvm.com)
* [DotVVM Documentation](https://www.dotvvm.com/docs)
* [DotVVM GitHub](https://github.com/riganti/dotvvm)
* [Twitter @dotvvm](https://twitter.com/dotvvm)
* [Samples](https://www.dotvvm.com/samples)
