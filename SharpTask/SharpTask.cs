using System;
using Microsoft.Win32.TaskScheduler;

namespace SharpTask
{
	class SharpTask
	{
		//https://docs.microsoft.com/en-us/windows/desktop/taskschd/task-scheduler-objects
		static void Main(string[] args)
		{
			if (args == null || args.Length < 1)
			{
				printUsage();
			}

			if ((args[0] == "--ListAll") && (args.Length == 3))
			{
				string computer = args[1];
				string taskFolder = args[2];
				ListAll(computer, taskFolder);
			}
			else if ((args[0] == "--AddTask") && (args.Length == 7))
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
			else if ((args[0] == "--RemoveTask") && (args.Length == 4))
			{
				string computer = args[1];
				string taskFolder = args[2];
				string taskName = args[3];
				RemoveTask(computer, taskFolder, taskName);
			}
			else if ((args[0] == "--GetRunning") && (args.Length == 2))
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
			Console.WriteLine("\n[-] Usage: \n\t--ListAll <Computer|local|hostname|ip> <Folder|\\|\\Microsoft\\Windows\\AppID>\n\n\t--AddTask <Computer|local|hostname|ip> <24h:time|12:30> <Folder|\\|\\Microsoft\\Windows\\AppID> <Name|VerifiedPublisherSmartScreenCheck> <Description|\"Inspects the AppID cache for invalid SmartScreen entries.\"> <ExecutablePath|C:\\Windows\\notepad.exe> <ExecutableArgs|Optional>\n\n\t--RemoveTask <Computer|local|hostname|ip> <Folder|\\|\\Microsoft\\Windows\\AppID> <Name|VerifiedPublisherSmartScreenCheck>\n\n\t--GetRunning <Computer|local|hostname|ip>") ;
			System.Environment.Exit(1);
		}

		static void ListAll(string computer, string taskFolder)
		{
			var taskService = new TaskService();

			if (computer.ToLower() != "local")
			{
				taskService = new TaskService(computer);
			}

			var existingTasks = taskService.GetFolder(taskFolder).Tasks;

			foreach (var task in existingTasks)
			{
				Console.Write("\nName:\t" + task.Name + "\nDesc:\t" + task.Definition.RegistrationInfo.Description + "\nAction:\t" + task.Definition.Actions + "\nLast:\t" + task.LastRunTime + "\nResult:\t" + task.LastTaskResult + "\nNext:\t" + task.NextRunTime + "\n");
			}

			taskService.Dispose();
		}

		static void AddTask(string computer, int hour, int min, string taskFolder, string taskName, string taskDescription, string actionPath, string actionArgs)
		{
			var taskService = new TaskService();

			if (computer.ToLower() != "local")
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

		static void RemoveTask(string computer, string taskFolder, string taskName)
		{
			var taskService = new TaskService();

			if (computer.ToLower() != "local")
			{
				taskService = new TaskService(computer);
			}

			try
			{
				taskService.FindTask(taskName).Stop();
				taskService.RootFolder.DeleteTask(taskFolder + taskName);
				taskService.Dispose();
			}
			catch (Exception)
			{
				// ignored
			}
		}

		static void GetRunning(string computer)
		{
			var taskService = new TaskService();

			if (computer.ToLower() != "local")
			{
				taskService = new TaskService(computer);
			}

			var runningTasks = taskService.GetRunningTasks(true);

			foreach (var task in runningTasks)
			{
				Console.Write("\nName:\t" + task.Name + "\nDesc:\t" + task.Definition.RegistrationInfo.Description + "\nAction:\t" + task.CurrentAction + "\n");
			}

			taskService.Dispose();
		}
	}
}