using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using System.Diagnostics;

namespace WebApplication1.Models
{
    public class WindowUtils
    {
        #region user32
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextLength")]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowText")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
        [DllImport("User32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32.dll", EntryPoint = "GetDlgItem")]
        private static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassName")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumChildWindows")]

        private static extern int EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "GetWindowThreadProcessId")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", EntryPoint = "mouse_event")]
        private static extern IntPtr mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("User32.dll", EntryPoint = "GetWindowRect")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("User32.dll", EntryPoint = "SetCursorPos")]
        private static extern IntPtr SetCursorPos(int dx, int dy);
        #endregion

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        private const int BM_CLICK = 0x00F5;

        private const int maxClassNameLength = 256;

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private static List<WindowsInfo> windowsList;

        private struct ElementsList
        {
            public List<Element> Elemlst;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
            {
                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder titlebuilder = new StringBuilder(length);
                GetWindowText(hWnd, titlebuilder, length + 1);
                windowsList.Add(new WindowsInfo { Handle = hWnd, Title = titlebuilder.ToString() });
            }

            return true;
        }

        private static bool EnumChildProcCallback(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder namebuilder = new StringBuilder(maxClassNameLength);
            GetClassName(hWnd, namebuilder, maxClassNameLength);

            ElementsList elementsList = (ElementsList)GCHandle.FromIntPtr(lParam).Target;
            elementsList.Elemlst.Add(new Element { Handle = hWnd, ClassName = namebuilder.ToString() });

            return true;
        }

        private static List<AutomationElement> GetAllElementsInChromeWindow(int processid)
        {
            Process process = Process.GetProcessById(processid);
            List<AutomationElement> elements = new List<AutomationElement>();

            if (process.MainWindowHandle != IntPtr.Zero)
            {
                AutomationElement mainWindowElement = AutomationElement.FromHandle(process.MainWindowHandle);
                Condition condition = new PropertyCondition(AutomationElement.ProcessIdProperty, processid);
                AutomationElementCollection mainWindowElements = mainWindowElement.FindAll(TreeScope.Descendants, condition);

                foreach (AutomationElement ele in mainWindowElements)
                {
                    elements.Add(ele);
                }
            }

            return elements;
        }

        #region public method
        public static List<WindowsInfo> GetAllWindows()
        {
            windowsList = new List<WindowsInfo>();
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
            return windowsList;
        }

        public static List<Element> GetAllSubWindows(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentException($"window with handle '{windowHandle}' not found");
            }

            ElementsList elementsList = new ElementsList { Elemlst = new List<Element>() };
            EnumChildWindows(windowHandle, EnumChildProcCallback, GCHandle.ToIntPtr(GCHandle.Alloc(elementsList)));

            return elementsList.Elemlst;

        }


        public static List<Element> GetAllElements(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentException($"window with handle '{windowHandle}' not found");
            }

            List<IntPtr> elemHandles = new List<IntPtr>();
            List<Element> res = new List<Element>();
            IntPtr elemHandle = IntPtr.Zero;

            do
            {
                elemHandle = FindWindowEx(windowHandle, elemHandle, null, null);

                if (elemHandle != IntPtr.Zero)
                {
                    elemHandles.Add(elemHandle);
                }
            }
            while (elemHandle != IntPtr.Zero);

            if (elemHandles.Count > 0)
            {
                foreach (IntPtr handle in elemHandles)
                {
                    StringBuilder classnamebuilder = new StringBuilder(maxClassNameLength);
                    GetClassName(handle, classnamebuilder, classnamebuilder.Capacity);

                    StringBuilder textbuilder = new StringBuilder(maxClassNameLength);
                    GetWindowText(handle, classnamebuilder, classnamebuilder.Capacity);

                    res.Add(new Element { Handle = handle, ClassName = classnamebuilder.ToString(), Text = textbuilder.ToString() });
                }
            }

            return res;
        }

        public static int getProcessIdByWindowHandle(IntPtr windowHandle)
        {
            int processid = -1;
            GetWindowThreadProcessId(windowHandle, out processid);
            return processid;
        }

        public static List<Element> DisplayAllELementsFromChromeWindow(int processid)
        {
            List<AutomationElement> elements = GetAllElementsInChromeWindow(processid);

            List<Element> res = new List<Element>();

            foreach (AutomationElement ele in elements)
            {
                res.Add(new Element { ClassName = ele.Current.LocalizedControlType, Text = ele.Current.Name });
            }

            return res;
        }

        public static void SetXunleiDownloadDialogToFrontAndClickSubmit(IntPtr handle)
        {
            SetForegroundWindow(handle);
            RECT windowRect;
            GetWindowRect(handle, out windowRect);

            int x = windowRect.Left + (windowRect.Right - windowRect.Left) / 2;
            int y = windowRect.Bottom - (windowRect.Bottom - windowRect.Top) / 8;

            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        #endregion
    }
}