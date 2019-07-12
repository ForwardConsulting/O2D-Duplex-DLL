using System;
using System.IO;
using Duplex;
using Duplex.Tool;
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

        private static string _ftpserver;
        private static string _ftpuser;
        private static string _ftppassword;
        private static string _ftpFolder;
        private static int _logDownloadProcessid;

        private static clsLog Log;
        static void Main(string[] args)
        {
            //try
            //{
           

            GetConfig();
            Log = new clsLog(_conStr);
            clsSFTP sftp = new clsSFTP();
            sftp.sftpIP = _ftpserver;
            sftp.Sftpuser = _ftpuser;
            sftp.Sftppwd = _ftppassword;
            sftp.Remotefolder = _ftpFolder;
            _logDownloadProcessid=Log.LogProcessInsert (clsLog.Logger.Inventory,clsLog.ProcessCategory.Inventory, "Start Download File from SFTP",DateTime.Now);
            try
            {
                sftp.DownloadLatestFile();
                Log.LogProcessUpdate(_logDownloadProcessid, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.LogAlert(clsLog.Logger.Inventory, clsLog.ErrorLevel.MiddleImpact, clsLog.ProcessCategory.Inventory, $"Inventory snapshot download problem found:{ex.Message}");
            }

            try
            {
                Inventory inv = new Inventory(_conStr);
                inv.ImportOpening(sftp.LocalFile);
            }
            catch (Exception ex)
            {
                Log.LogAlert(clsLog.Logger.Inventory, clsLog.ErrorLevel.MiddleImpact, clsLog.ProcessCategory.Inventory, $"Inventory snapshot IMPORT problem found:{ex.Message}");
            }



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

                _ftpserver = config["ftpserver"];
                _ftpuser = config["ftpuser"];
                _ftppassword = config["ftppassword"];
                _ftpFolder = config["ftpfolder"];

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
