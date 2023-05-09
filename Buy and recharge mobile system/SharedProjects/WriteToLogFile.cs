using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sedco.SelfService.Kiosk.SharedProject
{
    public static class WriteToLogFile
        {
            public static string CreateFile(string fileName)
            {
                DateTime currentDate = DateTime.Now;
                string root = AppDomain.CurrentDomain.BaseDirectory + "\\logs";
                string monthPath = root + "\\" + string.Format("{0:dd MMMM}", currentDate.Date) + "\\";
                string filePath = monthPath + $"{fileName}.txt";

                if (!File.Exists(filePath))
                {
                    new FileInfo(filePath).Directory.Create();
                }
                return filePath;
            }

            public static void WriteToFile(string content, string filePath)
            {
                StringBuilder storyLog = new StringBuilder();
                storyLog.AppendLine(DateTime.Now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + content);
                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(storyLog.ToString());
                }
            }
            public static void WriteToLogStoryFile(string content)
            {
                string filePath = CreateFile("StoryLog");
                StackTrace stackTrace = new StackTrace();
                StackFrame frame = stackTrace.GetFrame(1);
                string functionName = frame.GetMethod().Name;
                string className = frame.GetMethod().DeclaringType.Name;
                string logs = " {0} {1} {2} ";
                logs = string.Format(logs, className, functionName, content);
                WriteToFile(logs, filePath);
            }

            public static void WriteToLogErrorFile(Exception ex)
            {
                string filePath = CreateFile("ErrorLogs");
                string logs = " {0} {1} \nException Message:{2} {3}";
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame frame = stackTrace.GetFrame(0);
                int line = frame.GetFileLineNumber();
                string currentMethod = frame.GetMethod().Name;
                string className = frame.GetMethod().DeclaringType.Name;
                logs = string.Format(logs, className, currentMethod, ex.Message, ex.StackTrace);
                WriteToFile(logs, filePath);
            }
        }
    }

