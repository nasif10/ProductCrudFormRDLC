using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCrudFormRDLC.Models
{
    [Serializable]
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; }
    }
}