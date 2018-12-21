using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using QnyWeb.Models;

[assembly: OwinStartupAttribute(typeof(QnyWeb.Startup))]
namespace QnyWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultRolesandUsers();
        }
        // In this method we will create default User roles and Admin user for login
        private void CreateDefaultRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User 
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website				

                var user = new ApplicationUser();
                user.UserName = "admin";
                user.LockoutEnabled = false;
                //user.Email = "admin@gmail.com";

                string userPassWord = "admin123";

                var chkUser = UserManager.Create(user, userPassWord);

                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Manager role 
            if (!roleManager.RoleExists("Sales"))
            {
                var role = new IdentityRole();
                role.Name = "Sales";
                roleManager.Create(role);

            }

            // creating Creating Employee role 
            if (!roleManager.RoleExists("Inputer"))
            {
                var role = new IdentityRole();
                role.Name = "Inputer";
                roleManager.Create(role);

            }
        }
    }
}
