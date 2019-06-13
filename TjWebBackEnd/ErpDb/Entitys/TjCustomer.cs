using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpDb.Entitys
{
    [Table("TjCustomer")]
    public partial class TjCustomer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public int Id { get; set; }


        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(80)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [StringLength(50)]
        public string Region { get; set; }

        [StringLength(30)]
        public string Location { get; set; }

        [StringLength(50)]
        public string Industry { get; set; }

        public int Owner { get; set; }

        public int ServiceBy { get; set; }
    }
}
