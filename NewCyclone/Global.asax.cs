using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Web.SessionState;
using System.Web.Http;
using NewCyclone.Models;

namespace NewCyclone
{
    public class Global : HttpApplication
    {
        public Global() {
            AuthorizeRequest += new EventHandler(MvcApplication_AuthorizeRequest);
        }

        void MvcApplication_AuthorizeRequest(object sender, EventArgs e) {
            IIdentity id = Context.User.Identity;
            if (id.IsAuthenticated) {
                try
                {
                    SysUser u = new SysUser(id.Name);
                    var roles = u.role.Split(',');
                    Context.User = new GenericPrincipal(id, roles);
                }
                catch { }
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
    }
}