﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    public class ClipboardWatcher
    {
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

        // クリップボードが変更された時のメッセージコード
        private const int WM_CLIPBOARDUPDATE = 0x031D;


        IntPtr handle;
        HwndSource hwndSource = null;


        public event EventHandler UpdateClipboard;
        //イベント起動
        private void raiseUpdateClipboard()
        {
            UpdateClipboard?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ウィンドウプロシージャー。
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                this.raiseUpdateClipboard();
                handled = true;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="handle">System.Windows.Interop.WindowInteropHelper(this).Handleとかで取得。<br>
        /// ここで使用したハンドルが無効になると本クラスは正常に動作しない（ウィンドウを閉じるなど）。<br>
        /// ウィンドウを閉じてしまうと有効なハンドルがなくなってしまうことからウィンドウを非表示とする場合、Window.Hideなどを使用すること。
        /// </param>
        public ClipboardWatcher(IntPtr handle)
        {
            hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WndProc);
            this.handle = handle;
        }
        //クリップボード監視開始
        public void Start()
        {
            AddClipboardFormatListener(handle);
        }
        //クリップボード監視停止
        public void Stop()
        {
            RemoveClipboardFormatListener(handle);
        }
    }
}
