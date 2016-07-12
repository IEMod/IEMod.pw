using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Patchwork.Attributes;
using UnityEngine;

namespace ConsoleCommands.pw
{
    [ModifiesType("CommandLine")]
    public class mod_CommandLine
    {
        [NewMember]
        public static void CheckAchievements()
        {
            if (AchievementTracker.Instance.DisableAchievements == true)
            {
                global::Console.AddMessage("Your achievements were previously disabled for this playthrough.", Color.red);
                global::Console.AddMessage("To reactivate them, type: ReenableAchievements");
            }
            else
                global::Console.AddMessage("Your achievements are doing fine.", Color.green);
        }
    }
    /*
        [ModifiesType("CommandLineRun")]
        public class mod_CommandLineRun
        {
            // TJH 8/26/2015 - It's no longer necessary to override RunCommand. We can just make sure all methods are always
            // available and not treated as cheats

            [ModifiesMember("MethodIsAvailable")]
            public static bool MethodIsAvailable(MethodInfo method)
            {
                return true;
            }

        }
    */

    [ModifiesType("CommandLineRun")]
    public class mod_CommandLineRun
    { 
        [ModifiesMember("RunCommand")]
        public static void RunCommand(string command)
        {
            object[] objArray;
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            if (command.ToLower() == "runcommand")
            {
                return;
            }
            IList<string> strs = StringUtility.CommandLineStyleSplit(command);
            bool flag = false;
            bool flag1 = false;
            string empty = string.Empty;
            IEnumerator<MethodInfo> enumerator = CommandLineRun.GetAllMethods().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MethodInfo current = enumerator.Current;
                    if (string.Compare(current.Name, strs[0], true) != 0)
                    {
                        continue;
                    }
                    /*
                    if (!CommandLineRun.MethodIsAvailable(current))
                    {
                        flag = true;
                    }
                    */
                    else if (!CommandLineRun.FillMethodParams(current, strs, out objArray, out empty))
                    {
                        flag1 = true;
                    }
                    else
                    {
                        current.Invoke(null, objArray);
                        return;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            if (flag1)
            {
                Console.AddMessage(string.Concat("Command or script '", strs[0], "' parameter error: ", empty), Color.yellow);
            }
            else if (!flag)
            {
                Console.AddMessage(string.Concat("No command or script named '", strs[0], "' exists."), Color.yellow);
            }
            else
            {
                Console.AddMessage(string.Concat("The command or script '", strs[0], "' is not available at this time."), Color.yellow);
            }
        }
    }
}
