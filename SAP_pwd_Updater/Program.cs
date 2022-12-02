using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using FlaUI.UIA2;
using FlaUI.Core.Tools;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using SAP_pwd_Updater;




if (args.Length == 1)
{
    PwdMan pwdMan = new PwdMan(args[0]);
    
    File.WriteAllLines(args[0], pwdMan.UpdatePwdList(pwdMan.PwdList, pwdMan.NextPwd, args[0]));
}
else
{
    PwdMan pwdMan = new PwdMan(args[0]);
    var app = FlaUI.Core.Application.Launch($"C:\\Program Files (x86)\\SAP\\FrontEnd\\SapGui\\saplogon.exe");

    try
    {
        string username = args[1];
        string filtered = args[2];

        using (var automation = new UIA2Automation())
        {
            var desktop = automation.GetDesktop();
            var mainwin = Retry.WhileNull<Window>(() =>
            {
                return desktop.FindFirstChild(cf => cf.ByName("SAP Logon 760")).AsWindow();
            }, timeout: TimeSpan.FromSeconds(10), throwOnTimeout: true, timeoutMessage: "can't get the window");
            if (mainwin.Success)
            {
                var filter = Retry.WhileNull<AutomationElement>(() =>
                {
                    return mainwin.Result.FindFirstChild(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
                }, timeout: TimeSpan.FromSeconds(10), throwOnTimeout: true, timeoutMessage: "can't get the filter");
                if (filter.Success)
                {
                    filter.Result.Focus();
                    NewPWD(filtered, pwdMan.CurrentPwd, pwdMan.NextPwd, username);
                }
                mainwin.Result.Close();
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}


void NewPWD(string filter, string currentpwd, string newpwd, string user)
{
    using (var automation = new UIA2Automation())
    {
        var key = "SUP3RC4L!FR4G!L!ST!C3XP!R4L!D0S0";
        var desktop = automation.GetDesktop();
        Keyboard.Type(filter);
        Keyboard.Type(VirtualKeyShort.ENTER);
        var win = Retry.WhileNull<Window>(() =>
        {
            return desktop.FindFirstChild(cf => cf.ByClassName("SAP_FRONTEND_SESSION")).AsWindow();
        }, timeout: TimeSpan.FromSeconds(10), throwOnTimeout: true, timeoutMessage: "can't get the window");
        if (win.Success)
        {
            Console.WriteLine("got window");
            win.Result.Focus();

            Keyboard.Type(user);
            Keyboard.Type(VirtualKeyShort.TAB);

            Keyboard.Type(PwdMan.DecryptPwd(key, currentpwd));
            Keyboard.Type(VirtualKeyShort.F5);

            var pane = Retry.WhileNull<AutomationElement>(() =>
            {
                return win.Result.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Pane).And(cf.ByAutomationId("101")));
            }, timeout: TimeSpan.FromSeconds(10), throwOnTimeout: true, timeoutMessage: "can't get the pane");

            if (pane.Success)
            {
                Console.WriteLine("got pane inside new pwd");
                pane.Result.Focus();
                Keyboard.Type(PwdMan.DecryptPwd(key, newpwd));
                Keyboard.Type(VirtualKeyShort.TAB);
                Keyboard.Type(PwdMan.DecryptPwd(key, newpwd));
                Keyboard.Type(VirtualKeyShort.ENTER);
            }
            else
            {
                Console.WriteLine("can't reach pane");
            }
        }
    }
}