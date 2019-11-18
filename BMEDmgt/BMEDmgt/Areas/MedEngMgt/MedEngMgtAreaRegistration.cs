using System.Web.Mvc;

namespace BMEDmgt.Areas.MedEngMgt
{
    public class MedEngMgtAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MedEngMgt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MedEngMgt_default",
                "MedEngMgt/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "BMEDmgt.Areas.MedEngMgt.Controllers" }
            );
        }
    }
}