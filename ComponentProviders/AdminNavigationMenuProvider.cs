using Penguin.Cms.Modules.Core.ComponentProviders;
using Penguin.Cms.Modules.Core.Navigation;
using Penguin.Cms.Modules.Forms.Constants.Strings;
using Penguin.Navigation.Abstractions;
using Penguin.Security.Abstractions;
using Penguin.Security.Abstractions.Interfaces;
using System.Collections.Generic;
using SecurityRoles = Penguin.Security.Abstractions.Constants.RoleNames;

namespace Penguin.Cms.Modules.Forms.ComponentProviders
{
    public class AdminNavigationMenuProvider : NavigationMenuProvider
    {
        public override INavigationMenu GenerateMenuTree()
        {
            return new NavigationMenu()
            {
                Name = "Admin",
                Text = "Admin",
                Children = new List<INavigationMenu>() {
                    new NavigationMenu()
                    {
                        Text = "Forms",
                        Name = "FormAdmin",
                        Href = "/Admin/Form/Index",
                        Permissions = new List<ISecurityGroupPermission>()
                        {
                            this.CreatePermission(RoleNames.ContentManager, PermissionTypes.Read),
                            this.CreatePermission(SecurityRoles.SysAdmin, PermissionTypes.Read | PermissionTypes.Write)
                        },
                        Children = new List<INavigationMenu>()
                        {
                            new NavigationMenu()
                            {
                                Text = "Submissions",
                                Name = "SearchForms",
                                Icon = "search",
                                Href = "/Admin/Form/Submissions",
                                Permissions = new List<ISecurityGroupPermission>()
                                {
                                    this.CreatePermission(RoleNames.ContentManager, PermissionTypes.Read),
                                    this.CreatePermission(SecurityRoles.SysAdmin, PermissionTypes.Read | PermissionTypes.Write)
                                }
                            },
                            new NavigationMenu()
                            {
                                Text = "Author",
                                Name = "AuthorForm",
                                Icon = "add_box",
                                Href = "/Admin/Form/Create",
                                Permissions = new List<ISecurityGroupPermission>()
                                {
                                    this.CreatePermission(RoleNames.ContentManager, PermissionTypes.Read),
                                    this.CreatePermission(SecurityRoles.SysAdmin, PermissionTypes.Read | PermissionTypes.Write)
                                }
                            },
                            new NavigationMenu()
                            {
                                Text = "List",
                                Name = "ListForms",
                                Icon = "list",
                                Href = "/Admin/Form/List",
                                Permissions = new List<ISecurityGroupPermission>()
                                {
                                    this.CreatePermission(RoleNames.ContentManager, PermissionTypes.Read),
                                    this.CreatePermission(SecurityRoles.SysAdmin, PermissionTypes.Read | PermissionTypes.Write)
                                }
                            }
                        }
                    },
                    }
            };
        }
    }
}