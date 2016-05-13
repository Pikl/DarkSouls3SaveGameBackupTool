# Dark Souls III Super Backup Tool!
![DarkSouls3SuperBackupTool](https://github.com/pikl/DarkSouls3SuperBackupTool/blob/master/Images/Main Window.png)

### What is?
A handy tool that creates backups for your Dark Souls III saves, highly configurable and very easy to use.
Based off the original backup tool created by [insane0hflex](https://github.com/insane0hflex).

## NOTE : 
### This Readme does not yet accurately represent the application.
### The Super Backup Tool is currently in development, it is not recommended to use it in it's current state.

### What do?
+ Creates backups every x seconds, or immediately when your saves are changed
+ Maximum backup limit, deletes oldest backups when limit reached (size or quantity)
+ Settings are stored in BackupConfig.xml within the Dark Souls AppData folder
+ Backs up all profiles if there are more than one (Steam accounts, GOG etc.)
+ Doesn't create backups if Dark Souls has not modified any saves
+ Doesn't create backups if Dark Souls is not running
+ Can start backing up immediately on startup
+ Quick 'n Easy backup loading UI, with thumbnails for each backup

### How do?
The application will work without any prior configuration.
Open the application and click the 'Start' button to start making a backup every 15 minutes.
When you are done playing, press the "Stop" button.

To restore a backup, click 'Restore a Backup'.
To restore to the previous backup, click 'Restore previous'.
Otherwise, scroll through the available backups listed, select one and click 'Restore'.

#### Configuration
Every feature is configurable through the 'Settings' button.
+ Option to backup every x minutes or seconds, or have the application detect any change made by Dark Souls III immediately.
+ Option to have a maximum backup limit, specified by quantity of backups, or size of backups. The oldest backups will be deleted to make room for the new ones. Use '0' for infinite backups.
+ All of these settings are stored in '''AppData/Roaming/DarkSoulsIII/BackupConfig.xml'''.
+ Supports multiple Dark Souls profiles, for people who have multiple Steam accounts or use more than one launcher, such as GOG.
+ Doesn't create backups if Dark Souls III has not modified any saves, or if Dark Souls III is not running. Can be turned on anyway.
+ Toggle option to start the backup process immediately on application startup.
+ Option to store screenshot thumbnails of Dark Souls III at the time of backup.
+ Option to stop backing up and close the backup tool when Dark Souls III is closed.

### What else should I know?
Back ups are created in the following format:
`Month-Day-Year_Hour-Minute-Second_DS30000.sl2`

### Releases
[Version 1.0](https://github.com/pikl/DarkSouls3SuperBackupTool/releases/blah)