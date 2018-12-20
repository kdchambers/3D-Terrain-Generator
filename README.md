# 3D Procedural Terrain Generator

![Alt text](./terrainGeneratorscreenshot.png?raw=true "Screen Shot")

### Description 

Project generates a 3D, coloured terrain based on input values specified by user. These values and what they do are specified below in the Usage section of this readme. 

Map is coloured based off of height, for example the lowest levels of the map are blue to indicate water. Each height level can also support multiple colors that are randomly selected so that the texture of the world is more detailed. Although configuration of the colors is not exposed to the user via the Unity Editor, is it simple to work with in the code. 

Also, the lowest height which represents water is a special case since it is clamped up to the maximum height of water. I.e You don't have curved seas. 

Once you generate a terrain, you can run the game where you will be able to fly around the map to inspect it.

Finally, you can render the map in one big chunk, or break it into lots of different chunks. Although the project doesn't support dynamic generation and deletion of chunks it would be easy enough to implement with the ability to create chunks complete.

### Work Done

All of the scripts and assets were created by me. I did not use any external assets except what Unity provided in the project creation. 

### Most Proud Of

I am most proud of being able to generate worlds in the Unity Editor and that different colors can be used to create a more detailed texture for different height levels. This is always a deterministic process to, I.e you always get the same color variation when you generate the same map.

Below is a video showcasing it and explaining a little about it

[![YouTube](http://img.youtube.com/vi/DW9pV15X7ho/0.jpg)](https://www.youtube.com/watch?v=DW9pV15X7ho&feature=youtu.be)

### Usage: 

Use the options in TerrainGenerator to control what kind of terrain you wish to generate. Here's an overview of the options available:

1. **Map Size Setting**: Sets the size of a particular chuck. The actual size will just be this value * 5
2. **Seed**: The seed to use when generating noise for the map 
3. **Scale**: The scale to use when generating noise (Higher value has a zooming effect)
4. **Num Octaves**: The number of passes to apply when generating noise
5. **Persistance**: The persistance to use when generating noise. 
6. **Lacunarity**: The lacunarity to use when generating noise
7. **Use Terrain Colors**: True if you want to use the terrain colors applied for different height levels of the map. False to to use black and white to indicate height
8. **Auto Generate**: Will regenerate the map when any of the parameters change. Can cause program to run slow if map generation is difficult but good to see how the map changes as you change the paramters.
9. **Max Map Height**: The multiplier to apply to the y values to give the terrain height
10. **Chunk Render Distance**: How many chunks to render around the origin chunk. If set to 0, only the origin chunk will be rendered, if set to 1, 8 chunks will be rendered around the origin chunk, etc
11. **Generate**: Will generate the map
12. **Clear Map**: Will clear the current map if exists






