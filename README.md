# Questions?
Feel free to get in contact regarding any issues or questions regarding the API and Abathur.
I develop on a Windows computer, so there might be some Linux/Mac related issues! Please flag them.
=> https://www.linkedin.com/in/schmidthansen/ or schmidtgit at protonmail.com

# SC2Abathur
SC2Abathur is a reboot of Abathur - a modularized AI framework for StarCraft II.
![AbathurFramework](https://i.imgur.com/uTU6eKT.gif)
[![forthebadge](https://forthebadge.com/images/badges/contains-technical-debt.svg)](https://adequatesource.com/)


## Launching for the first time!
    - A directory for settings and cached data (/data) will be created at the current path
    - A directory for log files will be created (/log)
    - The directory will be populated with a gamesettings.json and a setup.json
    - A StarCraft II client will launch to fetch game data from, which will be cached at /data/essence.data
## Turn features on and off
    The framework comes pre-packed with modules that solve regular problems.
    Turn these modules on/off by adding/removing them from the generated setup.json file, examples:
    - AutoHarvestGather (force idle workers to mine minerals and assign vespene workers)
    - AutoSupply (queue a supply depot/overlord/pulon when supply capped and at less than 200 supply)
    - AutoRestart (start a new game when the current one ends)
## Write your own...
    - Create a new module (at /Modules) and implement the IModule interface
    - Open the generated setup.json file and add the classname of your module
    - Launch the framework and watch it go!

# The IModule Interface
The Abathur Framework is based around modules interacting with the core functionality of the framework. The idea is that each module should be small and sastify a simple task, eg. making sure there is enough supply or that workers are never idle. The hope is that enough small modules can form a powerful opponent on the battlefield!

## Access the core functionality
The core functionality in the abathur framework is accessed through 'managers' e.g. the IntelManager. These can be accessed through dependency injection. See the /Modules/FullModule.cs

### Initialize()
Called once - after the framework has successfully connected to the StarCraft II client.
The game state as not been fetched yet, as the game is not launched yet.

### OnStart()
Called once - after the framework has succesfully connected to the StarCraft II client.

### OnStep()
Called each frame (step-mode) or as often as possible (real-time).
Not called in the initial frame - use OnStart() instead.

### OnGameEnded()
Called when the game have ended, but before the framework leave the game (or restart)

# Feedback?
This framework is still being developed! I would appreciate feedback! Keep updated on the progress at AdequateSource.com

Made with ❤ at the IT University of Copenhagen by [SchmidtGit](https://github.com/schmidtgit) & [MikkelLam](https://github.com/mikkellam)
