Start scene file:
Assets/Scenes/MainMenu


How to play:
The objective is to find and defeat zombie bosses to collect supplies needed to move on to the next stage; all while killing herds of zombies and looking for health and ammo kits to stay alive.
The controls map is displayed at the start of the game, which can also be deactivated. Additional information about game tips is provided in the 'INFO' page from the main menu.


Technology requirements:
Character always facing forward following the player's mouse movement. Animations are blended well to smoothly transition character's movement going sideways or backwards.
Camera is smooth and always faces forward even when hitting an obstacle. When camera view can be blocked by an obstacle, camera zooms in to the player to avoid obstacle blocking view.
Interactive scripted doors where they open when player has defeated the relevant bosses and character is located near the door.
Props (crates, barrels, etc.) in the environment are simulated when colliding with a zombie.
Zombie agents are controlled by multiple AI states of behavior (guard, chase, attack, etc.)
Zombies rotate smoothly to face player and also has smooth transition from attacking back to chasing.
Trigger boss zombies upon player entering the boss's point of view. Fluid animation when zombies go from idle to chase.
UI keeps track of player's health, ammunition, and collectables. When dropping the gun, the gun's remaining ammo and magazine are kept in place.
When zombie takes damage from player, it will alert the zombie and chase the player even from far away.
Player is capable of pausing the game and resuming the game. Also, player is given option to restart or quit game upon death.
Mini map provided for players to locate nearby zombies, boss zombies, and collectables.
Damage indicator overlays the screen to notify that the player is being hurt by zombies.
Player is slowed down when stepping on blood spills on the ground. Player can jump over spills to avoid the slow effect.
Camera shakes when player shoots gun. The intensity of the shake gets larger as the player moves (even larger when running)


Known problem areas:
Player is pushed around rapidly when attacked by surrounded zombies.


References:
Footstep sound: https://assetstore.unity.com/packages/audio/sound-fx/foley/footsteps-essentials-189879
Blood splatter: https://assetstore.unity.com/packages/2d/textures-materials/blood-splatter-decal-package-7518
Collectable boxes: https://assetstore.unity.com/packages/3d/props/furniture/boxes-pack-32717
Game font: https://assetstore.unity.com/packages/2d/fonts/fatality-fps-gaming-font-216954
Rifle effects: https://assetstore.unity.com/packages/vfx/particles/war-fx-5669
Rifle: https://assetstore.unity.com/packages/3d/props/guns/free-fps-weapon-akm-180663
Environment: https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-construction-kit-modular-159280
Background Music: https://assetstore.unity.com/packages/audio/music/dynamic-music-35925
Characters: https://www.mixamo.com/