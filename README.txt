This project was last tested with Unity 5.3.5f1.

The version of Node_Editor used is available at https://github.com/Baste-RainGames/Node_Editor/tree/7abc6350ddecf047dafef410f097a6377c88e038.

A couple of tweaks were implemented in order to support additional functionality and are not available in the original repository:
- Framework/ConnectionTypes.cs
	-- colored knobs based on input/output type
	-- colored transitions based on output value 

- Framework/NodeTypes.cs
	-- reordering of the node selection menu

In order to use the editor Unity must be in "Play Mode" at all times.

The Node Editor can be started from Window > Node Editor, to create and edit existing canvas.
The Runtime Node Editor can be shown in the game by pressing F12, but is read-only.