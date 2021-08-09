using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PersonalNewsSiteSupportTool.ViewModels
{
    /// <summary>
    /// クリップボード監視クラス。
    /// </summary>
    public class ClipboardWatcher
    {

        /// <summary>
        /// クリップボードが変更された時のメッセージコード
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        private readonly IntPtr handle;

        /// <summary>
        /// WPFからWin32APIにアクセスするためのハンドル。
        /// </summary>
        private readonly HwndSource hwndSource;

        /// <summary>
        /// クリップボード更新イベントハンドラー
        /// </summary>
        public event EventHandler UpdateClipboard;
        //イベント起動
        private void RaiseUpdateClipboard()
        {
            UpdateClipboard?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ウィンドウプロシージャー。
        /// </summary>
        /// <param name="hwnd">ウィンドウハンドル</param>
        /// <param name="msg">メッセージ</param>
        /// <param name="wParam">メッセージ固有の付加情報</param>
        /// <param name="lParam">メッセージ固有の付加情報</param>
        /// <param name="handled">ハンドルフラグ</param>
        /// <returns>メッセージを処理した結果</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                this.RaiseUpdateClipboard();
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
        ///<summary>クリップボード監視開始</summary>
        public void Start()
        {
            _ = NativeMethods.AddClipboardFormatListener(handle);
        }
        ///<summary>クリップボード監視停止</summary>
        public void Stop()
        {
            _ = NativeMethods.RemoveClipboardFormatListener(handle);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool AddClipboardFormatListener(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    }
}
