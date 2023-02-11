using System.Diagnostics;
using System.Linq;
using Majorsilence.Media.Videos;
using NUnit.Framework;

namespace MplayerUnitTests;

[TestFixture]
public class MPlayer_Test
{
    [Test]
    public void MemoryLeak_Test()
    {
        for (var i = 0; i < 500; i++)
            using (var a = new MPlayer(-1, MplayerBackends.ASCII, GlobalVariables.MplayerPath))
            {
                a.Play(GlobalVariables.Video1Path);
            }

        var processes = Process.GetProcessesByName("mplayer");
        Assert.That(processes.Any(), Is.EqualTo(false), $"mplayer process count is {processes.Count()}");
    }
}