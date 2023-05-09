using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EntityFarmework
{[DataContract]
    public partial class Packages
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Packages()
        {
            Customers = new HashSet<Customers>();
        }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [DataMember]
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        [DataMember]
        public int price { get; set; }
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customers> Customers { get; set; }
    }
}
