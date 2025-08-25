using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Tests.Tablet;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Samples;

public partial class CustomSamples
{
    private const string pattern = @"^\{ (?:(\w{2})+ )+\}, Delta:(.+)ms, Touch data:(?:, Point \#(\d+): <(\d+)  (\d+)>;)*, ReportType:(\w+)$";

    public static List<ITouchReport> Reports { get; } = new();
    public static List<float> Deltas { get; } = new();

    public static void Initialize()
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "Dumps/UserTapReports.txt";

        if (!File.Exists(path))
            return;

        var serializedReports = File.ReadAllText(path);
        var split = serializedReports.Replace("\r", "").Split('\n');

        foreach (var line in split)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var match = Pattern().Match(line);

            if (match.Success)
            {
                // Group 1 should contain the hex values
                var values = match.Groups[1].Captures;
                var data = new byte[values.Count];

                for (int i = 0; i < values.Count; i++)
                {
                    data[i] = Convert.ToByte(values[i].Value, 16);
                }

                Deltas.Add(float.Parse(match.Groups[2].Value, CultureInfo.CurrentCulture));

                TouchPoint[] touches = new TouchPoint[1];

                // Touch data was captured
                if (match.Groups[3].Success && 
                    match.Groups[4].Success && 
                    match.Groups[5].Success)
                {
                    touches[0] = new()
                    {
                        TouchID = byte.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                        Position = new Vector2(int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                                               int.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture))
                    };
                }

                var report = new GenericTouchReport()
                {
                    Raw = data,
                    Touches = touches
                };

                Reports.Add(report);
            }
        }
    }

    public static void TestByDelta(ITestOutputHelper output)
    {
        if (Reports.Count != Deltas.Count)
            throw new InvalidOperationException("Reports and Deltas count mismatch");

        if (Reports.Count == 0)
            return;

        int completedCount = 0;
        int endedCount = 0;

        int deltaSum = 0;
        double deadline = 300;
        bool previousEnded = false;

        for (int i = 0; i < Reports.Count; i++)
        {
            var touches = Reports[i].Touches;
            var delta = Deltas[i];

            if (!previousEnded)
                deltaSum += (int)delta;

            // touch point released
            if (touches[0] == null)
            {
                endedCount++;

                if (deltaSum < deadline)
                {
                    completedCount++;
                    output.WriteLine($"Attempt {endedCount} Completed at {deltaSum}ms");
                }
                else
                    output.WriteLine($"Attempt {endedCount} Failed at {deltaSum}ms");

                deltaSum = 0;

                previousEnded = true;
            }
            else
                previousEnded = false;
        }

        output.WriteLine($"Gesture was completed {completedCount} / {endedCount} Attempts by time.");
    }

    public static void GetMaxesFromReports(ITestOutputHelper output)
    {
        float maxX = 0;
        float maxY = 0;

        foreach (var report in Reports)
        {
            foreach (var touch in report.Touches)
            {
                if (touch == null)
                    continue;

                maxX = Math.Max(maxX, touch.Position.X);
                maxY = Math.Max(maxY, touch.Position.Y);
            }
        }

        output.WriteLine($"Max X: {maxX}");
        output.WriteLine($"Max Y: {maxY}");
    }

    [GeneratedRegex(pattern)]
    private static partial Regex Pattern();
}