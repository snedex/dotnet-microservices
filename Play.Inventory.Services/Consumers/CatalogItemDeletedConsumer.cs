using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Services.Entites;

namespace Play.Inventory.Services.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(message.ItemId);

            //Do nothing if we don't have it
            if (item == null)
                return;
            
            await repository.RemoveAsync(item.Id);
        }
    }
}