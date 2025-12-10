using ECommerce.Factory;

namespace ECommerce.UI.Helpers
{
    public static class BLLManager
    {
        public static BLLType CurrentBLLType { get; set; } = BLLType.LINQ;

        public static void ToggleBLLType()
        {
            CurrentBLLType = CurrentBLLType == BLLType.LINQ 
                ? BLLType.StoredProcedure 
                : BLLType.LINQ;
        }

        public static string GetCurrentBLLTypeName()
        {
            return CurrentBLLType == BLLType.LINQ ? "LINQ" : "Stored Procedure";
        }
    }
}
