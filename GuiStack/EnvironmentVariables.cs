using System;
using System.IO;

namespace GuiStack
{
    /// <summary>
    /// A static class containing mappings for certain environment variables.
    /// </summary>
    public static class EnvironmentVariables
    {
        public static bool HasProtobufSupport { get; }
        public static string ProtobufCompilerExecutable { get; }

        public static bool S3ForcePathStyle { get; }

        static EnvironmentVariables()
        {
            ProtobufCompilerExecutable = Environment.GetEnvironmentVariable("PROTOC_EXECUTABLE");
            HasProtobufSupport = !string.IsNullOrWhiteSpace(ProtobufCompilerExecutable) && File.Exists(ProtobufCompilerExecutable);

            var forcePathStyleEnv = Environment.GetEnvironmentVariable("AWS_S3_FORCE_PATH_STYLE") ?? "false";

            if(bool.TryParse(forcePathStyleEnv, out var forcePathStyle))
                S3ForcePathStyle = forcePathStyle;
            else
                S3ForcePathStyle = forcePathStyleEnv == "1";
        }
    }
}
