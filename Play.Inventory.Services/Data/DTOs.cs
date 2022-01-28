namespace Play.Inventory.Services.Data
{
    public record GrantItemsDTO (Guid UserId, Guid CatalogItemId, int Quantity);

    public record InventoryItemDTO (Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);

}