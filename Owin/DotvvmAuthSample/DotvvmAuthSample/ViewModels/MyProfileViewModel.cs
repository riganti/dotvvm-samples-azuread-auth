using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Text;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace DotvvmAuthSample.ViewModels
{
    [Authorize]
	public class MyProfileViewModel : DotvvmViewModelBase
    {

        public string Email => Context.HttpContext.User.Identity.Name;


        public void SignOut()
        {
            // sign out from both cookie and Open ID Connect
            Context.GetAuthentication().SignOut(CookieAuthenticationDefaults.AuthenticationType, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            
            // tell DotVVM not to continue with processing this request
            Context.RedirectToRoute("Default");
        }
    }
}

