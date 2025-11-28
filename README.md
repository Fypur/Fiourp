# *Fiourp* - A game engine built on Monogame
This is a C# game engine built on top of Monogame made in 2 years alongside my game [Unnamed](https://fypur.itch.io/unnamed) ([source code](https://github.com/fypur/unnamed)). The engine is inspired by Monocle, Celeste's engine.

## Features
The engine includes
- An entity component system, with entities that can Update and Render, have components, have children
- Platformer physics system with structures like solids and actors inspired by [this article](https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5)
- A small basic physics engine based on [Erin Catto's](https://box2d.org/about/) work
- Various premade components such as timers, different renderers like sprites, Sound utility, state machines...
- a sound system with spacial sound using [FMOD](https://fmod.com)
- A computationally efficient light drawing system inspired by [this article](https://medium.com/@NoelFB/remaking-celestes-lighting-3478d6f10bf)
- Data loading and caching (with .ase files support thanks to [Monogame Aseprite](https://monogameaseprite.net/))
- A simple particle system
- A simple UI system that can scale with screensize
- Input Management
- Various utils (bezier curves, raycasting, drawing primitives...)

## Requirements
Everything is already featured in the csproj, but this uses the [Monogame.Framework.DesktopGL](https://www.nuget.org/packages/MonoGame.Framework.DesktopGL/3.8.5-develop.13) and [Monogame.Aseprite](https://www.nuget.org/packages/MonoGame.Aseprite) packages.
