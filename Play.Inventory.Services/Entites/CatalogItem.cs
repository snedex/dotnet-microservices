using Play.Common;

namespace Play.Inventory.Services.Entites
{
    //Implementing only what makes sense for the Inventory service from the contract
    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}