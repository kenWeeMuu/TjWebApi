using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpDb.Entitys
{
    [Table("TjCustomer")]
    public partial class TjCustomer
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
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

        public Guid? Owner { get; set; }

        public Guid? ServiceBy { get; set; }
    }
}
