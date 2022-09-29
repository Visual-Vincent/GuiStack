/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

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
