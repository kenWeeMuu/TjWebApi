using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpDb.Entitys
{
    [Table("TjSku")]
    public partial class TjSku
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(80)]
        public string SkuCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string SkuType { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string CName { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(200)]
        public string EName { get; set; }
    }
}
