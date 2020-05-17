# Neon Wonderland
# Design
## Overview
Neon Wonderland is a first-person parkour game from the perspective of the White Rabbit. The mechanics of the game revolve around speed-running each level to beat the clock since the rabbit is late for an important date! The mechanics of the game revolve around parkour, with the player using various mechanics to reach the goal in the fastest way possible. The story of the game and the controls are given to the player through the use of text prompts throughout each of the courses.

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon1.png"> 

<i>An example of some of the jumping puzzles in the game</i>

Throughout development, I tried to keep to a theme. Around halfway through, I was experimenting with Unity’s post-process effects and decided that a neon theme would be the way to go. Since then, I decided to home in on this theme, creating a custom VHS-style shader and overlaying a broken VHS video clip over the game’s camera. This helps sell the effect that the player is stuck in this 80’s-style ‘Neon Wonderland’.  

## Game Rules and User Interactions
As stated above, the gameplay revolves around parkour mechanics, which are gradually explained to the player as they play the demo more and more. On the first level, the player is introduced to movement using the WASD keys, using the mouse to look around, jumping and wall-running. Wall-running is my favourite mechanic in the game and got lots of positive feedback when asking colleagues to playtest. 

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon2.png">  

When wall-running, the player can reach positions that are farther away than if they just jumped. On top of this, the camera tilts, to give the player a better idea of the direction they are going in.
Level 2 continues the idea of gradually introducing game mechanics, beginning with a ladder puzzle, to introduce the player to ladders in the game. 

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon3.png">  

<i>Ladders can be climbed by walking or jumping onto them and pressing W. The player can jump back at any time by pressing Space</i>

When they get to the top, the player is introduced to crouching and sliding, with archways. Crouching can be done when standing, but to slide, the player must be sprinting by pressing Shift. 

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon4.png">

<i>The player can choose to crouch and walk under these archways by standing still and pressing C, but they can get under them faster if they sprint and press C.</i>

Also introduced in Level 2 is Vaulting. Vaulting occurs when the player jumps close to small walls. This allows the player to continue moving without losing their momentum. 

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon5.png"> 

<i>Specific vaults are indicated to the player using Pink walls, but the player can vault over any wall that is 2 blocks or below in the game.</i>

Level 3 is based around the grappling hook mechanic. 

<img src="http://www.tomdotscott.com/images/Github/NeonWonderland/Neon6.png"> 

<i>The grappling hook can be picked up by the player by walking into it.</i>

# Implementation
## Game Features Developed
All mechanics of the game were built from scratch, with a custom first-person controller, comprising of 3 parts: the PlayerController class, the PlayerInput class and the PlayerMovement class. Rather than using physics for movement, to give more control in terms of developing the mechanics and to give more controls to the player, to feel more natural to them, the CharacterController component’s Move() function was used. This allows for more complex movement using movement deltas.  
To detect whether the player is to do any special movements, the PlayerController class has a reference to each specific LayerMask. Physics Raycasts are then used to detect if the player can do any of the specific movements. For instance, to be able to wall-run:
```CSharp
if (Physics.Raycast(transform.position + (transform.right * (wall * radius)), transform.right * wall, out var hit, halfradius, wallrunLayer))
{
wallDir = wall;
       wallNormal = Vector3.Cross(hit.normal, Vector3.up) * -wallDir;
       State = MovementState.WallRunning;
}
```
To detect if there is a wall to run along, a raycast is sent out to either the left or the right, depending on the value of the variable ‘wall’. This variable’s value is set depending on the Boolean method HasWallToSide(int dir), where the parameter is -1 or 1 depending on the direction.

On top of this, a custom Shader was made to sell the VHS look of the game, and a VHSEffect class was made to combine this shader with the video overlay that is on the main camera. 

The GameManager and SoundManager classes both use the Singleton design pattern. This is beneficial as it ensures that there is only ever one instance of the class in the game and provides a global point of access to the instance.   

## Known Issues / Bugs and Possible Solutions
When giant, the white walls sometimes do not smash on the first contact. I believe this is an issue with the OnTriggerEnter() function when used in conjunction with Unity’s CharacterController component. This can be fixed if a different approach is taken, using raycasts as in the PlayerController class to detect if there is a breakable wall within a certain range of the player and then a further check for if the player is giant. 

The timer sometimes remains visible in the menus. This can be fixed by making the visibility code more complex, since now it is visible when the timescale is 1 and invisible when the timescale is 0.

Sometimes the growing and shrinking sound effects play more than once whilst the animation is still happening. This can be rectified potentially by adding further checks to make sure that it only plays once. 
# Future Improvements
I would like to develop this game further in the future, adding more gameplay features and more levels to the game. One of the things I would like to do is make the level structure less linear. This will help make the gameplay more interesting to the player. 
I would also like to expand the game adding a full-body player model, making it animated as well. This would make the entire game more immersive for the player. 
Mechanics that I would like to add to the game include balancing, momentum, the use of tools to scale walls, a double jump, combat, ziplines, and others based on the games I used for inspiration – Mirror’s Edge, Dying Light and Jedi: Fallen Order. 
# External Assets
Main logo text - https://indieground.net/product/road-rage-font/
Paused Menu Text - https://www.dafont.com/cyberspace-raceway.font
Paused Menu text - https://indieground.net/product/neon-80s-font/
Main Menu music - https://www.youtube.com/watch?v=qptMg0JWmFY&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=1
Level 1 Music - https://www.youtube.com/watch?v=ACvtdUeiQ9U&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=7
Level 2 Music - https://www.youtube.com/watch?v=wWyf7viDRC4&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=10
Level 3 Music - https://www.youtube.com/watch?v=egKdVELkKVI&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=26
Level 4 Music - https://www.youtube.com/watch?v=CbR3VOwQOA4&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=51
Level 5 Music - https://www.youtube.com/watch?v=ZyVq4HF8oRw&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=6
Credits Music - https://www.youtube.com/watch?v=zmynVSBSLI0&list=PLGfusIXrWRHKJMo_4wa-2Mhf7JDzCcEj3&index=8
VHS Video Overlay taken from - https://www.youtube.com/watch?v=qELSSAspRDI
Palm Trees - https://vectorified.com/palm-tree-png-vector
Hills - https://www.artstation.com/artwork/q6xge
3D Assets - https://assetstore.unity.com/packages/3d/environments/low-poly-pack-94605
Skybox - https://assetstore.unity.com/packages/3d/environments/sci-fi/real-stars-skybox-lite-116333
Grapple gun model - https://www.turbosquid.com/FullPreview/Index.cfm/ID/1237576
Mad Hatter Model – https://www.cgtrader.com/items/868723/download-page
Teapot Model – https://free3d.com/3d-model/teapot-7341.html
Pocket Watch model – https://www.cgtrader.com/items/769058/download-page
Playing card image  – https://i.pinimg.com/originals/15/e4/3a/15e43a712ac44407d23c437b0a5b43bc.png
Cheshire cat model - https://www.stlfinder.com/model/cheshire-cat-mf0Ixedo/7691798/
Potion Bottle - https://sketchfab.com/3d-models/potion-bottle-45e5d63bbd874a9d859d9ee09704b4c2#download
Other Textures - https://3dtextures.me/
Mad Hatter laugh - https://www.youtube.com/watch?v=JFqdUTzDJEs
Cheshire Cat line - https://www.youtube.com/watch?v=fFBWQ6T9gFc
Teapot song - https://www.youtube.com/watch?v=msvOUUgv6m8
Item Pick Up noise - https://freesound.org/people/suntemple/sounds/253172/
Tea Party picture - https://www.gettyimages.co.uk/detail/news-photo/illustration-by-sir-john-tenniel-watercolour-by-gertrude-news-photo/1162590205?adppopup=true
Sound Effects - https://www.fesliyanstudios.com/royalty-free-sound-effects-download/spells-and-power-ups-217
