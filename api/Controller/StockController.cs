using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Mappers;
using api.Models;
using api.Dtos.Stock;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = (await _context.Stocks.ToListAsync())
                .Select(s => s.ToStockDto());
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock.ToStockDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {            var stock = stockDto.ToStockFromCreateDTO();
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return Ok(stock.ToStockDto());
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Stock updatedStock)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            stock.Symbol = updatedStock.Symbol;
            stock.CompanyName = updatedStock.CompanyName;
            stock.Purchase = updatedStock.Purchase;
            stock.LastDiv = updatedStock.LastDiv;
            stock.Industry = updatedStock.Industry;
            stock.MarketCap = updatedStock.MarketCap;

            await _context.SaveChangesAsync();
            return Ok(stock.ToStockDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return Ok(stock.ToStockDto());
        }
    }
}
