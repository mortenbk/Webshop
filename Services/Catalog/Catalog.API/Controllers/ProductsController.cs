using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Infrastructure;
using Catalog.API.Models;
using WebShop.RabbitMQ;
using Catalog.API.Events;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly IEventBus _eventBus;

        public ProductsController(CatalogContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            var entityProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(prod => prod.Id == product.Id);

            var oldPrice = entityProduct.Price;
            var oldName = entityProduct.Name;

            bool invokeProductPriceChanged = oldPrice != product.Price;
            bool invokeProductNameChanged = oldName != product.Name;

            entityProduct = product;
            _context.Products.Update(entityProduct);

            try
            {
                await _context.SaveChangesAsync();

                if (invokeProductPriceChanged)
                    _eventBus.Publish(new ProductPriceChangedMessageEvent() { ProductId = id, Price = product.Price });
                if (invokeProductNameChanged)
                    _eventBus.Publish(new ProductNameChangedMessageEvent() { ProductId = id, ProductName = product.Name });

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _eventBus.Publish(new ProductRemovedMessageEvent() { ProductId = id });

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
