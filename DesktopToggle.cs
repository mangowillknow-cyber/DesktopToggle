using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class DesktopToggle : ApplicationContext
{
    private NotifyIcon trayIcon;
    private bool iconsHidden = false;
    private Icon iconShown, iconHidden;

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    static IntPtr GetDesktopListView()
    {
        IntPtr progman = FindWindow("Progman", null);
        IntPtr workerW = IntPtr.Zero, shellDefView;
        do
        {
            workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
            if (workerW != IntPtr.Zero)
            {
                shellDefView = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellDefView != IntPtr.Zero) break;
            }
        } while (workerW != IntPtr.Zero);
        shellDefView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
        if (shellDefView != IntPtr.Zero)
            return FindWindowEx(shellDefView, IntPtr.Zero, "SysListView32", "FolderView");
        return IntPtr.Zero;
    }

    Bitmap RenderIcon(bool hidden)
    {
        var bmp = new Bitmap(24, 24, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);

            if (hidden)
            {
                using (var pen = new Pen(Color.FromArgb(220, 180, 180, 180), 1.6f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawRectangle(pen, 4, 6, 16, 12);
                    g.DrawLine(pen, 12, 18, 12, 21);
                    g.DrawLine(pen, 8, 21, 16, 21);
                }
                using (var slash = new Pen(Color.FromArgb(200, 255, 100, 80), 1.6f))
                { slash.StartCap = LineCap.Round; slash.EndCap = LineCap.Round; g.DrawLine(slash, 3, 4, 21, 22); }
            }
            else
            {
                Color accent = Color.FromArgb(220, 60, 130, 230);
                using (var brush = new SolidBrush(Color.FromArgb(50, 60, 130, 230))) { g.FillRectangle(brush, 4, 6, 16, 12); }
                using (var pen = new Pen(accent, 1.8f))
                {
                    pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round;
                    g.DrawRectangle(pen, 4, 6, 16, 12);
                    g.DrawLine(pen, 12, 18, 12, 21);
                    g.DrawLine(pen, 8, 21, 16, 21);
                }
                using (var dot = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                {
                    g.FillRectangle(dot, 7, 9, 3, 3);
                    g.FillRectangle(dot, 12, 9, 3, 3);
                    g.FillRectangle(dot, 7, 13, 3, 3);
                }
            }
        }
        return bmp;
    }

    Icon CreateIcon(bool hidden)
    {
        using (var bmp = RenderIcon(hidden))
        {
            return (Icon)Icon.FromHandle(bmp.GetHicon()).Clone();
        }
    }

    public DesktopToggle()
    {
        // Pre-render both icons
        iconShown = CreateIcon(false);
        iconHidden = CreateIcon(true);

        trayIcon = new NotifyIcon
        {
            Icon = iconShown,
            Text = "桌面图标：显示中",
            Visible = true
        };

        trayIcon.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Left) Toggle();
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("切换隐藏/显示", null, (s, e) => Toggle());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("退出", null, (s, e) => ExitApp());
        trayIcon.ContextMenuStrip = menu;

        Application.ApplicationExit += (s, e) =>
        {
            if (iconsHidden) { IntPtr lv = GetDesktopListView(); if (lv != IntPtr.Zero) ShowWindow(lv, SW_SHOW); }
            trayIcon.Visible = false;
            trayIcon.Dispose();
            if (iconShown != null) { iconShown.Dispose(); }
            if (iconHidden != null) { iconHidden.Dispose(); }
        };
    }

    void Toggle()
    {
        IntPtr lv = GetDesktopListView();
        if (lv == IntPtr.Zero) return;
        iconsHidden = !iconsHidden;
        ShowWindow(lv, iconsHidden ? SW_HIDE : SW_SHOW);

        // Direct swap, no rendering
        trayIcon.Icon = iconsHidden ? iconHidden : iconShown;
        trayIcon.Text = iconsHidden ? "桌面图标：隐藏中" : "桌面图标：显示中";
    }

    void ExitApp() { Application.Exit(); }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new DesktopToggle());
    }
}
