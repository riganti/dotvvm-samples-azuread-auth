using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace DotvvmAuthSample.ViewModels
{
    public class DefaultViewModel : DotvvmViewModelBase
    {

        public async Task SignIn()
        {
            // redirect to Azure AD login page
            await Context.GetAuthentication().ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, 
                new AuthenticationProperties()
                { 
                    RedirectUri = "/myProfile"
                });

            // tell DotVVM not to continue with processing this request
            Context.InterruptRequest();
        }

        [Authorize]
        public void ActionThatRequiresSignIn()
        {
            // this should redirect automatically to the login page
            Context.RedirectToUrl("/myProfile");
        }
    }
}
