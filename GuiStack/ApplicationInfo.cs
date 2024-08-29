/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GuiStack
{
    /// <summary>
    /// A static class containing information about the application.
    /// </summary>
    public static class ApplicationInfo
    {
        /// <summary>
        /// A record representing a license to a third-party resource.
        /// </summary>
        /// <param name="Name">The name of the third-party resource.</param>
        /// <param name="LicenseText">The text of the resource's copyright license.</param>
        public record class ThirdPartyLicense(string Name, string LicenseText);

#pragma warning disable IDE0001 // Simplify name
        internal static readonly Type ProgramClass = typeof(global::GuiStack.Program); // Deliberate choice to ensure we always refer to GuiStack
#pragma warning restore IDE0001

        /// <summary>
        /// The URL to the project that this application is part of.
        /// </summary>
        public const string ProjectUrl = "https://github.com/Visual-Vincent/GuiStack";

        /// <summary>
        /// The assembly that this application resides in.
        /// </summary>
        public static readonly Assembly MainAssembly = ProgramClass.Assembly;

        /// <summary>
        /// The copyright information of the application.
        /// </summary>
        public static readonly string Copyright = ProgramClass.Assembly.GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault()?.Copyright;

        /// <summary>
        /// The application description.
        /// </summary>
        public static readonly string Description = 
            MainAssembly.GetCustomAttributes<AssemblyDescriptionAttribute>().FirstOrDefault()?.Description ??
            MainAssembly.GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault()?.Title;

        /// <summary>
        /// The copyright license of the application.
        /// </summary>
        public static readonly string License = GetStringResource("LICENSE.txt");

        /// <summary>
        /// The version number of the application.
        /// </summary>
        public static readonly Version Version = ProgramClass.Assembly.GetName().Version;

        /// <summary>
        /// The collection of copyright licenses of third-party resources used by the application.
        /// </summary>
        public static readonly IReadOnlyCollection<ThirdPartyLicense> ThirdPartyLicenses = ((Func<IReadOnlyCollection<ThirdPartyLicense>>)(() => {
            var licenses = new List<ThirdPartyLicense>();
            var resources = MainAssembly.GetManifestResourceNames();

            foreach(var resource in resources)
            {
                if(!resource.StartsWith($"{nameof(GuiStack)}.Licenses", StringComparison.OrdinalIgnoreCase))
                    continue;

                var resourceName = resource.Remove(0, nameof(GuiStack).Length + 1);
                var thirdPartyName = resource.Remove(0, $"{nameof(GuiStack)}.Licenses".Length + 1);
                var license = GetStringResource(resourceName);

                if(license == null)
                    continue;

                var extension = Path.GetExtension(thirdPartyName);

                switch(extension.ToLower())
                {
                    case ".txt":
                    case ".md":
                        thirdPartyName = thirdPartyName.Remove(thirdPartyName.Length - extension.Length);
                        break;
                }

                licenses.Add(new ThirdPartyLicense(thirdPartyName, license));
            }

            return licenses.OrderBy(l => l.Name).ToArray();
        })).Invoke();

        /// <summary>
        /// Returns an embedded resource string, or null if none was found.
        /// </summary>
        /// <param name="resourceName">The name of the resource to fetch (case sensitive).</param>
        internal static string GetStringResource(string resourceName)
        {
            using var resourceStream = MainAssembly.GetManifestResourceStream($"{nameof(GuiStack)}.{resourceName}");

            if(resourceStream == null)
                return null;

            using var streamReader = new StreamReader(resourceStream);

            return streamReader.ReadToEnd();
        }
    }
}
