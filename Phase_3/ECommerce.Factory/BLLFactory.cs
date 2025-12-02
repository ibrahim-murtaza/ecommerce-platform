using ECommerce.BLL.Interfaces;
// You will need to add references to the specific BLL implementations 
// once your teammates create them (ECommerce.BLL.LinqImplementation, etc.)

namespace ECommerce.Factory
{
    public enum BLLType
    {
        LINQ,
        StoredProcedure
    }

    public static class BLLFactory
    {
        // Currently returning NULL because teammates haven't built the classes yet.
        // This acts as the placeholder skeleton.
        
        public static IProductService GetProductService(BLLType type)
        {
            if (type == BLLType.LINQ)
            {
                // return new ProductServiceLINQ(); 
                return null; 
            }
            else
            {
                // return new ProductServiceSP();
                return null;
            }
        }

        // Repeat for User, Order, Cart services...
    }
}