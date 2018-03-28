=== DESCRIPTION ===

Customizable Animated Glowing Orbs gives you a toolkit for making orbs, animated using procedural noise. Light source automatically changes color based on the colors of the orb.

Very easy to use.

There are 11 different variables that let you change the appearance and the behavior of the orbs you create directly from the inspector.

The following are the variables that you can change:

- Size of the core
- Thickness of the glow
- Color of the core
- Color of the edges of the core
- Color of the glow
- Color of the produced noise
- Animation of noise
- Amount of noise
- Amplitude of noise
- Light intensity
- Relative light range

All variables are easily accessible and editable through scripts as well. Source code included.


=== USAGE ===

To use, simply drag one of the prefabs in the <Orb/Prefabs> directory into your scene. All of them are the same, they just have different pre-defined values for the variables.

Once you have it somewhere in the scene, click on it, then go to the Inspector and tweak the variables under the tab "Orb" to your taste.

An example scene is included in <Orb/Example/Test scene.unity>.

A code example is included in <Orb/Example/ExampleOscillator.cs>.

The names of the variables for accessing them through the script are as follows:

core               (float)   - Size of the core               (From 0 to 1)
glowThickness      (float)   - Thickness of the glow          (From 0 to 1)
coreColor          (Color)   - Color of the core
edgeColor          (Color)   - Color of the edges of the core
glowColor          (Color)   - Color of the glow
noiseColor         (Color)   - Color of the produced noise
noiseAnimation     (Vector3) - Animation of noise
noiseAmount        (float)   - Amount of noise
noiseAmplitude     (float)   - Amplitude of noise             (From 0 to 1)
lightIntensity     (float)   - Light intensity                (From 0 to 8)
relativeLightRange (float)   - Relative light range

All variables are automatically validated on change.


=== TROUBLESHOOTING ===

Feel free to contact me at emil.boman@gmail.com if you have any problems with the asset.