using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Services.Clients;
using Play.Inventory.Services.Data;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> repo;
        private readonly CatalogClient catalogClient;

        public ItemsController(IRepository<InventoryItem> repo, CatalogClient client)
        {
            this.catalogClient = client;
            this.repo = repo;

        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var catalogItems = await catalogClient.GetCatalogItemsAsync();

            var items = (await repo.GetAllAsync(item => item.UserId == userId))
            .Select(i => {
                var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == i.CatalogItemId);
                return i.AsDTO(catalogItem?.Name, catalogItem?.Description);
            });

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDTO grantItem)
        {
            if (grantItem == null || grantItem.CatalogItemId == Guid.Empty)
                return BadRequest();

            var item = await repo.GetAsync(item => item.CatalogItemId == grantItem.CatalogItemId && item.UserId == grantItem.UserId);

            if (item == null)
            {
                item = new InventoryItem()
                {
                    CatalogItemId = grantItem.CatalogItemId,
                    AcquiredDate = DateTimeOffset.UtcNow,
                    Quantity = grantItem.Quantity,
                    UserId = grantItem.UserId
                };

                await repo.CreateAsync(item);
            }
            else
            {
                item.Quantity += grantItem.Quantity;
                await repo.UpdateAsync(item);
            }

            return Ok(item);
        }
    }
}