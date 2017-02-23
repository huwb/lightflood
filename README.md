# lightflood
Experimental progress GI renderer in Unity using physics raycasts for light rays

![Teaser](https://raw.githubusercontent.com/huwb/lightflood/master/img/teaser.jpg)  

## Overview

This is a hacky experimental CPU progressive lightmapper. Geometry is represented as collideable quads. Quads because lightmap UVs become trivial, and collideable because rays are cast using Physics.Raycast. None of this is highly performant (although it does update interactively for small scenes), nor is it highly suitable for production scenarios in its current state. So far it was purely for fun and experimentation.

## Instructions

The scene should 'just work', last opened in Unity 5.5.

Enable or disable gameobjects in the scene to change it as desired - no scene configuration etc is required, everything is dynamic.

You can fly around with WASD/arrow keys. Press space to shoot bouncy spheres of light.
