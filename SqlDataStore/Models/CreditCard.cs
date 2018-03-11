using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDataStore.Models
{
    public class CreditCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public double AnnualFee { get; set; }
        public double PurchaseApr { get; set; }
        public string Link { get; set; }
    }
}
