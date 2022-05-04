using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Penguin.Web.Abstractions.Interfaces;

namespace Penguin.Cms.Modules.Forms.Areas.Admin
{
    public class RouteConfig : IRouteConfig
    {
        public void RegisterRoutes(IRouteBuilder routes)
        {
            _ = routes.MapRoute(
                "Admin_Submissions",
                "Admin/Form/Submissions/{Name?}",
                new { area = "admin", controller = "Form", action = "Submissions" }

            );

            _ = routes.MapRoute(
                "Form",
                "Form/{Name}",
                new { area = "", controller = "Form", action = "ViewByName" }

            );

            _ = routes.MapRoute(
                 name: "Form_Submit",
                 template: "Form/Actions/Submit",
                 defaults: new { controller = "Form", action = "Submit" }
             );

            _ = routes.MapRoute(
                name: "Form_Id_View",
                template: "Form/View/{Id}",
                defaults: new { controller = "Form", action = "ViewById" }
            );
        }
    }
}