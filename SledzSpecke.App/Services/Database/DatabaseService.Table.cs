using SledzSpecke.App.Exceptions;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<bool> TableExists(string tableName)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                var result = await this.database.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=?", tableName);
                return result > 0;
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się sprawdzić istnienia tabeli {tableName}");
        }

        public async Task<List<string>> ListTables()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var tableInfos = await this.database.QueryAsync<TableInfo>(
                    "SELECT name FROM sqlite_master WHERE type='table'");
                return tableInfos.Select(t => t.name).ToList();
            },
            null, "Nie udało się pobrać listy tabel");
        }

        public async Task DropTableIfExists(string tableName)
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                await this.database.ExecuteAsync($"DROP TABLE IF EXISTS {tableName}");
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się usunąć tabeli {tableName}");
        }

        public async Task<int> GetTableRowCount(string tableName)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                try
                {
                    var result = await this.database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tableName}");
                    return result;
                }
                catch
                {
                    return -1; // Tabela nie istnieje lub inny błąd
                }
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się pobrać liczby wierszy w tabeli {tableName}");
        }
    }
}