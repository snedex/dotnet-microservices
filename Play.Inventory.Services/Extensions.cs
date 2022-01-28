using Play.Inventory.Services.Data;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services
{
    public static class Extensions
    {
        public static InventoryItemDTO AsDTO(this InventoryItem item)
        {
            return new InventoryItemDTO(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}