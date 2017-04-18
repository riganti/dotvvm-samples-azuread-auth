using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace DotvvmAuthSample.ViewModels
{
    public class DefaultViewModel : DotvvmViewModelBase
    {

        public void SignIn()
        {
            // redirect to Azure AD login page
            Context.GetAuthentication().Challenge(new AuthenticationProperties()
                {
                    RedirectUri = "/myProfile"
                },
                OpenIdConnectAuthenticationDefaults.AuthenticationType
            );

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
