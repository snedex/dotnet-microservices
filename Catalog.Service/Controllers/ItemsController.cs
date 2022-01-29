using Catalog.Service.Data;
using Catalog.Service.Entities;
using Play.Common;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemRepo;

    public ItemsController(IRepository<Item> repo)
    {
        this.itemRepo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAsync()
    {
        var items = (await itemRepo.GetAllAsync())
                .Select(i => i.AsDTO());

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ItemDTO> GetByIdAsync(Guid id)
    {
        var item = await itemRepo.GetAsync(id);
        return item?.AsDTO();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDTO>> PostAsync(CreateItemDTO createItem)
    {
        var item = new Item()
        {
            Id = Guid.NewGuid(),
            Name = createItem.Name,
            Description = createItem.Description,
            Price = createItem.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await itemRepo.CreateAsync(item);

        //Includes the location in the response headers
        return CreatedAtAction("GetById", new { item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDTO updateItem)
    {
        var item = await itemRepo.GetAsync(id);

        if (item == null)
            return NotFound();

        item.Name = updateItem.Name;
        item.Description = updateItem.Description;
        item.Price = updateItem.Price;

        await itemRepo.UpdateAsync(item);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await itemRepo.GetAsync(id);

        if (item == null)
            return NotFound();

        await itemRepo.RemoveAsync(item.Id);

        return NoContent();
    }

}
