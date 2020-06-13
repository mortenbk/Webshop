using Catalog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API
{
    public class UpdateProductDto : CreateProductDto
    {

        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    }
}
