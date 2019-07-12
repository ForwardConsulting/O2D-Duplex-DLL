using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Duplex.Tool
{
    public class clsSFTP
    {

        private string _sftpIP;
        private string _sftpuser;
        private string _sftppwd;
        private string _remotefolder;
        private string _remotefileName;
        private FileInfo _localFile;

        public string sftpIP { get => _sftpIP; set => _sftpIP = value; }
        public string Sftpuser { get => _sftpuser; set => _sftpuser = value; }
        public string Sftppwd { get => _sftppwd; set => _sftppwd = value; }
        public string RemoteFileName { get => _remotefileName; set => _remotefileName = value; }
        public FileInfo LocalFile { get => _localFile; set => _localFile = value; }
        public string Remotefolder { get => _remotefolder; set => _remotefolder = value; }

        public clsSFTP()
        {
        }

        //public int Send(string fileName)
        //{
        //    var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));
        //    // Upload File
        //    using (var sftp = new SftpClient(connectionInfo))
        //    {

        //        sftp.Connect();
        //        //sftp.ChangeDirectory("/MyFolder");
        //        using (var uplfileStream = System.IO.File.OpenRead(fileName))
        //        {
        //            sftp.UploadFile(uplfileStream, fileName, true);
        //        }
        //        sftp.Disconnect();
        //    }
        //    return 0;
        //}
        public int DownloadLatestFile()
        {
            try
            {


                GetLatestFile();
                ConnectionInfo conInfo = new ConnectionInfo(_sftpIP, _sftpuser, new PasswordAuthenticationMethod(_sftpuser, _sftppwd));
                if (_localFile == null) { _localFile = new FileInfo("C:\\Temp\\SAPSnapshot.txt"); }
                using (SftpClient sftp = new SftpClient(conInfo))
                {
                    using (FileStream fstr = new FileStream(_localFile.FullName, FileMode.Create))
                    {
                        sftp.Connect();
                        sftp.ChangeDirectory($"/{_remotefolder}");
                        sftp.DownloadFile(_remotefileName, fstr);
                        
                    }


                }
                return 0;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error on DownloadLatestFile:{ex.Message }");
            }
        }
        public void GetLatestFile()
        {
            try
            {
              

                ConnectionInfo conInfo = new ConnectionInfo(_sftpIP, _sftpuser, new PasswordAuthenticationMethod(_sftpuser, _sftppwd));

                using (SftpClient sftp = new SftpClient(conInfo))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory("/SKICO2D");
                    IEnumerable<SftpFile> fs1;
                    fs1 = sftp.ListDirectory("/SKICO2D");
                    foreach (var item in fs1)
                    {
                        //It will run to the end of last file by character ascending
                        _remotefileName = item.Name;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on GetLatestFileDate:{ex.Message }");
            }
        }
    }
}
