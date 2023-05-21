## PingPong Game.

The game has to modes: 
- Hotseat
	Playing on the single device.
- Multiplayer.
	Playing on two different devices.

The game is intended to be launched in portrait orientation.
Tested resolutions:
- 720x1280
- 1440x2560

The networking part is implemented using Photon True Sync. Internet connection is required for multiplayer, so local multiplayer is not available.

The paddles are controlled by dragging on the paddle.

To launch the game in the Unity editor, select the Lobby scene and press Play. So, the Lobby scene is the main scene.

The game balance configuration is done through ScriptableObjects located in the Assets/Resources/Defs directory.
The GameDef file contains colors for the enemy platform and the player's platform. It also contains a set of configurations for different balls.
The BallDef file contains settings for size, speed, color, and allows selecting the ball's direction generation algorithm.

Ball direction generation algorithms:
- RandomDirectionGenerator - generates initial direction of the ball completely randomly.
- SpreadDirectionGenerator - generates initial direction of the ball in the direction of random player with spread of SpreadAngle.

You can download the latest version for Android from the following link: https://bitbucket.org/sollinux/truepong/downloads/TruePong.apk
