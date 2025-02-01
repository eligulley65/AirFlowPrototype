# AirFlowPrototype

This project visualizes Computational Fluid Dynamics (CFD) data. This data is input using .csv files
in the form of individual nodes. Each node has a position, velocity, and temperature, all generated
from an external simulation. The nodes are created in Unity to create a vector field, and then
particles are shot through the field using integration to calculate how they move from node to node.
The paths are then drawn, and the colors are modified to reflect the temperatures. The result is the lines
seen when the project is run.

The project takes input from the "Current" folder within the "CSV Files" folder. If you need to change what data
is taken in, move the files in "Current" to another folder, and put the new files in "Current".

ReadCSV.cs is the file responsible for extracting the node data from the CSV files. It uses a StreamReader
object to take in each line, then it splits the line into sections using the commas as seperators.
It also calculates the different temperature ranges so node colors can be set based on temperature.

LineCreator.cs runs next, and it is responsible for taking the data from ReadCSV and creating Node objects from it.
It also slices the node field into sections, making it much easier for the particles to search through later.
It then sets up ParticleManager and runs CreateParticles()

ParticleManager.cs creates a particle object at each node within the intake box. It then sets up each particle and 
runs the StartMovingWithIntegration method within them.

Particle.cs houses the main algorithm to track how the particles move throughout the node field. Each one starts at a 
node, and then uses the Fourth-Order Runge-Kunta method to solve where the particle will end up after 0.3 seconds.
There are two checks that determine whether or not a particle gets to continue on its path. The first is if the 
velocity of the node is zero. If the velocity is zero, it'll never move to another node, so the movement is 
terminated to help save processing power. Then, the equations are used in the FindNextPosition method. The 
next check sees if the previous position is the same as the one given by the method. If so, the particle isn't
moving, so it is terminated. If both checks pass, the new position becomes the particle's current position
and the position is added to the final position list. Finally, a line is drawn using the LineRenderer to visualize
the path the particle traveled on.