namespace Play.Inventory.Services.Data
{
    public record GrantItemsDTO (Guid UserId, Guid CatalogItemId, int Quantity);

    public record InventoryItemDTO (Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);


    //for fetching more information from the Catalog service
    public record CatalogItemDTO(Guid Id, string Name, string Description);

}