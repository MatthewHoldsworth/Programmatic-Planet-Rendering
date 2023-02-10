# Programmatic-Planet-Rendering
3rd Year Dissertation Project. Made in Unity 2020.3.9f1

## About
My dissertation was about assessing and analysing techniques for proecudral generation. I based this around the idea of generating a planet, and that I would use a range of techniques to generate its surface and its biomes, alonside some form of level of detail (LOD).

## LOD
Rather than have a chunk of terrain I settled on a spherical ball that would be my planet and apply noise to its vertices to make its surface rough and resemble terrain. This is where LOD was applied, as I was generating the sphere programmatically I could alter the total number of vertices in the mesh, allowing me to assess the impact of vertices on performance and how well other techniques scale against one another.
## Noise

## Shaders
