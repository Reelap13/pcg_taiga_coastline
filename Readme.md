# The taiga coastline generator

Mikhail Stepanov
mi.stepanov@innopolis.university
GD-01
https://github.com/Reelap13/pcg_taiga_coastline

This is the work of Mikhail Stepanov for Individual Project 1 of the PCG course.


## Algorithms

I used chunk-based generation and agent-based techniques for this project, as well as Midpoint Displacement Fractal and Perlin Noise.

### Midpoint Displacement Fractal

I used this algorithm to generate the coastline, and the result turned out quite realistic.

1. Before starting the algorithm, I set a seed for randomness to make the calculations deterministic.
2. The algorithm starts by initializing two extreme points (set via the inspector).
3. For each pair of neighboring points, I found the midpoint and moved it by a random value. To make the line smoother, I reduced the displacement with each iteration.
4. The new points were stored in an array.
5. The process repeated from step 3 until the set number of iterations was completed.

```c#
Random.InitState(Generator.Instance.Seed * _coastline_seed_coefficient);

Vector2[] points = new Vector2[] { new(_x_boarders.x, _y_start), new(_x_boarders.y, _y_start) };

for (int iteration = 0; iteration < _iteration_number; ++iteration)
{
    List<Vector2> new_points = new();
    for (int i = 0; i < points.Length - 1; ++i)
    {
        Vector2 start = points[i];
        Vector2 end = points[i + 1];
        new_points.Add(start);

        Vector2 middle = (start + end) / 2;
        middle.y += Random.Range(-_height_offset, _height_offset) * Mathf.Pow(_roughness, iteration);

        new_points.Add(middle);
    }

    new_points.Add(points[points.Length - 1]);
    points = new_points.ToArray();
}

_points = points;
```

### Perlin Noise

I used this to generate the heightmap for the land (not the sea). I multiplied the noise values by a coefficient so that the height increased as you moved away from the coastline, which created a realistic terrain.

To compress the noise, I divided it by a `scale` parameter. I also used Perlin noise with a coefficient that increased up to 1 as the distance from the coastline grew.

```sh
[SerializeField] private float _scale;
[SerializeField] private float _height_coefficient = 50;
[SerializeField] private float _linear_displacement = 0.01f;

public override float GetHeight(Vector2 position)
{


    return SeaHeight + _height_coefficient * 
        Mathf.PerlinNoise(position.x / _scale, position.y / _scale) * 
        Mathf.Min(1, _linear_displacement * GetOffsetsFromCoastline(position));
}
``` 

### Chunk system

To achieve better performance and scalability, I decided not to generate the entire map at once, but instead use chunk-based generation.

I implemented:

- ChunksManager — manages chunk loading and unloading.
- ChunksFactory — creates chunks and provides an interface for saving and loading.
- ChunkGenerator — collects data from agents.
- ChunkApplier — applies data to the chunk.

This results in deterministic chunk generation with a time complexity of O(n), where n is the number of cells in a chunk.

To optimize performance, I load chunks gradually using coroutines, loading one chunk per frame to avoid system overload.

### Agent system

I divided the generation into several agents:

- Heights Agent — responsible for height generation. It splits the terrain into sea and land. The sea uses a linear function, while the land uses Perlin noise.

- Biomes Agent — determines the biome for each point. Biomes are based on distance from the coast:

    - from -1000 to -10 — sea,
    - from -10 to 10 — coast,
    - from 10 to 16 — field,
    - from 16 to 300 — taiga.

- Textures Agent — assigns textures to each point based on its biome.

- Objects Agent — places objects according to the biome.

## Main problems

1. Water Implementation

    The sea consists of many small 1x1 planes, which caused synchronization problems. I minimized water movement to reduce the visibility of the grid.

2. Working with Terrain

    Inverted X and Z axes had to be manually fixed. The heightmap has resolution limitations (2^n + 1, where n > 5).

3. Algorithm Errors

    Some errors in formulas took a long time to debug.

## Future work

1. Chunk Saving

    Implement a system for saving chunks, especially for future map changes.

2. Biome Distribution System

    I’d like to make biome transitions more varied and natural.

3. New Biomes

    Add mountain, swamp, and other biomes.

4. Heightmap

    Improve the heightmap generation algorithm with biomes in mind.

## Conclusion

I really enjoyed this project. I learned a lot about generation and Unity, and I want to keep working on it (maybe using the Voronoi algorithm in Assignment 2).


## Pictures

### Coastline

![alt text](image.png)
![alt text](image-1.png)
![alt text](image-2.png)

### Coastline after generation

![alt text](image-3.png)
![alt text](image-4.png)

### Taiga

![alt text](image-5.png)

### Seabed

![alt text](image-6.png)