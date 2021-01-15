using DbUp;
using System;
using System.Linq;
using System.Reflection;

namespace DbMigration
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString = args.FirstOrDefault()
                                   ??
                                   "Server=172.27.192.146;Uid=testuser;Pwd=windows10#;Database=orleans";

            var upgrader =
                DeployChanges.To               
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                #if DEBUG
                Console.ReadLine();
                #endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
