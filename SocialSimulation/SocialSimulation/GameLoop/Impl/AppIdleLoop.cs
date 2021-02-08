using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using SocialSimulation.Game;

namespace SocialSimulation.GameLoop.Impl
{
    class AppIdleLoop : ICustomLoopBehavior
    {
        private IGame _game;
        private Stopwatch _sw;
        private double _elapsed;
        private double _previous;

        public void Start(IGame game)
        {
            _game = game;
            _sw = new Stopwatch();
            _sw.Start();
            _previous = _sw.Elapsed.TotalMilliseconds;
            ComponentDispatcher.ThreadIdle += OnThreadIdle;
        }
        private bool AppStillIdle
        {
            get
            {
                MSG msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }
        private void OnThreadIdle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                var el = _sw.Elapsed.TotalMilliseconds;
                _elapsed = el - _previous;
                _previous = el;
                // Render a frame during idle time (no messages are waiting)
                _game.Update(/*(float)_elapsed*/1);
                _game.Render(/*(float)_elapsed*/1);

            }
        }

        public void Stop(IGame game)
        {
            _sw.Stop();
            ComponentDispatcher.ThreadIdle -= OnThreadIdle;
            game.Unload();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            [MarshalAs(UnmanagedType.U4)]
            public int time;
            public Point pt;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PeekMessage(
            out MSG lpMsg,
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.U4)] uint wMsgFilterMin,
            [MarshalAs(UnmanagedType.U4)] uint wMsgFilterMax,
            [MarshalAs(UnmanagedType.U4)] uint wRemoveMsg);
    }
}
