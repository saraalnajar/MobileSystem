using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EntityFarmework
{
    [DataContract]
    public partial class Customers
    {
        [DataMember]
        [Required]
        [StringLength(80)]
        public string Name { get; set; }
        [DataMember]
        [Key]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [DataMember]
        [StringLength(10)]
        public string BirthDate { get; set; }
        [DataMember]
        public DateTime? ExpireDate { get; set; }
        [DataMember]
        public int PackageID { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [IgnoreDataMember]
        public virtual Packages Packages { get; set; }
    }
}
