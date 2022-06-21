using System;
using System.Windows.Media;

namespace MilitaryShooter
{
    public static class CompositionTargetEx
    {
        private static TimeSpan _last = TimeSpan.Zero;

        private static event EventHandler<RenderingEventArgs>? FrameUpdatingP;

        public static event EventHandler<RenderingEventArgs> FrameUpdating
        {
            add
            {
                if (FrameUpdatingP == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                FrameUpdatingP += value;
            }
            remove
            {
                FrameUpdatingP -= value;
                if (FrameUpdatingP == null)
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        private static void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;
            if (args.RenderingTime == _last)
                return;
            _last = args.RenderingTime;
            FrameUpdatingP!(sender, args);
        }
    }
}