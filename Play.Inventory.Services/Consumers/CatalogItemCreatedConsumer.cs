using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            //Check we haven't seen this message already
            var item = await repository.GetAsync(message.ItemId);

            if(item != null)
                return;  

            item = new CatalogItem() {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };   

            await repository.CreateAsync(item);    
        }
    }
}