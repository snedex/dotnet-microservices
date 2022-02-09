using Catalog.Service.Data;
using Catalog.Service.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Catalog.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private const string AdminRole = "Admin";
    private readonly IRepository<Item> itemRepo;

    private readonly IPublishEndpoint publishEndpoint;

    public ItemsController(IRepository<Item> repo, IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
        this.itemRepo = repo;
    }

    [HttpGet]
    [Authorize(Policies.Read)]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAsync()
    {
        var items = (await itemRepo.GetAllAsync())
                .Select(i => i.AsDTO());

        return Ok(items);
    }

    [HttpGet("{id}")]
    [Authorize(Policies.Read)]
    public async Task<ItemDTO> GetByIdAsync(Guid id)
    {
        var item = await itemRepo.GetAsync(id);
        return item?.AsDTO();
    }

    [HttpPost]
    [Authorize(Policies.Write)]
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

        await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

        //Includes the location in the response headers
        return CreatedAtAction("GetById", new { item.Id }, item);
    }

    [HttpPut("{id}")]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDTO updateItem)
    {
        var item = await itemRepo.GetAsync(id);

        if (item == null)
            return NotFound();

        item.Name = updateItem.Name;
        item.Description = updateItem.Description;
        item.Price = updateItem.Price;

        await itemRepo.UpdateAsync(item);

        await publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));


        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await itemRepo.GetAsync(id);

        if (item == null)
            return NotFound();

        await itemRepo.RemoveAsync(item.Id);

        await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));

        return NoContent();
    }

}
