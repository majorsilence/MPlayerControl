using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace MplayerUnitTests
{
    [SetUpFixture]
    public class SetupInitialize
    {
        private static string finalPath;

        [OneTimeSetUp]
        public void RunBeforeAnyTests ()
        {
            //Startup s = new Startup();
            //s.Initialize();

#if DEBUG
            string path = Path.Combine ("../", "../", "../", "TestRun");
#else
            var path = Path.Combine(Path.GetTempPath(), "MPlayerControl", "Tests");
#endif

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            finalPath = Path.Combine (path, "TestVideos");
            var testVideos = Path.Combine(path, "TestVideos.zip");
            if (!File.Exists(testVideos))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("http://files.majorsilence.com/TestVideos.zip", 
                        testVideos);
                }
            }
            ExtractZip (path, testVideos);

            GlobalVariables.InitPath(finalPath, "mplayer", "/usr/lib/x86_64-linux-gnu/libmpv.so.1");

        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            // TODO, delete output files
            System.IO.Directory.Delete (finalPath, true);
        }

        public void ExtractZip(string extractFolder, string currentZipFile, string zipProgramPath = "")
        {
            if (zipProgramPath.Trim() == "")
            {
                zipProgramPath = "7za";
            }

            string programPath = zipProgramPath;
            Console.WriteLine(string.Format("7za Path: {0}", programPath));
            using (var p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = programPath,
                    UseShellExecute = false,
                    Arguments = string.Format("-y x \"{0}\" -o\"{1}\"", currentZipFile, extractFolder)
                }
            })
            {
                p.Start();
                p.WaitForExit();

                Debug.Assert(p.ExitCode == 0);
                if (p.ExitCode > 0)
                {
                    Console.WriteLine(string.Format("Error unzipping file {0}", currentZipFile));
                    Environment.Exit(1);
                }
                p.Close();
            }
        }

    }
}
