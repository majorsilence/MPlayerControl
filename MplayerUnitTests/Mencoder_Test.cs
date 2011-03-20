using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace MplayerUnitTests
{

    [TestFixture()]
    public class Mencoder_Test
    {


        [Test()]
        public void Convert_Test1()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2WebM(GlobalVariables.Video8Path, GlobalVariables.OutputVideoWebM);
        }

        [Test()]
        public void Convert_Test2()
        {
            LibMPlayerCommon.Mencoder a = new LibMPlayerCommon.Mencoder();
            a.Convert2X264(GlobalVariables.Video8Path, GlobalVariables.OutputVideoX264);
        }

    }
}
