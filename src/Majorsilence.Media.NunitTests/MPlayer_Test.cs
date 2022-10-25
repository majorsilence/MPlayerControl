using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace MplayerUnitTests
{

    [TestFixture()]
    public class MPlayer_Test
    {

        #region Test Video1

        [Test()]
        public void MemoryLeak_Test()
        {
            for (int i = 0; i < 500; i ++) {
                using (var a = new Majorsilence.Media.Videos.MPlayer (-1, Majorsilence.Media.Videos.MplayerBackends.ASCII, GlobalVariables.MplayerPath)) {
                    a.Play (GlobalVariables.Video1Path);
                }

            }

            var processes = System.Diagnostics.Process.GetProcessesByName ("mplayer");
            Assert.That (processes.Any (), Is.EqualTo (false), $"mplayer process count is {processes.Count()}");

        }

       
        #endregion
    }
}
