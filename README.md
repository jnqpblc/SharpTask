# SharpTask
SharpTask is a simple code set to interact with the Task Scheduler service API using the same DCERPC process as schtasks, which open with TCP port 135 and is followed by the use of an ephemeral TCP port. This code is compatible with Cobalt Strike.

```
C:>SharpTask.exe

[-] Usage:
        --ListAll <Computer|local|hostname|ip> <Folder|\|\Microsoft\Windows>

        --AddTask <Computer|local|hostname|ip> <24h:time|12:30> <Folder|\|\Microsoft\Windows> <Name|VerifiedCheckboxUnchecker> <Description|"Inspects stuff..."> <ExecutablePath|C:\Windows\notepad.exe> <ExecutableArgs|Optional>

        --RemoveTask <Computer|local|hostname|ip> <Folder|\|\Microsoft\Windows> <Name|VerifiedCheckboxUnchecker>

        --GetRunning <Computer|local|hostname|ip>
```
# 1 Add a task on a remote system.
```
beacon> execute-assembly SharpTask.exe --AddTask workstn.foo.local 12:30 \ Test "Testing This Thing" C:\Windows\notepad.exe
beacon> 
```
# 2 Verify that the task was created.
```
beacon> execute-assembly SharpTask.exe --ListAll workstn.foo.local \

Name:   GoogleUpdateTaskMachineCore
Desc:   Keeps your Google software up to date. If this task is disabled or stopped, your Google software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Google software using it.
Action: C:\Program Files\Google\Update\GoogleUpdate.exe /c
Last:   6/29/2019 8:46:33 AM
Result: 0
Next:   6/30/2019 8:46:33 AM

Name:   GoogleUpdateTaskMachineUA
Desc:   Keeps your Google software up to date. If this task is disabled or stopped, your Google software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Google software using it.
Action: C:\Program Files\Google\Update\GoogleUpdate.exe /ua /installsource scheduler
Last:   6/29/2019 9:46:34 PM
Result: 0
Next:   6/29/2019 10:46:33 PM

Name:   Test
Desc:   Testing This Thing
Action: C:\Windows\notepad.exe
Last:   6/29/2019 10:17:54 PM
Result: 267009
Next:   6/30/2019 12:30:00 PM
```
# 3 Verify that the task is running.
```
beacon> execute-assembly SharpTask.exe --GetRunning workstn.foo.local

Name:   SystemSoundsService
Desc:   System Sounds User Mode Agent
Action: Microsoft PlaySoundService Class

Name:   MsCtfMonitor
Desc:   TextServicesFramework monitor task
Action: MsCtfMonitor task handler

Name:   CacheTask
Desc:   Wininet Cache Task
Action: Wininet Cache task object

Name:   Test
Desc:   Testing This Thing
Action: C:\Windows\notepad.exe
```
# 4 Stop and remove the task.
```
beacon> execute-assembly SharpTask.exe --RemoveTask workstn.foo.local \ Test
beacon> 
```
# 5 Verify that the task was removed.
```
beacon> execute-assembly SharpTask.exe --ListAll workstn.foo.local \

Name:   GoogleUpdateTaskMachineCore
Desc:   Keeps your Google software up to date. If this task is disabled or stopped, your Google software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Google software using it.
Action: C:\Program Files\Google\Update\GoogleUpdate.exe /c
Last:   6/29/2019 8:46:33 AM
Result: 0
Next:   6/30/2019 8:46:33 AM

Name:   GoogleUpdateTaskMachineUA
Desc:   Keeps your Google software up to date. If this task is disabled or stopped, your Google software will not be kept up to date, meaning security vulnerabilities that may arise cannot be fixed and features may not work. This task uninstalls itself when there is no Google software using it.
Action: C:\Program Files\Google\Update\GoogleUpdate.exe /ua /installsource scheduler
Last:   6/29/2019 9:46:34 PM
Result: 0
Next:   6/29/2019 10:46:33 PM
```
