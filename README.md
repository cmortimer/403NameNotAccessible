# Project MacGuffin Quest (403NameNotAccessible)
IGM Production Studio Project

As of now, the game can only be run in the Unity Editor, due to some issues with the build finding files we use for saving and loading data.

The game starts out at the Main Menu scene, which acts as a hub for all interaction outside of game combat.
From here, the player can access the 'Tavern', 'Workshop', or 'Headout' to initiate the main game interactions.
The Tavern allows the player to access their Guild, which is a list of all the characters they have recruited to fight for them.
From there, the player can recruit or remove Guild members, or access and change their equipment to make them stronger.
The Workshop allows the player to craft various equipment, which acts as the sole form of progression in the game.
In the workshop, the player will find various recipes and the requirements to craft them, as well as the ability to craft them.
The Headout option allows the player to select various locations of increasing difficulty, which will generate random dungeons using specific sets of enemies to match the location.

When you Headout, you will be loaded into a randomly generated dungeon that consists of multiple 'rooms' (5x5 sets of tiles).
For each room generated, there is a chance that an enemy will spawn.
In every dungeon, there will be one blue colored tile that represents the end point.
Moving onto this tile will either load you into the next floor of the dungeon, or back to the main menu depending on the dungeon you chose.

Combat in the game is turn based on a grid, with action points. The turns are set up simply as a player and enemy turn, allowing you to take actions in whatever order you desire.
You can select players and enemies with your mouse to view their various stats, as well as follow up by moving, attacking, or choosing to make the target inactive to end turn early.
By default, selecting a player will put you into move mode, which is also accessible by hitting the 1 key. Attack mode is accesible by hitting the 2 key. Setting a character to inactive can be done by hitting the 3 key. Hitting 3 while the character is inactive will make them active again.
In the attack and move modes, tiles on the grid will be highlighted to show the area that you can move to or reach with an attack.
