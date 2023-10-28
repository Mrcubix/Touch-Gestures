using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using OTD.EnhancedOutputMode.Lib.Interface;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures
{
    [PluginName(PLUGIN_NAME)]
    public class GesturesHandler : IFilter, IGateFilter, IDisposable
    {
        public const string PLUGIN_NAME = "Touch Gestures";

        private RpcServer<GesturesDaemon> rpcServer;

        public GesturesHandler()
        {
            // start the RPC server
            rpcServer = new RpcServer<GesturesDaemon>("GesturesDaemon");

            _ = Task.Run(() => rpcServer.MainAsync());
        }

        public Vector2 Filter(Vector2 input) => input;

        public List<IGesture> Gestures { get; set; } = new();

        public bool Pass(IDeviceReport report, ref ITabletReport tabletreport)
        {
            if (report is ITouchReport touchReport)
            {
                if (rpcServer.Instance.IsReady)
                {
                    // this part has nothing to do with the RPC server
                    foreach (var gesture in Gestures)
                    {
                        gesture.OnInput(touchReport.Touches);

                        /*if (gesture.HasStarted && !gesture.HasEnded)
                        {
                            if (gesture is IMixedBasedGesture mixedGesture)
                            {
                                if (mixedGesture.CurrentNodeIndex == mixedGesture.Nodes.Count - 1)
                                    mixedGesture.HasEnded = true;
                                else
                                    mixedGesture.CurrentNodeIndex++;
                            }
                            else
                            {
                                gesture.HasEnded = true;
                            }
                        }

                        if (!gesture.HasStarted)
                        {
                            gesture.HasStarted = true;
                        }*/
                    }

                    
                }
            }

            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public FilterStage FilterStage => FilterStage.PreTranspose;

        [Property("Numerical Input Box Property"),
         Unit("Some Unit Here"),
         DefaultPropertyValue(727),
         ToolTip("Filter template:\n\n" +
                 "A property that appear as an input box.\n\n" +
                 "Has a numerical value.")
        ]
        public int ExampleNumericalProperty { get; set; }
        [Property("String Type Input Box Property"),
         DefaultPropertyValue("727"),
         ToolTip("Filter template:\n\n" +
                 "A property that appear as an input box.\n\n" +
                 "Has a string value.")
        ]
        public int ExampleStringProperty { get; set; }
        [BooleanProperty("Boolean Property", ""),
         DefaultPropertyValue(true),
         ToolTip("Area Randomizer:\n\n" +
                 "A property that appear as a check box.\n\n" +
                 "Has a Boolean value")
        ]
        public bool ExampleBooleanProperty { set; get; }
    }
}
