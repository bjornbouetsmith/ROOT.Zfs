namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Contains easy 'Size' structs for valid block sizes
    /// </summary>
    public static class BlockSizes
    {
        /// <summary>
        /// Minimum allowed block size.
        /// Not recommended, should use at least 8K according to warning message
        /// when trying to create a volume with less
        /// </summary>
        public static readonly Size Minimum = "512B";
        
        /// <summary>
        /// 1K
        /// </summary>
        public static readonly Size K1 = "1K";
        
        /// <summary>
        /// 2K
        /// </summary>
        public static readonly Size K2 = "2K";
        
        /// <summary>
        /// 4K
        /// </summary>
        public static readonly Size K4 = "4K";
        
        /// <summary>
        /// 8K
        /// </summary>
        public static readonly Size K8 ="8K";
        
        /// <summary>
        /// 16K
        /// </summary>
        public static readonly Size K16 = "16K";
        
        /// <summary>
        /// 32K
        /// </summary>
        public static readonly Size K32 = "32K";
        
        /// <summary>
        /// 64K
        /// </summary>
        public static readonly Size K64 = "64K";
        
        /// <summary>
        /// 128K
        /// </summary>
        public static readonly Size K128 = "128K";
        
        /// <summary>
        /// 256K
        /// </summary>
        public static readonly Size K256 = "256K";
        
        /// <summary>
        /// 512K
        /// </summary>
        public static readonly Size K512 = "512K";
        
        /// <summary>
        /// 1M - highest allowed blocksize
        /// </summary>
        public static readonly Size M1 = "1M";

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
