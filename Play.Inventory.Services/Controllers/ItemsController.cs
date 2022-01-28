using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Services.Data;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> repo;

        public ItemsController(IRepository<InventoryItem> repo)
        {
            this.repo = repo;

        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync(Guid userId)
        {
            if(userId == Guid.Empty)
                return BadRequest();

            var items = (await repo.GetAllAsync(item => item.UserId == userId)).Select(item => item.AsDTO());

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDTO grantItem)
        {
            if(grantItem == null || grantItem.CatalogItemId == Guid.Empty)
                return BadRequest();

            var item = await repo.GetAsync(item => item.CatalogItemId == grantItem.CatalogItemId && item.UserId == grantItem.UserId);

            if(item == null)
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