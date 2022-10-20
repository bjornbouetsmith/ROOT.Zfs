using System;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Contains easy 'Size' structs for valid block sizes
    /// </summary>
    public class BlockSizes
    {
        public static readonly Size Minimum = new(512);
        public static readonly Size K1 = new(1024);
        public static readonly Size K2 = new(1024 << 1);
        public static readonly Size K4 = new(1024 << 2);
        public static readonly Size K8 = new(1024 << 3);
        public static readonly Size K16 = new(1024 << 4);
        public static readonly Size K32 = new(1024 << 5);
        public static readonly Size K64 = new(1024 << 6);
        public static readonly Size K128 = new(1024 << 7);
        public static readonly Size K256 = new(1024 << 8);
        public static readonly Size K512 = new(1024 << 9);
        public static readonly Size M1 = new(1024 << 10);

        /// <summary>
        /// Validates if the block size if a valid block size and returns true/false to indicate that
        /// </summary>
        /// <param name="size">The size to validate</param>
        /// <param name="errorMessage">The error message if the size is not a valid block size</param>
        /// <returns>true if the size is a valid block size;false otherwise</returns>
        public static bool IsValid(Size size, out string errorMessage)
        {
            if (size.Bytes < 512 || size.Bytes > 1024 << 10)
            {
                errorMessage = "block size must between 512 & 1048576 (1M)";
                return false;
            }

            if ((size.Bytes & (size.Bytes - 1)) != 0)
            {
                errorMessage = "block size must between 512 & 1048576 (1M)";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }
}
