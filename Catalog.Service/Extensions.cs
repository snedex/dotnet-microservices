using Catalog.Service.Entities;
using Catalog.Service.Data;

namespace Catalog.Service
{
    public static class Extensions
    {
        //Automapper would be better here
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO(item.Id, item.Name, 
                item.Description, item.Price, item.CreatedDate);
        }
    }
}