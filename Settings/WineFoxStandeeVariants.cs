namespace STS2_WineFox.Settings
{
    public static class WineFoxStandeeVariants
    {
        public const string New = "new";
        public const string Old = "old";

        public static string Normalize(string? variant)
        {
            return variant == Old ? Old : New;
        }

        public static bool IsOld(string? variant)
        {
            return Normalize(variant) == Old;
        }

        public static string SkeletonPath(string? variant)
        {
            return IsOld(variant) ? Const.Paths.StandeeOldSkeleton : Const.Paths.StandeeNewSkeleton;
        }
    }
}
