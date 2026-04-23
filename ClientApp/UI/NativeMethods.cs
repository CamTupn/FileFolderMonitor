using System;
using System.Runtime.InteropServices;

namespace ClientApp.UI
{
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