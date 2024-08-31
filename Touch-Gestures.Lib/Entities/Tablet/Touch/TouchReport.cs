namespace TouchGestures.Lib.Entities.Tablet.Touch
{
    public class TouchReport
    {
        public byte[] Raw { set; get; } = new byte[0];
        public TouchPoint[] Touches { get; init; } = new TouchPoint[0];
    }
}