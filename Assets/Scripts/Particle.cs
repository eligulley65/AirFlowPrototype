using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Particle : MonoBehaviour
{
    private Node currentNode;
    private LineRenderer lineRendererPrefab;
    private Vector3 start;
    private List<LineRenderer> lines;
    private Vector3 maxes;
    private Vector3 mins;
    private List<List<Node>> slices;

    //Similar to start moving, but instead of moving from node to node
    //it used steps and velocity to calculate the next position
    public List<Node> StartMovingWithIntegration(List<Node> nodes, float stepSize){
        List<Node> nodesInPath = new List<Node>();
        lines = new List<LineRenderer>();
        LineRenderer line = Instantiate(lineRendererPrefab, transform);
        line.positionCount = 1;
        line.SetPosition(0, currentNode.GetPosition());
        nodesInPath.Add(currentNode);
        Vector3 prevPosition = currentNode.GetPosition();
        int i = 0;
        bool simplified = false;
        int limiter = 10000;
        List<Vector3> tempPositions = new List<Vector3>();
        while (true && i < limiter){
            try{
                Vector3 velocity = currentNode.GetVelocity();
                //if the velocity is 0, there is no next position
                if (velocity == Vector3.zero){
                    Debug.Log("blocked by 0 velocity node");
                    break;
                }
                //find next position using fun math
                Vector3 nextPosition = FindNextPosition(prevPosition, currentNode, stepSize, nodes);
                if (prevPosition == nextPosition) {
                    Debug.Log("not moving");
                    break;
                }
                var prevNode = currentNode;
                currentNode = GetNodeFromPosition(nextPosition, nodes, currentNode);
                if (currentNode is null){
                    Debug.Log("no node found");
                    break;
                }
                if (prevNode == currentNode){
                    tempPositions.Add(nextPosition);
                }
                else{
                    tempPositions.Add(nextPosition);
                    tempPositions.Clear();
                }
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, nextPosition);
                nodesInPath.Add(currentNode);
                prevPosition = nextPosition;
                i++;
                if (i >= 100 && !simplified){
                    i = line.positionCount - 1;
                    if (line.positionCount <= 3){
                        break;
                    }
                    simplified = true;
                }
            }
            //if the next position doesn't have a valid node, this exception will be thrown
            catch (NotFoundException){
                Debug.Log("Ran out of nodes at " + i);
                break;
            }
        }
        line.Simplify(0.01f);
        line.enabled = false;
        if (i >= limiter){
            Debug.Log("limiter");
        }

        Vector3[] lineVertices = new Vector3[line.positionCount];
        line.GetPositions(lineVertices);
        SplitAndSetColorAll(lineVertices.ToList<Vector3>(), nodes);
        return nodesInPath;
    }

    private Vector3 FindNextPosition(Vector3 startPosition, Node startNode, float stepSize, List<Node> listOfAllNodes){
        Vector3 velocity = startNode.GetVelocity();

        //derive position from velocity using fourth-order Runge-Kutta method
        Vector3 temp = stepSize*GetVelocityFromPosition(startPosition, listOfAllNodes, startNode);
        Vector3 tempWithHalfStep = stepSize*GetVelocityFromPosition(new Vector3 (startPosition.x + (stepSize/2.0f), startPosition.y + (stepSize/2.0f), startPosition.z + (stepSize/2.0f)), listOfAllNodes, startNode);
        Vector3 tempWithFullStep = stepSize*GetVelocityFromPosition(new Vector3(startPosition.x + stepSize, startPosition.y + stepSize, startPosition.z + stepSize), listOfAllNodes, startNode);

        Vector3 displacement = 1.0f/6.0f*(temp + 4*tempWithHalfStep + tempWithFullStep);
        return startPosition + displacement;
    }

    private Vector3 GetVelocityFromPosition(Vector3 position, List<Node> listOfAllNodes, Node startNode = null){
        //check the starting node as it's very likely the point will be inside. Saves some computations
        if (startNode != null && startNode.IsPointInside(position)){
            return startNode.GetVelocity();
        }

        List<Node> slice = GetSliceFromPosition(position.x);
        //if not inside the start node, check all other nodes
        foreach(Node node in slice){
            if (node.IsPointInside(position)){
                if (node == startNode){ continue; }
                return node.GetVelocity();
            }
        }

        if (startNode != null && IsPointInsideRoom(startNode.GetPosition())){
            return startNode.GetVelocity();
        }
        throw new NotFoundException("Velocity not found.");
    }

    private bool IsPointInsideRoom(Vector3 point){
        if (point.x > maxes.x || point.y > maxes.y || point.z > maxes.z){
            return false;
        }
        if (point.x < mins.x || point.y < mins.y || point.z < mins.z) {
            return false;
        }
        return true;
    }

    private Node GetNodeFromPosition(Vector3 position, List<Node> listOfAllNodes, Node startNode = null){
        //check the starting node as it's very likely the point will be inside. Saves some computations
        if (startNode != null && startNode.IsPointInside(position)){
            return startNode;
        }

        List<Node> slice = GetSliceFromPosition(position.x);
        //if not inside the start node, check all other nodes
        foreach(Node node in slice){
            if (node.IsPointInside(position)){
                return node;
            }
        }
        return null;
    }

    private void SplitAndSetColorAll(List<Vector3> positions, List<Node> nodes){
        int index = 0;
        foreach(Vector3 pos in positions){
            if (index == positions.Count - 1){ break; }
            Node node = GetNodeFromPosition(pos, nodes);
            if (node is null) {
                Debug.Log("yoink");
                index++;
                continue;
            }
            LineRenderer tempLine = Instantiate(lineRendererPrefab, transform);
            tempLine.positionCount = 2;
            tempLine.SetPosition(0, pos);
            tempLine.SetPosition(1, positions[++index]);
            tempLine.startColor = node.GetColor();
            tempLine.endColor = node.GetColor();
        }
    }

    private List<Node> GetSliceFromPosition(float xPos){
        int i = 0;
        for (;i < slices.Count; i++){
            if (xPos < (i+1)*0.1f){
                return slices[i];
            }
        }
        return slices[i - 1];
    }

    public void SetSlices(List<List<Node>> s){
        slices = s;
    }

    public void SetMaxesAndMins(Vector3 maximums, Vector3 minimums)
    {
        maxes = maximums;
        mins = minimums;
    }
    public void SetNode(Node node){ 
        currentNode = node; 
        start = node.GetPosition();
    }
    public void SetLine(LineRenderer line)
    {
        lineRendererPrefab = line;
    }
}
