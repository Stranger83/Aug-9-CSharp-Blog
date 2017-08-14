using Aug_9_CSharp_Blog.Migrations;
using Aug_9_CSharp_Blog.Models;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(Aug_9_CSharp_Blog.Startup))]
namespace Aug_9_CSharp_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BlogDbContext, Configuration>());
            ConfigureAuth(app);
        }
    }
}
