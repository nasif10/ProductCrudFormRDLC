using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCrudFormRDLC.Models
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; }
        public Category Category { get; set; }
    }
}