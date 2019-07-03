using System;
using Microsoft.Win32.TaskScheduler;

namespace SharpTask
{
	class SharpTask
	{
		// https://docs.microsoft.com/en-us/windows/desktop/taskschd/task-scheduler-objects
		static void Main(string[] args)
		{
			if (args == null || args.Length < 1)
			{
				printUsage();
			}

			if ((args[0].ToUpper() == "--LISTALL") && (args.Length == 3))
			{
				string computer = args[1];
				string taskFolder = args[2];
				ListAll(computer, taskFolder);
			}
			else if ((args[0].ToUpper() == "--ADDTASK") && (args.Length == 7))
			{
				string computer = args[1];
				string[] time = args[2].Split(':');
				int hour = int.Parse(time[0]);
				int minute = int.Parse(time[1]);
				string taskFolder = args[3];
				string taskName = args[4];
				string taskDescription = args[5];
				string actionPath = args[6];
				if (args.Length == 8)
				{
					string actionArgs = args[8];
					AddTask(computer, hour, minute, taskFolder, taskName, taskDescription, actionPath, actionArgs);
				}
				else
				{
					AddTask(computer, hour, minute, taskFolder, taskName, taskDescription, actionPath, null);
				}
			}
			else if ((args[0].ToUpper() == "--REMOVETASK") && (args.Length == 4))
			{
				string computer = args[1];
				string taskFolder = args[2];
				string taskName = args[3];
				RemoveTask(computer, taskFolder, taskName);
			}
			else if ((args[0].ToUpper() == "--GETRUNNING") && (args.Length == 2))
			{
				string computer = args[1];
				GetRunning(computer);
			}
			else
			{
				printUsage();
			}
		}

		static void printUsage()
		{
			Console.WriteLine("\n[-] Usage: \n\t--ListAll <Computer|local|hostname|ip> <Folder|\\|\\Microsoft\\Windows\\AppID>" +
				"\n\n\t--AddTask <Computer|local|hostname|ip> <24h:time|12:30> <Folder|\\|\\Microsoft\\Windows\\AppID> <Name|VerifiedPublisherSmartScreenCheck>" +
				"<Description|\"Inspects the AppID cache for invalid SmartScreen entries.\"> <ExecutablePath|C:\\Windows\\notepad.exe> <ExecutableArgs|Optional>" +
				"\n\n\t--RemoveTask <Computer|local|hostname|ip> <Folder|\\|\\Microsoft\\Windows\\AppID> <Name|VerifiedPublisherSmartScreenCheck>" +
				 "\n\n\t--GetRunning <Computer|local|hostname|ip>") ;
			System.Environment.Exit(1);
		}

		static void ListAll(string computer, string taskFolder)
		{
			try
			{
				var taskService = new TaskService();

				if (computer.ToUpper() != "LOCAL")
				{
					taskService = new TaskService(computer);
				}

				var existingTasks = taskService.GetFolder(taskFolder).Tasks;

				foreach (var task in existingTasks)
				{
					Console.Write("\nName:\t{0}\nDesc:\t{1}\nAction:\t{2}\nLast:\t{3}\nResult:\t{4}\nNext:\t{5}\n", task.Name, task.Definition.RegistrationInfo.Description, task.Definition.Actions, task.LastRunTime, task.LastTaskResult, task.NextRunTime);
				}

				taskService.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
				return;
			}

		}

		static void AddTask(string computer, int hour, int min, string taskFolder, string taskName, string taskDescription, string actionPath, string actionArgs)
		{
			try
			{
				var taskService = new TaskService();

				if (computer.ToUpper() != "LOCAL")
				{
					taskService = new TaskService(computer);
				}

				var taskDefinition = taskService.NewTask();
				taskDefinition.RegistrationInfo.Description = taskDescription;
				taskDefinition.Principal.UserId = "SYSTEM";

				var trigger = new DailyTrigger
				{
					StartBoundary = DateTime.Today + TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(min)
				};

				taskDefinition.Triggers.Add(trigger);

				taskDefinition.Actions.Add(new ExecAction(actionPath, actionArgs));

				taskDefinition.Settings.Enabled = true;
				taskDefinition.Settings.Hidden = false;
				taskDefinition.Settings.WakeToRun = true;
				taskDefinition.Settings.AllowDemandStart = true;
				taskDefinition.Settings.StartWhenAvailable = true;
				taskDefinition.Settings.DisallowStartIfOnBatteries = false;
				taskDefinition.Settings.DisallowStartOnRemoteAppSession = false;
				taskDefinition.Settings.StopIfGoingOnBatteries = false;

				taskService.RootFolder.RegisterTaskDefinition(taskFolder + taskName, taskDefinition);
				taskService.FindTask(taskName).Run();
				taskService.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
				return;
			}

		}

		static void RemoveTask(string computer, string taskFolder, string taskName)
		{
			try
			{
				var taskService = new TaskService();

				if (computer.ToUpper() != "LOCAL")
				{
					taskService = new TaskService(computer);
				}

				taskService.FindTask(taskName).Stop();
				taskService.RootFolder.DeleteTask(taskFolder + taskName);
				taskService.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
				return;
			}
		}

		static void GetRunning(string computer)
		{
			try
			{
				var taskService = new TaskService();

				if (computer.ToUpper() != "LOCAL")
				{
					taskService = new TaskService(computer);
				}

				var runningTasks = taskService.GetRunningTasks(true);

				foreach (var task in runningTasks)
				{
					Console.Write("\nName:\t{0}\nDesc:\t{1}\nAction:\t{2}\n", task.Name, task.Definition.RegistrationInfo.Description, task.CurrentAction);
				}

				taskService.Dispose();
			}
			catch (Exception e)
			{
				Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
				return;
			}

		}
	}
}