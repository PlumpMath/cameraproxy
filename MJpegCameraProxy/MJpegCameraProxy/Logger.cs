﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MJpegCameraProxy
{
	public enum LogType
	{
		HttpServer
	}
	public enum LoggingMode
	{
		None = 0,
		Console = 1,
		File = 2,
		Email = 4
	}
	public class Logger
	{
		public static LoggingMode logType = LoggingMode.Console | LoggingMode.File;
		public static string logFilePath;
		private static object lockObj = new object();
		static Logger()
		{
			string applicationDirectory = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName;
			logFilePath = Path.Combine(applicationDirectory, "MJpegCameraErrors.txt");
		}
		public static void Debug(Exception ex, string additionalInformation = "")
		{
			if (additionalInformation == null)
				additionalInformation = "";
			lock (lockObj)
			{
				if ((logType & LoggingMode.Console) > 0)
				{
					if (ex != null)
						Console.Write("Exception thrown at ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(DateTime.Now.ToString());
					if (ex != null)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(ex.ToString());
					}
					if (!string.IsNullOrEmpty(additionalInformation))
					{
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						if (ex != null)
							Console.Write("Additional information: ");
						Console.WriteLine(additionalInformation);
					}
					Console.ResetColor();
				}
				if ((logType & LoggingMode.File) > 0 && (ex != null || !string.IsNullOrEmpty(additionalInformation)))
				{
					StringBuilder debugMessage = new StringBuilder();
					debugMessage.Append("-------------").Append(Environment.NewLine);
					if (ex != null)
						debugMessage.Append("Exception thrown at ");
					debugMessage.Append(DateTime.Now.ToString()).Append(Environment.NewLine);
					if (!string.IsNullOrEmpty(additionalInformation))
					{
						if (ex != null)
							debugMessage.Append("Additional information: ");
						debugMessage.Append(additionalInformation).Append(Environment.NewLine);
					}
					if (ex != null)
						debugMessage.Append(ex.ToString()).Append(Environment.NewLine);
					debugMessage.Append("-------------").Append(Environment.NewLine);
					int attempts = 0;
					while (attempts < 5)
					{
						try
						{
							File.AppendAllText(logFilePath, debugMessage.ToString());
							attempts = 10;
						}
						catch (Exception)
						{ 
							attempts++;
						}
					}
				}
			}
		}

		public static void Debug(string message)
		{
			Debug(null, message);
		}
		public static void Info(string message)
		{
			if (message == null)
				return;
			lock (lockObj)
			{
				if ((logType & LoggingMode.Console) > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(DateTime.Now.ToString());
					Console.ResetColor();
					Console.WriteLine(message);
				}
				if ((logType & LoggingMode.File) > 0)
				{
					int attempts = 0;
					while (attempts < 5)
					{
						try
						{
							File.AppendAllText(logFilePath, DateTime.Now.ToString() + Environment.NewLine + message + Environment.NewLine);
							attempts = 10;
						}
						catch (Exception)
						{
							attempts++;
						}
					}
				}
			}
		}

		public static void Special(LogType logType, string message)
		{
			if (logType == LogType.HttpServer)
			{
				//Info(message);
			}
		}

		public static SimpleHttp.ILogger httpLogger = new HttpLogger();
	}
	class HttpLogger : SimpleHttp.ILogger
	{
		void SimpleHttp.ILogger.Log(Exception ex, string additionalInformation)
		{
			Logger.Debug(ex, additionalInformation);
		}

		void SimpleHttp.ILogger.Log(string str)
		{
			Logger.Debug(str);
		}
	}
}
