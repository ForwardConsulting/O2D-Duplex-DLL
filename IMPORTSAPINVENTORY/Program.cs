using System;
using System.IO;
using Duplex;
using Microsoft.Extensions.Configuration;

namespace IMPORTSAPINVENTORY
{
    class Program
    {
        private static string _server;
        private static string _dbase;
        private static string _dbuser;
        private static string _dbpassword;
        private static string _conStr;
        private static string _txtFile;
        static void Main(string[] args)
        {
            //try
            //{
                if (args.Length != 1)
                {
                    Console.WriteLine($"1 Parameters required (TextFilePath). Connection should already be provided by .json");
                    return;
                }

                _txtFile = args[0];
                Console.WriteLine($"Text File Path is {_txtFile}");
                Console.WriteLine($"Connection to database is {_conStr}");

                System.IO.FileInfo fs = new FileInfo(_txtFile);
                GetConfig();
                Inventory inv = new Inventory(_conStr);
                inv.ImportOpening(fs);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error found:{ex.Message}"); ;
            //}

            //Console.WriteLine($" Hello { _conStr } !");
        }


        private static void GetConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json");

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
                _server = config["server"];
                _dbase = config["database"];
                _dbuser = config["user"];
                _dbpassword = config["password"];

                //connection always point to O2D
                _conStr = $"Data Source= {_server},1433;Initial Catalog={_dbase};Timeout=30;User Id={_dbuser};Password={_dbpassword};MultipleActiveResultSets=True;Application Name=IMPORTSAPINVENTORY;";


            }
            catch (Exception ex)
            {
                throw new Exception("Error on GetConfig:" + ex.Message);
            }

        }

    }
}
