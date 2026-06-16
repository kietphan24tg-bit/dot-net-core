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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var stocks = (await _context.Stocks.ToListAsync(cancellationToken))
                .Select(s => s.ToStockDto());
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var stock = await _context.Stocks.FindAsync([id], cancellationToken);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock.ToStockDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto, CancellationToken cancellationToken)
        {
            var stock = stockDto.ToStockFromCreateDTO();
            await _context.Stocks.AddAsync(stock, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(stock.ToStockDto());
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateStockDto stockDto, CancellationToken cancellationToken)
        {
            var stock = await _context.Stocks.FindAsync([id], cancellationToken);
            if (stock == null)
            {
                return NotFound();
            }
            stock.Symbol = stockDto.Symbol;
            stock.CompanyName = stockDto.CompanyName;
            stock.Purchase = stockDto.Purchase;
            stock.LastDiv = stockDto.LastDiv;
            stock.Industry = stockDto.Industry;
            stock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(stock.ToStockDto());
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Stock updatedStock, CancellationToken cancellationToken)
        {
            var stock = await _context.Stocks.FindAsync([id], cancellationToken);
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

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(stock.ToStockDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var stock = await _context.Stocks.FindAsync([id], cancellationToken);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(stock.ToStockDto());
        }
    }
}
