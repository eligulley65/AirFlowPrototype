using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] Particle particlePrefab;
    [SerializeField] LineRenderer lineRendererPrefab;
    [SerializeField] Transform particleParent;
    [SerializeField] LineCreator lineCreator;
    public Vector3 mins;
    public Vector3 maxes;
   private Vector3[] intakeCorners = {new Vector3(1.3f, 0.0f, 0.0f), new Vector3(1.3f, 0.0f, 0.5f),
        new Vector3(1.3f, 0.5f, 0.5f), new Vector3(2.3f, 0.5f, 0.5f),
        new Vector3(2.3f, 0.0f, 0.5f), new Vector3(2.3f, 0.5f, 0.0f),
        new Vector3(2.3f, 0.0f, 0.0f), new Vector3(1.3f, 0.5f, 0.0f)};
    private Vector3[] outtakeCorners = {new Vector3 (4.646666f, 2.5f, 2.3092308f), new Vector3 (4.646666f, 2.5f, 2.661539f), 
        new Vector3 (4.433333f, 2.5f, 2.3092308f), new Vector3 (4.433333f, 2.5f, 2.661539f)};

    float[] intakeMaxes = {float.MinValue, float.MinValue, float.MinValue};
    float[] intakeMins = {float.MaxValue, float.MaxValue, float.MaxValue};
    private List<Node> nodesToStartAt;
    private List<List<Node>> slices;

    private void Start() 
    {
        lineCreator.NodesCreated += LineCreator_NodesCreated;
    }

    private void LineCreator_NodesCreated(object sender, EventArgs e)
    {
        Debug.Log("particles");
    }

    public void CreateParticles()
    {
        FindIntakeMaxesAndMins();
        nodesToStartAt = GetNodesToStartAt();
        List<Node> nodes = lineCreator.GetNodes();
        for (int i = 0; i < nodesToStartAt.Count; i++)
        {
            //nodesToStartAt[i].GetComponent<MeshRenderer>().enabled = true;
            //Debug.Log("I did it daddy");

            
            //int nodeIndex = i;
            Particle particle = Instantiate(particlePrefab, particleParent).GetComponent<Particle>();
            particle.SetNode(nodesToStartAt[i]);
            particle.SetLine(lineRendererPrefab);
            particle.SetMaxesAndMins(maxes, mins);
            particle.SetSlices(slices);
            //List<Node> nodesToRemove = particle.StartMoving(nodes);
            List<Node> nodesToRemove = particle.StartMovingWithIntegration(nodes, 0.3f);
            /*
            if (nodesToRemove == null) continue;
            foreach (Node node in nodesToRemove)
            {
                nodes.Remove(node);
            }
            //*/
        }
    }

    private List<Node> GetNodesToStartAt()
    {
        List<Node> nodes = lineCreator.GetNodes();
        List<Node> temp = new List<Node>();
        int numOfParticles = nodes.Count;
        for (int i = 0; i < numOfParticles; i++)
        {
            if (IsInIntakeBox(nodes[i].GetPosition()))
            {
                if (nodes[i].GetVelocity() == Vector3.zero){ continue; }
                Debug.Log("got one!: " + nodes[i].GetPosition());
                temp.Add(nodes[i]);
            }
        }
        return temp;
    }

    private void FindIntakeMaxesAndMins()
    {
        ///*
        for (int i = 0; i < intakeCorners.Length; i++)
        {
            CheckAndSet(intakeCorners[i].x, 0);
            CheckAndSet(intakeCorners[i].y, 1);
            CheckAndSet(intakeCorners[i].z, 2);
        }
        //*/
        /*
        for (int i = 0; i < outtakeCorners.Length; i++)
        {
            CheckAndSet(outtakeCorners[i].x, 0);
            CheckAndSet(outtakeCorners[i].y, 1);
            CheckAndSet(outtakeCorners[i].z, 2);
        }
        */
        /*
        for (int i = 0; i < mins.Length; i++)
        {
            mins[i] -= 0.3f;
            maxes[i] += 0.3f;
        }
        //*/
    }

    private void CheckAndSet(float value, int pos)
    {
        if (value < intakeMins[pos])
        {
            intakeMins[pos] = value;
        }
        else if (value > intakeMaxes[pos])
        {
            intakeMaxes[pos] = value;
        }
    }

    private bool IsInIntakeBox(Vector3 pointToCheck)
    {
        if (pointToCheck.x < intakeMins[0] || pointToCheck.x > intakeMaxes[0]){ return false; }
        if (pointToCheck.y < intakeMins[1] || pointToCheck.y > intakeMaxes[1]){ return false; }
        if (pointToCheck.z < intakeMins[2] || pointToCheck.z > intakeMaxes[2]){ return false; }

        return true;
    }

    public void SetSlices(List<List<Node>> s){
        slices = s;
    }
}
