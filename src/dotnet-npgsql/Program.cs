using System;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql;

namespace npgsql
{
    public static class Program
    {
        /// <summary>
        /// The console color at startup.
        /// </summary>
        static readonly ConsoleColor InitialForeground = Console.ForegroundColor;

        /// <summary>
        /// The <see cref="StringBuilder"/> for SQL commands.
        /// </summary>
        [NotNull] static readonly StringBuilder SqlText = new StringBuilder();

        /// <summary>
        /// The <see cref="ResultWriter"/> responsible for executing and presenting query results.
        /// </summary>
        [NotNull] static readonly ResultWriter ResultWriter = new ResultWriter(Console.Out, Console.Error);

        /// <summary>
        /// The main entry point for the npgsql CLI.
        /// </summary>
        /// <param name="args">The array of commandline arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">A connection string is required.</exception>
        public static async Task Main([NotNull] string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("A connection string is required.");

            await ConsoleBanner.Write(Console.Out);

            using (var connection = new NpgsqlConnection(args[0]))
            {
                var isOpen = connection.OpenAsync();

                var database = connection.Database ?? Environment.UserName;

                while (true)
                {
                    await WritePrefix(database);

                    SqlText.Append(Console.ReadLine());

                    if (SqlText[SqlText.Length - 1] != ';')
                        continue;

                    await isOpen;
                    await ResultWriter.Execute(connection, SqlText.ToString());

                    SqlText.Clear();
                }
            }
        }

        /// <summary>
        /// Writes a line prefix to the terminal.
        /// </summary>
        /// <param name="database">The current database.</param>
        /// <param name="prefix">The prefix continuation character.</param>
        static async Task WritePrefix([NotNull] string database, Prefix prefix = Prefix.Complete)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            await Console.Out.WriteAsync($"{database}{(char) prefix}# ");
            Console.ForegroundColor = InitialForeground;
            await Console.Out.FlushAsync();
        }
    }
}