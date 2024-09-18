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