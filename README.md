# VBC2
This is the second Valent Birthday Collab!

## Important Stuff
- Everest Documentation: https://github.com/EverestAPI/Resources/wiki/Code-Mod-Setup
- Need to have
    - An IDE supporting C# (Visual Studio, Visual Studio Code, or JetBrains Rider)
    - Git (https://git-scm.com/downloads)
    - Github desktop (https://desktop.github.com/download/)
    - A Github account (duh)
    - Celeste
    - Everest

## Setting up the repository
For those who haven't ever used github, the concept is pretty simple. There are mainly two operations, you can either push or pull.  pushing sends the changes you've made locally to the repository online, and pulling updates what's on your machine to match what's currently on the repository.

Once you have installed Git and Github desktop, we can set up the repository on your machine.

### Windows

To set up the repository, start by navigating to the directory where Celeste is installed. This can be done either through Steam or Olympus. Once you're in the directory (which should end in `Steam/steamapps/common/Celeste/`), look for the `Mods` folder.  Right click on it and press *Open in Terminal*.  This should open a command line for you.  Now, you'll want to type in the following command:
```
git clone https://github.com/bigboyconst/VBC2
```
This will create folder in your `Mods` directory named `VBC2` containing everything currently on the repository. Now to set this up with GitHub Desktop, you need to open it and click on `File > Add local repository`.  You will receive a pop up telling you to input the path of the repository.  This is where you input the path to the repository you just cloned (which should look like `.../Steam/steamapps/common/Celeste/Mods/VBC2/`).

Now, everything should be properly set up for you to push and pull from the repo.

### MacOS / Linux

For MacOS or Linx, you'll follow the same procedure, except when right clicking on the `Mods` folder, you'll want to click on `Services > New Terminal at Folder`.  Then, copy paste the same command as earlier to clone the repository and follow the same steps for GitHub desktop to be able to push and pull.
