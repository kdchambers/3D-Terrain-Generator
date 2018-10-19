# Game Engines project

## 3D terrain generator

For my project I want to develop a procedurally generated 3d dimentional terrain, as I have always been facinated by their unmatched beauty in games such as Minecraft, and Civilization.

### Overview

For my world I would like to develop a finite world, that generates based on biomes and weather zones. Weather zones will be affected by height and latitude and biomes will add more specific details of a correlated weather zone. For example, it will be more likely to generate a desert biome closer to the equator, however there will be no hard boundaries to where any particular biome may occur. 

Apart from the basic components of a terrain, such as seas and mountains I hope to spawn in various different types of plants such as trees and flowers, in accordance to their biome. 

Things like grass, mountains and sea will all be textured appropriatly to provide greater detail and realism. 

This will be my target for the project, however if time allows there are many more interesting things that could take the world to a new level, such as:

- Generating animals with ai for movements
- Spawning weather
- Generating simple settlements
- Applying different art styles (Low poly, etc)

### Implementation thoughts

At this point, there is still research required into the techniques that would suit this project best. However, I can give a rough outline as to what techniques will be deployed in the project, and why. 


I will use Perlin noise as the basic building block for building my world. I will generate a height map by layering noise functions and applying them to different areas. Similarily with the weather zones, I will apply noise with a bias that ensures that the closer you are the equator, the hotter the base weather will be. However, height will also have an effect on temperature. 

Water level will simply be at a fixed height.

As for the plants that occupy the world, I will probably use L-Systems to generate realistic plants. These of course will be varied depending on the biome they exist within.

At the end of all this, we should have a reasonable varied world that the user can then explore and inspect!

**References**

Smelik, Ruben M., et al. "A survey of procedural methods for terrain modelling." Proceedings of the CASA Workshop on 3D Advanced Media In Gaming And Simulation (3AMIGAS). 2009.

Smith, Alvy Ray. "Plants, fractals, and formal languages." ACM SIGGRAPH Computer Graphics 18.3 (1984): 1-10.