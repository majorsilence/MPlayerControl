using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace MplayerUnitTests
{
    [SetUpFixture]
    public class SetupInitialize
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            //Startup s = new Startup();
            //s.Initialize();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            // ...
        }

    }
}
