using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DotvvmAuthSample.ViewModels
{
    [Authorize]
	public class MyProfileViewModel : DotvvmViewModelBase
    {

        public string Email => Context.HttpContext.User.Identity.Name;


        public async Task SignOut()
        {
            // sign out from both cookie and Open ID Connect
            await Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await Context.GetAuthentication().SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // tell DotVVM not to continue with processing this request
            Context.RedirectToRoute("Default");
        }
    }
}

