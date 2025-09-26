

namespace System
{
    internal class MouseEventArgs
    {
        private Action<object, EventArgs> trackBarMove_MouseUp;
        private Action<object, Windows.Forms.MouseEventArgs> trackBarTurn_MouseUp;

        public MouseEventArgs(Action<object, EventArgs> trackBarMove_MouseUp)
        {
            this.trackBarMove_MouseUp = trackBarMove_MouseUp;
        }

        public MouseEventArgs(Action<object, Windows.Forms.MouseEventArgs> trackBarTurn_MouseUp)
        {
            this.trackBarTurn_MouseUp = trackBarTurn_MouseUp;
        }
    }
}