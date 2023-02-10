# Programmatic-Planet-Rendering
3rd Year Dissertation Project. Made in Unity 2020.3.9f1

## About
My dissertation was about assessing and analysing techniques for procedural generation. I based this around the idea of generating a planet, and that I would use a range of techniques to generate its surface and its biomes, alongside some form of level of detail (LOD).

## LOD
Rather than have a chunk of terrain I settled on a spherical ball that would be my planet and apply noise to its vertices to make its surface rough and resemble terrain. This is where LOD was applied, as I was generating the sphere programmatically, I could alter the total number of vertices in the mesh, allowing me to assess the impact of vertices on performance and how well other techniques scale against one another.
## Noise
The noise algorithm used is Simplex Noise. It is applied in filters that have varying levels of customisation. They also can apply multiple layers of noise to provide a rougher surface. This was assessed in my dissertation, as increasing the number of layers affected the fidelity of the planet, as a rougher planet looks better and is more accurate, however these layers increase the number of iterations on the meshes vertices, which scales against the total number of vertices as well. Yet if the vertices count is low the roughness is erased anyways. In my dissertation I explained the need to find a middle ground between the right amount of noise layers and enough vertices to represent the noise.
## Shaders
As I was originally using these to generate biomes, I decide to use compute shaders. In this shader the number of biomes would be set and random vector4 of colours would be given, it would then colour the world in way in which each biome had a origin and vertices closest would be coloured in that biomes colour. This was assessed against C# version that would do the same thing. From this I found that the results were incomparable, as the compute shader could generate 1000s of biomes on large mesh in milliseconds whereas the C# could do a couple on large mesh. Only on small meshes were these results ever close. To expand on this and to get more results I decided to use a compute shader for sphere generation and found similar results against its C# counterpart.
