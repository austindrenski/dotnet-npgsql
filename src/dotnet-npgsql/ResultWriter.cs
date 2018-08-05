using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql;

namespace npgsql
{
    public class ResultWriter
    {
        [NotNull] private readonly TextWriter _stdout;
        [NotNull] private readonly TextWriter _stderr;

        public ResultWriter([NotNull] TextWriter stdout, [NotNull] TextWriter stderr)
        {
            _stdout = stdout ?? throw new ArgumentNullException(nameof(stdout));
            _stderr = stderr ?? throw new ArgumentNullException(nameof(stderr));
        }

        public async Task Execute([NotNull] NpgsqlConnection connection, [NotNull] string sql)
        {
            try
            {
                await Process(connection, sql);
            }
            catch (NpgsqlException e)
            {
                await _stderr.WriteLineAsync(e.Message);
            }
        }

        async Task Process([NotNull] NpgsqlConnection connection, [NotNull] string sql)
        {
            (string[] headers, List<string[]> values) = await ReadResults(connection, sql);

            for (int i = 0; i < headers.Length; i++)
            {
                if (i != 0)
                    await _stdout.WriteAsync(" | ");

                await _stdout.WriteAsync(headers[i]);
            }

            await Console.Out.WriteLineAsync();

            for (int i = 0; i < headers.Length; i++)
            {
                if (i != 0)
                    await _stdout.WriteAsync("-|-");

                await _stdout.WriteAsync(new string('-', headers[i].Length));
            }

            await Console.Out.WriteLineAsync();

            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < headers.Length; j++)
                {
                    if (j != 0)
                        await Console.Out.WriteAsync(" | ");

                    await Console.Out.WriteAsync(values[i][j]);
                }

                await _stdout.WriteLineAsync();
            }

            await _stdout.WriteLineAsync();
        }

        static async Task<(string[] Headers, List<string[]> Values)> ReadResults([NotNull] NpgsqlConnection connection, [NotNull] string sql)
        {
            if (connection.State <= ConnectionState.Closed)
                await connection.OpenAsync();

            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var headers = new string[reader.FieldCount];
                    var types = new Type[reader.FieldCount];

                    for (int i = 0; i < headers.Length; i++)
                    {
                        headers[i] = reader.GetName(i);
                        types[i] = reader.GetFieldType(i);
                    }

                    var values = new List<string[]>();

                    while (await reader.ReadAsync())
                    {
                        var row = new string[headers.Length];

                        for (int i = 0; i < headers.Length; i++)
                        {
                            row[i] = reader.GetValue(i).ToString();
                        }

                        values.Add(row);
                    }

                    connection.Close();

                    return (headers, values);
                }
            }
        }
    }
}