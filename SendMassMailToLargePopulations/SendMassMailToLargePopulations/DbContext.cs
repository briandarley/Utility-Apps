using System.Data.Entity;

namespace SendMassMailToLargePopulations
{
    public class DbContext:System.Data.Entity.DbContext
    {
        public DbSet<DistributionGroupMember> DistributionGroupMembers { get; set; }
        public DbContext():base("Data Source=itsgensql2.ad.unc.edu,2433;Initial Catalog=ITS_WSAPP_MASSMAIL;Integrated Security=True;MultipleActiveResultSets=true;Application Name=DAL.MassMail.API")
        {
            
        }
    }
}
