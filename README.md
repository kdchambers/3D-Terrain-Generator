# 3D Procedural Terrain Generator

![Alt text](./terrainGeneratorscreenshot.png?raw=true "Screen Shot")

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

Apart from making maps, you can also start the game and fly around to get a better look. 




