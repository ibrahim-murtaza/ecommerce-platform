using ECommerce.BLL.Interfaces;
using ECommerce.BLL.LinqImplementation;
using ECommerce.BLL.SPImplementation;
using ECommerce.DAL;

namespace ECommerce.Factory
{
    public enum BLLType
    {
        LINQ,
        StoredProcedure
    }

    public static class BLLFactory
    {
        public static IProductService GetProductService(BLLType type)
        {
            var context = new ECommerceContext();

            if (type == BLLType.LINQ)
            {
                return new ProductServiceLINQ(context);
            }
            else
            {
                return new ProductServiceSP(context);
            }
        }

        public static IUserService GetUserService(BLLType type)
        {
            var context = new ECommerceContext();

            if (type == BLLType.LINQ)
            {
                return new UserServiceLINQ(context);
            }
            else
            {
                return new UserServiceSP(context);
            }
        }

        public static IOrderService GetOrderService(BLLType type)
        {
            var context = new ECommerceContext();

            if (type == BLLType.LINQ)
            {
                return new OrderServiceLINQ(context);
            }
            else
            {
                return new OrderServiceSP(context);
            }
        }

        public static ICartService GetCartService(BLLType type)
        {
            var context = new ECommerceContext();

            if (type == BLLType.LINQ)
            {
                return new CartServiceLINQ(context);
            }
            else
            {
                return new CartServiceSP(context);
            }
        }

        public static ICategoryService GetCategoryService(BLLType type)
        {
            var context = new ECommerceContext();

            if (type == BLLType.LINQ)
            {
                return new CategoryServiceLINQ(context);
            }
            else
            {
                return new CategoryServiceSP(context);
            }
        }
    }
}