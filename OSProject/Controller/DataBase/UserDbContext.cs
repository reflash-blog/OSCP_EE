using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OSProject.Model.Structures;

namespace OSProject.Controller.DataBase
{
    public class UserDbContext : DbContext
    {
        public UserDbContext()
        {
            // Turn off the Migrations, (NOT a code first Db)
            Database.SetInitializer<UserDbContext>(null);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Database does not pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    } 
}
