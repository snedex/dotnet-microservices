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
        private readonly IRepository<InventoryItem> inventoryRepo;
        private readonly IRepository<CatalogItem> itemRepo;

        public ItemsController(IRepository<InventoryItem> inventoryRepo, IRepository<CatalogItem> itemRepo)
        {
            this.inventoryRepo = inventoryRepo;
            this.itemRepo = itemRepo;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ActionResult<IEnumerable<InventoryItemDTO>>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();


            var inventoryItems = await inventoryRepo.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItems.Select(i => i.CatalogItemId);

            var catalogItems = await itemRepo.GetAllAsync(i => itemIds.Contains(i.Id));

            var dtos = inventoryItems.Select(i => {
                var catalogItem = catalogItems.SingleOrDefault(ci => ci.Id == i.CatalogItemId);
                return i.AsDTO(catalogItem?.Name, catalogItem?.Description);
            });

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDTO grantItem)
        {
            if (grantItem == null || grantItem.CatalogItemId == Guid.Empty)
                return BadRequest();

            var item = await inventoryRepo.GetAsync(item => item.CatalogItemId == grantItem.CatalogItemId && item.UserId == grantItem.UserId);

            if (item == null)
            {
                item = new InventoryItem()
                {
                    CatalogItemId = grantItem.CatalogItemId,
                    AcquiredDate = DateTimeOffset.UtcNow,
                    Quantity = grantItem.Quantity,
                    UserId = grantItem.UserId
                };

                await inventoryRepo.CreateAsync(item);
            }
            else
            {
                item.Quantity += grantItem.Quantity;
                await inventoryRepo.UpdateAsync(item);
            }

            return Ok(item);
        }
    }
}