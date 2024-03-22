using System.Drawing;
using System.Numerics;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    public class HoldGestureTest
    {
        #region Constants & Readonly fields

        protected const int MAX_TOUCHES = 3;
        protected const double DEADLINE = 500;
        protected const int TESTED_TOUCHES = 2;

        protected readonly Rectangle BOUNDS = new(0, 0, 30, 30);
        protected readonly Vector2 THRESHOLD = new(30, 30);

        #endregion

        #region Constructor

        protected readonly ITestOutputHelper _output;

        public HoldGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Test Samples

        

        #endregion

        #region Tests



        #endregion
    }
}