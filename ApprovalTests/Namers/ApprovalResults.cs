using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ApprovalUtilities.Utilities;

namespace ApprovalTests.Namers
{
    public class ApprovalResults
    {
        public static IDisposable UniqueForDotNetVersion()
        {
            return NamerFactory.AsEnvironmentSpecificTest(GetDotNetVersion);
        }

        public static string GetDotNetVersion()
        {
            return "Net_v" + Environment.Version;
        }

        public static IDisposable UniqueForRuntime(bool throwOnError = true)
        {
            return NamerFactory.AsEnvironmentSpecificTest(() => GetDotNetRuntime(throwOnError));
        }

        public static string GetDotNetRuntime(bool throwOnError)
        {
            var frameworkDescription = RuntimeInformation.FrameworkDescription;
            if (frameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
            {
                var version = Version.Parse(frameworkDescription.Replace(".NET Framework ", ""));
                return $"Net_{version.Major}.{version.Minor}";
            }

            if (frameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase))
            {
                var version = GetNetCoreVersion();
                return $"NetCore_{version.Major}.{version.Minor}";
            }

            if (throwOnError)
            {
                throw new NotImplementedException($@"Your current framework is not properly handled by ApprovalTests
Framework: {frameworkDescription}.
To suppress this error and make the test pass using the full FrameworkDescription use:
using (Namers.ApprovalResults.UniqueForRuntime(throwOnError: true)){{
    //The code being tested
}}
To help ApprovalTest please submit a new issue using the following link:
https://github.com/approvals/ApprovalTests.Net/issues/new?title=Unknown%3A+%27Runtime%27&body={frameworkDescription}
");
            }

            return frameworkDescription;
        }

        static Version GetNetCoreVersion()
        {
            var assembly = typeof(GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
            {
                var versionString = assemblyPath[netCoreAppIndex + 1];
                if (Version.TryParse(versionString, out var netCoreVersion))
                {
                    return netCoreVersion;
                }
            }
            return null;
        }

        public static IDisposable UniqueForMachineName()
        {
            return NamerFactory.AsEnvironmentSpecificTest(GetMachineName);
        }

        public static string GetMachineName()
        {
            return "ForMachine." + Environment.MachineName;
        }

        public static string GetOsName()
        {
            var name = TransformEasyOsName(OsUtils.GetFullOsNameFromWmi());
            return name.Trim().Replace(' ', '_');
        }

        public static string GetFullOsName()
        {
            var name = OsUtils.GetFullOsNameFromWmi();
            return name.Trim().Replace(' ', '_');
        }

        public static string TransformEasyOsName(string captionName)
        {
            string[] known = {"XP", "2000", "Vista", "7", "8", "Server 2003", "Server 2008", "Server 2012"};
            var matched = known.FirstOrDefault(s => captionName.StartsWith("Microsoft Windows " + s));
            if (matched != null)
            {
                return "Windows " + matched;
            }

            return captionName;
        }

        public static IDisposable UniqueForOs()
        {
            return NamerFactory.AsEnvironmentSpecificTest(GetOsName);
        }

        public static string GetUserName()
        {
            return "ForUser." + Environment.UserName;
        }

        public static IDisposable UniqueForUserName()
        {
            return NamerFactory.AsEnvironmentSpecificTest(GetUserName);
        }

        public static IDisposable ForScenario(string data)
        {
            var name = "ForScenario." + Scrub(data);
            return NamerFactory.AsEnvironmentSpecificTest(() => name);
        }

        public static IDisposable ForScenario(params object[] dataPoints)
        {
            var name = dataPoints.JoinStringsWith(o => "" + o, ".");
            return ForScenario(name);
        }

        public static string Scrub(string data)
        {
            var invalid = Path.GetInvalidFileNameChars().ToArray();
            var chars = data.Select(c => invalid.Contains(c) ? '_' : c).ToArray();
            return new string(chars);
        }
    }
}