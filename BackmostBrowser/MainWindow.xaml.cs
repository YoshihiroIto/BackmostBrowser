using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace BackmostBrowser
{
    public partial class MainWindow
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private static IntPtr HWND_BOTTOM = (IntPtr) 1;

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore InconsistentNaming

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
                // ReSharper disable once PossibleNullReferenceException
                source.AddHook(WndProc);

                var args = Environment.GetCommandLineArgs();

                var webAddress = args.Length != 2 ? "https://google.com" : args[1];

                WebView.Source = new Uri(webAddress);
                Title = webAddress;
            };

            StateChanged += (_, _) => WindowStyle = WindowState == WindowState.Maximized ? WindowStyle.None : WindowStyle.ThreeDBorderWindow;

            WebView.PreviewKeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_WINDOWPOSCHANGING:
                    if (WindowState != WindowState.Maximized)
                        break;

                    // ReSharper disable once PossibleNullReferenceException
                    var wp = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    wp.hwndInsertAfter = HWND_BOTTOM;
                    Marshal.StructureToPtr(wp, lParam, true);
                    break;
            }

            return IntPtr.Zero;
        }
    }
}