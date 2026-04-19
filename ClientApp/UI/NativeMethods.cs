using System;
using System.Runtime.InteropServices;

namespace ClientApp.UI
{
	// Helper gọi Win32 API để nhấp nháy icon trên Taskbar khi có thông báo mới
	internal static class NativeMethods
	{
		[DllImport("user32.dll")]
		private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

		public static void FlashWindow(IntPtr handle)
		{
			FlashWindow(handle, true);
		}
	}
}