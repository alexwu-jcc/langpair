namespace FtpFileDiscover
{

    using Renci.SshNet;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using static System.Net.Mime.MediaTypeNames;

    public class FtpFileAttributeQuery
    {
        private string sftpHost = "rsync.vny.1A57.edgecastcdn.net";
        private string sftpUser = "ALEX.WU@JUD.CA.GOV";
        private string sftpPass = "5978Lantanah!";
        private int sftpPort = 22;
        private Dictionary<string, string> file2lastUpdateTime = new Dictionary<string, string>();
        private Dictionary<string, string> file2lastAccessTime = new Dictionary<string, string>();
        private List<Tuple<string, long>> pq = new List<Tuple<string, long>>();
        private int dirCounter = 0;
        private int fileCounter = 0;
        private string output = "";


        public FtpFileAttributeQuery()
        {
            output = "file_info\\" + Path.Combine(output, DateTime.Now.ToString("MM-dd"));

            string currentDirectory = Directory.GetCurrentDirectory();
            int backWardCount = 3;
            while (backWardCount > 0)
            {
                currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("\\"));
                backWardCount--;
            }
            output = Path.Combine(currentDirectory, output);
            Directory.CreateDirectory(output);
        }


        public string Output{
            get
            {
                return this.output;
            }
        }

        public void Dfs(SftpClient sftp, string path, HashSet<string> seen)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Return due to empty path");
                return;
            }

            if (seen.Contains(path))
            {
                Console.WriteLine("Return due to seen before");
                return;
            }

            try
            {
                sftp.ChangeDirectory(path);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot change directory {path}, exception: {e}");
                return;
            }

            var listFileAttr = sftp.ListDirectory(path);

            int localFileCount = 0;
            foreach (var fileAttr in listFileAttr)
            {
                try
                {
                    string filename = fileAttr.Name;

                    if (fileAttr.IsDirectory && !filename.StartsWith("."))
                    {
                        dirCounter++;
                        if (dirCounter % 100 == 0)
                        {
                            Console.WriteLine($"Now pq size is {pq.Count}, and file2time size is {file2lastUpdateTime.Count}");
                        }

                        Dfs(sftp, path + "/" + filename, seen);
                    }
                    else if (fileAttr.IsRegularFile)
                    {
                        localFileCount++;
                        fileCounter++;
                        if (fileCounter % 1000 == 0)
                        {
                            Console.WriteLine($"Now pq size is {pq.Count}, and file2time size is {file2lastUpdateTime.Count}");
                        }

                        long filesize = fileAttr.Length;
                        string filepath = path + "/" + filename;
                        string modTime = fileAttr.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                        file2lastUpdateTime[filepath] = modTime;

                        string accessTime = fileAttr.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss");
                        file2lastAccessTime[filepath] = accessTime;

                        AddToPriorityQueue(pq, new Tuple<string, long>(filepath, filesize));
                    }
                }
                catch (Exception e2)
                {
                    Console.WriteLine($"Cannot process file {fileAttr.Name}, exception: {e2}");
                }
            }

            // need take into consideration of those TWO hidden folders
            if (localFileCount == listFileAttr.Count()-2)
            {
                seen.Add(path);
            }
        }

        public void GetResult(string startPath, string outputFile)
        {
            Console.WriteLine("GetResult query started");
            file2lastUpdateTime.Clear();
            file2lastAccessTime.Clear();
            pq.Clear();
            dirCounter = 0;

            using (var ssh = new SshClient(sftpHost, sftpPort, sftpUser, sftpPass))
            {
                ssh.Connect();
                using (var sftp = new SftpClient(ssh.ConnectionInfo))
                {
                    sftp.Connect();
                    HashSet<string> seen = new HashSet<string>();
                    Dfs(sftp, startPath, seen);

                    using (StreamWriter writer = new StreamWriter(outputFile))
                    {
                        writer.WriteLine($"Total files: {pq.Count}, and below files are in the format of offset, size in MB, last update time, last access time, and full file path name.");
                        int count = 0;
                        while (pq.Any())
                        {
                            count++;
                            var temp = pq.First();
                            pq.RemoveAt(0);
                            string fileName = temp.Item1;
                            long size = temp.Item2;
                            writer.WriteLine($"{count}\t\t{size}\t\t{file2lastUpdateTime[fileName]}\t\t{file2lastAccessTime[fileName]}\t\t{fileName}");
                        }
                    }
                }
                ssh.Disconnect();
            }
            Console.WriteLine("GetResult query done");
        }

        public void AddToPriorityQueue(List<Tuple<string, long>> pq, Tuple<string, long> element)
        {
            pq.Add(element);
            pq = pq.OrderBy(e => e.Item2).ToList();
        }

        public Tuple<string, long> PopFromPriorityQueue(List<Tuple<string, long>> pq)
        {
            var element = pq.First();
            pq.RemoveAt(0);
            return element;
        }

        static void Main(string[] args)
        {
            FtpFileAttributeQuery sol = new FtpFileAttributeQuery();
            string[] startPaths = { "/logs", "/cfcc", "/cjer", "/facilities" };

            startPaths = new string[]{ "/logs" };
            foreach (var startPath in startPaths)
            {
                string outputFile = sol.Output +  "/file_info_" + startPath.Replace("/", "_") + ".txt";
                sol.GetResult(startPath, outputFile);
            }

            Console.WriteLine("All done");

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Xml.
                //.Exit(1);
        }
    }

}