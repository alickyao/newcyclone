using System.Web.Mvc;

namespace NewCyclone.Areas.WxWeb
{
    public class WxWebAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "WxWeb";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "WxWeb_default",
                "wx/{controller}/{action}/{id}",
                new { controller = "Response", action = "response", id = UrlParameter.Optional }
            );
        }
    }
}