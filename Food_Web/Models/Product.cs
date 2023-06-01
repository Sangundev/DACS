namespace Food_Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            extrafoods = new HashSet<extrafood>();
            Garelies = new HashSet<Garely>();
            Order_detail = new HashSet<Order_detail>();
        }

        public int Productid { get; set; }

        [StringLength(100)]
        public string Productname { get; set; }

        public int? price { get; set; }

        [StringLength(100)]
        public string image { get; set; }

        [StringLength(100)]
        public string discription { get; set; }

        public int Categoryid { get; set; }

        [StringLength(100)]
        public string sortdiscription { get; set; }

        public int? chef_id { get; set; }

        [StringLength(128)]
        public string Userid { get; set; }

        public bool status { get; set; }

        public bool is_hot { get; set; }

        public int? DiscountedPrice { get; set; }

        public int? DiscountPercent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItems { get; set; }

        public virtual Category Category { get; set; }

        public virtual chef chef { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<extrafood> extrafoods { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Garely> Garelies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_detail> Order_detail { get; set; }
    }
}
