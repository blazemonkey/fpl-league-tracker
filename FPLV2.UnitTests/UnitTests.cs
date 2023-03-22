using FPLV2.Database.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace FPLV2.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        protected UnitOfWork UnitOfWork { get; private set; }
        protected static IConfiguration Configuration { get; private set; }
        private static string ConnectionString { get; set; }
        private static string DbName { get; set; } = "FPLV2UnitTests";

        [AssemblyInitialize]
        public static void ClassInit(TestContext context)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile("appsettings.Development.json", true, false)
                .AddEnvironmentVariables()
                .Build();

            Configuration = configuration;

            var connString = configuration.GetConnectionString("DefaultConnection");
            Assert.IsFalse(string.IsNullOrEmpty(connString), "DefaultConnection not set");

            var builder = new DbConnectionStringBuilder()
            {
                ConnectionString = connString
            };

            builder["Initial Catalog"] = DbName;
            ConnectionString = builder.ToString();
            CreateTestDbIfRequired();
        }

        private static string GetMasterConnectionString()
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = ConnectionString
            };

            builder["Initial Catalog"] = "master";
            return builder.ToString();
        }

        [TestInitialize]
        public async Task TestStart()
        {
            UnitOfWork = GetUnitOfWork();

            await UnitOfWork.Points.DeleteAll();
            await UnitOfWork.Players.DeleteAll();
            await UnitOfWork.Leagues.DeleteAll();
            await UnitOfWork.ElementStats.DeleteAll();
            await UnitOfWork.Elements.DeleteAll();
            await UnitOfWork.Teams.DeleteAll();
            await UnitOfWork.Seasons.DeleteAll();
        }

        /// <summary> 
        /// Build a custom settings file 
        /// </summary> 

        private IConfiguration GetConfiguration()
        {
            var config = new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", ConnectionString }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            return configuration;
        }


        /// <summary> 
        /// Sets UnitOfWork via reflection so we don't have to update this method when the constructor parameters change 
        /// </summary> 
        /// <exception cref="Exception">If any unexpected issues are found</exception> 
        private UnitOfWork GetUnitOfWork()
        {
            var constructor = typeof(UnitOfWork).GetConstructors().FirstOrDefault();
            if (constructor == null)
                throw new Exception("Where's the constructor?");

            var configuration = GetConfiguration();

            var parameters = constructor.GetParameters();
            var constructorParameters = new List<object>();
            foreach (var p in parameters)
            {
                var instance = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => p.ParameterType.IsAssignableFrom(x) && x.IsInterface == false && x.IsAbstract == false).FirstOrDefault();
                if (instance == null)
                    throw new Exception($"Can't find concrete implementation of interface '{p.ParameterType}");

                var parameter = Activator.CreateInstance(instance.UnderlyingSystemType, configuration);
                constructorParameters.Add(parameter);
            }

            var unitOfWork = Activator.CreateInstance(typeof(UnitOfWork), constructorParameters.ToArray());
            return (UnitOfWork)unitOfWork;
        }

        protected static string GetScriptsDir()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.LastIndexOf("FPLV2.UnitTests"));

            path = Path.Combine(path, "FPLV2.Database", "Scripts");
            if (Directory.Exists(path) == false)
                throw new Exception($"Failed to find directory: {path}");

            return path;
        }

        private static void CreateTestDbIfRequired()
        {
            var exists = CheckTestDbExists();
            if (exists)
                DropTestDb();

            CreateTestDb();
        }

        private static bool CheckTestDbExists()
        {
            using var conn = new SqlConnection(GetMasterConnectionString());
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = string.Format($"select db_id('{DbName}')");

            var exists = int.TryParse(cmd.ExecuteScalar().ToString(), out int dbId);
            return exists && dbId > 0;
        }

        private static void DropTestDb()
        {
            using var conn = new SqlConnection(GetMasterConnectionString());
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = string.Format($"alter database {DbName} set single_user with rollback immediate; drop database {DbName};");
            cmd.ExecuteNonQuery();
        }

        private static void CreateTestDb()
        {
            var dir = GetScriptsDir();

            var sql = Path.Combine(dir, "CreateTables.sql");
            var text = new StringBuilder();

            var createDb = File.ReadAllText(sql);
            text.AppendLine(createDb);
            var spPath = Path.Combine(dir, "Programmability");
            var spFiles = Directory.GetFiles(spPath);
            foreach (var spFile in spFiles)
            {
                var sp = File.ReadAllText(spFile);
                text.AppendLine(sp);
            }

            sql = Path.Combine(dir, "InitialData.sql");
            var initialData = File.ReadAllText(sql);
            text.AppendLine(initialData);

            using var conn = new SqlConnection(GetMasterConnectionString());
            conn.Open();

            using var cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = string.Format($"create database {DbName}");
            cmd.ExecuteNonQuery();

            using var unitTestConn = new SqlConnection(ConnectionString);
            unitTestConn.Open();

            foreach (var t in SplitSqlStatements(text.ToString()))
            {
                using var unitTestCmd = new SqlCommand();
                unitTestCmd.Connection = unitTestConn;
                unitTestCmd.CommandText = t;
                unitTestCmd.ExecuteNonQuery();
            }
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements 
            var statements = Regex.Split(
                    sqlScript,
                    @"^[\t ]*GO[\t ]*\d*[\t ]*(?:--.*)?(?=($|[\s]+))",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);


            // Remove empties, trim, and return 
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}