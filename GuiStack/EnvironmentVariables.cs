using System;

namespace GuiStack
{
    /// <summary>
    /// A static class containing mappings for certain environment variables.
    /// </summary>
    public static class EnvironmentVariables
    {
        public static bool S3ForcePathStyle { get; }

        static EnvironmentVariables()
        {
            var forcePathStyleEnv = Environment.GetEnvironmentVariable("AWS_S3_FORCE_PATH_STYLE") ?? "false";

            if(bool.TryParse(forcePathStyleEnv, out var forcePathStyle))
                S3ForcePathStyle = forcePathStyle;
            else
                S3ForcePathStyle = forcePathStyleEnv == "1";
        }
    }
}
