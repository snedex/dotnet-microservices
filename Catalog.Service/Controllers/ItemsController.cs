using Microsoft.AspNetCore.Mvc;
using Catalog.Service.Data;

namespace Catalog.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDTO> items  = new() {
        new ItemDTO(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDTO(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new ItemDTO(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
    };

    [HttpGet]
    public IEnumerable<ItemDTO> Get()
    {
        return items;
    }

    [HttpGet("{id}")]
    public ItemDTO GetById(Guid id)
    {
        return items.Where(i => i.Id == id).SingleOrDefault();
    }

    [HttpPost]
    public ActionResult<ItemDTO> Post(CreateItemDTO createItem)
    {
        var item = new ItemDTO(Guid.NewGuid(), createItem.Name, createItem.Description, createItem.Price, DateTimeOffset.UtcNow);
        items.Add(item);

        //Includes the location in the response headers
        return CreatedAtAction(nameof(GetById), new { item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDTO updateItem)
    {
        var item = items.Where(i => i.Id == id).SingleOrDefault();

        if(item == null)
            return NotFound();

        var updated = item with {
            Name = updateItem.Name,
            Description = updateItem.Description,
            Price = updateItem.Price
        };

        var index = items.FindIndex(item => item.Id == id);

        if(index == -1)
            return NotFound();

        items[index] = updated;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var index = items.FindIndex(item => item.Id == id);

        if(index == -1)
            return NotFound();

        items.RemoveAt(index);

        return NoContent();
    }

}
