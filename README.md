<h1>Villagers vs. Zombies (v0.2.0-alpha)</h1>

In the deepest backwoods, surrounded by dense forests and wild animals, stands a lonely cabin. It belongs to a quirky bunch of hillbillies who spend their simple lives brewing moonshine and telling stories about the undead. One evening, when they make a particularly mysterious find in the woods - an old, dusty gaming table - things take an unexpected turn.

![Unbenannt-2](https://github.com/user-attachments/assets/a62766f2-007e-4275-a305-bd947feb0726)

This table, it turns out, is not just a game. It's a cursed artifact that conjures up waves of zombies that attack everything and everyone for miles around. Soon the hillbillies realize that the only way to survive these zombie hordes is to use the table to build magical defenses and defend themselves.

![Unbenannt-1](https://github.com/user-attachments/assets/75083e37-0ad4-4af4-9b4a-b3b5809b05c4)

But here's the twist: the player is in the middle of the hut, and the hut is also on the playing field - a deceptive cycle that blurs the line between game and reality. When you look out of the windows, you can see your own defenses and the approaching zombies from a different perspective, which reinforces the feeling that you are not just playing, but actually fighting for your survival.

With every round the player survives, the zombie hordes become stronger and the dangers greater. The hillbillies have to improvise all sorts of wacky weapons and traps, from chainsaws to exploding booze barrels, to protect the cabin and their lives. And as they fight their way through the waves, it becomes clear that this game table is more than just a game - it could be the gateway to something much bigger and more terrifying.

In this insane mix of survival, strategy and black humor, players must save their cabin and the weird community of hillbillies while trying not to lose their minds - neither in the real world, nor in the game world.

<h2>How to install</h2>

<h3>1. Import the GitHub repo into Unity</h3>

  <h4>Install GitHub Desktop (if not already done):</h4>
      Download and install GitHub Desktop.

  <h4>Clone GitHub repo:</h4>
      Open GitHub Desktop and click <i>File > Clone Repository</i>.
      Paste the GitHub repo link and select a local location.

  <h4>Open Unity project:</h4>
      Open Unity Hub and click <i>Open.</i>
      Navigate to the folder where you cloned the repo and select the folder with the Unity project file (.sln or Assets and ProjectSettings folder).
      Click <i>Open</i> to load the project into Unity.

<h3>2. Configure build settings for the Quest 3</h3>

  <h4>Install Android Build Support:</h4>
      Open Unity Hub and go to <i>Installs.</i>
      Click on the three dots next to your Unity version and select Add Modules.
      Install Android Build Support, including SDK & NDK Tools and OpenJDK.

  <h4>Customize build settings:</h4>
      In Unity, go to <i>File > Build Settings.</i>
      Select Android as Platform and click on <i>Switch Platform.</i>
      Click on <i>Player Settings</i> and under <i>XR Plug-in Management</i> activate <i>Oculus.</i>

<h3>3. Prepare Quest 3 and install the app</h3>

  <h4>Set Quest 3 to developer mode:</h4>
      Open the Meta Quest app on your smartphone.
      Go to <i>Devices</i>, select your Quest 3 and activate <i>Developer Mode.</i>

  <h4>Connect Quest 3 to the PC:</h4>
      Connect the Quest 3 to your computer via USB-C (USB 3.2).
      Activate <i>USB debugging mode</i> on the Quest 3 when prompted when connecting.

  <h4>Transfer the app to the Quest 3:</h4>
      Back in Unity, click <i>Build and Run</i> in the Build Settings window.
      Select a folder to save the .apk file.
      Unity will build the project and install it directly on your Quest 3.

<h3>Your app should now run on the Quest 3. You can find and start it in the <i>Memory</i> menu under <i>Unknown sources.</i></h3>

<h2>The MIT License</h2>

© 2024 Florian Liebetrau

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
