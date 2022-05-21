using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Services;

namespace SendMassMailToLargePopulations
{
    [Table("DistributionGroupMember", Schema = "MassMail")]
    public class DistributionGroupMember : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Pid { get; set; }
        public string Uid { get; set; }
        public bool MassEmailAllowed { get; set; }
        public bool Student { get; set; }
        public bool Employee { get; set; }
        public bool IsActiveDirectory { get; set; }
        public bool Ddd { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        public DateTime? ImportedDate { get; set; }

    }
}
