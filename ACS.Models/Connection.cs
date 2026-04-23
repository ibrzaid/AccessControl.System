using ACS.Database.IConnection;

namespace ACS.Models
{
    public class Connection : IConnection
    {
        /// <summary>
        /// Database Server
        /// </summary>
        public string? Server { get; set; }

        /// <summary>
        /// Database Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Database User
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// Database Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Database Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Database Pooling
        /// </summary>
        public bool Pooling { get; set; }

        /// <summary>
        /// Database Type
        /// </summary>
        public string? DatabaseType { get; set; }

        /// <summary>
        /// Explicit Interface Implementation (Fixes Nullability Warning)
        /// </summary>
        string IConnection.Server => Server ?? string.Empty;
        string IConnection.Name => Name   ?? string.Empty;
        string IConnection.User => User   ?? string.Empty;
        string IConnection.Password => Password ?? string.Empty;

        /// <summary>
        /// Database Type (enum)
        /// </summary>
        public DatabaseType Type
        {
            get
            {
                var type = Database.Connection.Type.DatabaseType(
                    Database.Connection.Type.RemoveSpace(this.DatabaseType ?? "")
                );
                return type;
            }
        }

        /// <summary>
        /// Get Database Connection Object
        /// </summary>
        public Database.Connection.Connection Conn
        {
            get
            {
                return this.Type switch
                {
                    Database.IConnection.DatabaseType.OracleDatabase => new Database.Connection.OracleDatabase.Connection(this),
                    Database.IConnection.DatabaseType.PostgresDatabase => new Database.Connection.PostgresDatabase.Connection(this),
                    _ => new Database.Connection.PostgresDatabase.Connection(this),
                };
            }
        }
    }
}