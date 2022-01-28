using Play.Inventory.Services.Data;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services
{
    public static class Extensions
    {
        public static InventoryItemDTO AsDTO(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDTO(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
        }
    }
}