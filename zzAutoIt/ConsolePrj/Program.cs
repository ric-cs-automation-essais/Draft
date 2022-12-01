using System;
using System.Runtime.InteropServices;
using Automation.Infra.AutoIt;

namespace ConsolePrj
{
    public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    static class Program
    {
        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        public static void Main(string[] args)
        {
            new AutoItAutomation();

            //https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
            //https://www.codeproject.com/Articles/28064/Global-Mouse-and-Keyboard-Library

            Console.WriteLine("\n\nOk"); Console.ReadKey();
        }

        //private const int WH_MOUSE = 7;
        //private const int WH_KEYBOARD = 2;

        //UserActivityHook actHook;
        //void MainFormLoad(object sender, System.EventArgs e)
        //{
        //    actHook = new UserActivityHook(); // crate an instance

        //    // hang on events

        //    actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
        //    actHook.KeyDown += new KeyEventHandler(MyKeyDown);
        //    actHook.KeyPress += new KeyPressEventHandler(MyKeyPress);
        //    actHook.KeyUp += new KeyEventHandler(MyKeyUp);
        //}

        //public void MouseMoved(object sender, MouseEventArgs e)
        //{
        //    labelMousePosition.Text = String.Format("x={0}  y={1}", e.X, e.Y);
        //    if (e.Clicks > 0) LogWrite("MouseButton     - " + e.Button.ToString());
        //}
    }


    //class Form1
    //{
    //    static int hHook = 0;
    //    HookProc MouseHookProcedure;

    //    private void ActivateMouseHook_Click(object sender, System.EventArgs e)
    //    {
    //        if (hHook == 0)
    //        {
    //            MouseHookProcedure = new HookProc(Form1.MouseHookProc);
    //            hHook = Program.SetWindowsHookEx(WH_MOUSE,
    //                             MouseHookProcedure,
    //                             (IntPtr)0,
    //                             AppDomain.GetCurrentThreadId());
    //        }
    //    }

    //    public static int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    //    {
    //        //Marshall the data from the callback.
    //        MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

    //        if (nCode >= 0 &&
    //            MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
    //        {
    //            // Do something here
    //        }
    //        return CallNextHookEx(hookHandle, nCode, wParam, lParam);
    //    }
    //}
}
