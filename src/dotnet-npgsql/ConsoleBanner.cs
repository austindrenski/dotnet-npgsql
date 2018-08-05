using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql;

namespace npgsql
{
    public static class ConsoleBanner
    {
        /// <summary>
        /// The target framework of the current program (e.g. .NETCoreApp,Version=v2.1).
        /// </summary>
        [NotNull] static readonly string Framework =
            Assembly.GetEntryAssembly()
                    .GetCustomAttribute<TargetFrameworkAttribute>()
                    .FrameworkName;

        /// <summary>
        /// The informational version of <see cref="Npgsql"/> (e.g. 4.0.2).
        /// </summary>
        [NotNull] static readonly string Version =
            typeof(NpgsqlConnection)
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

        /// <summary>
        /// Prints the npgsql banner with version and framework information.
        /// </summary>
        public static async Task Write([NotNull] TextWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            await writer.WriteLineAsync($"npgsql ({Version} ({Framework}))");
            await writer.WriteLineAsync("Type \"help\" for help.");
            await writer.WriteLineAsync();
        }
    }
}