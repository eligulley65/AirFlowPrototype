using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;

public class LineCreator : MonoBehaviour
{
    [SerializeField] ReadCSV readCSV;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] Transform nodeParent;
    [SerializeField] ParticleManager particleManager;
    List<Node> nodes;
    List<List<Node>> nodeSlices;
    Vector3 maxes;
    Vector3 mins;
    const float sliceWidth = 0.1f;

    public event EventHandler NodesCreated;

    void Start()
    {
        readCSV.finishedParsing += ReadCSV_FinishedParsing;
    }

    private void ReadCSV_FinishedParsing(object sender, EventArgs e)
    {
        maxes = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        mins = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        List<Vector3> points = readCSV.GetPoints();
        List<Vector3> vels = readCSV.GetVels();
        List<float> temps = readCSV.GetTemps();
        float[] tempMaxAndMin = readCSV.GetMaxAndMinTemps();
        float tempSliceSize = readCSV.GetTempSliceSize();
        nodes = new List<Node>();
        for (int i = 0; i < points.Count; i++)
        {
            Node node = Instantiate(nodePrefab, nodeParent).GetComponent<Node>();
            node.transform.position = points[i];
            node.GetComponent<Node>().Initialize(points[i], vels[i]);
            node.SetTempData(tempMaxAndMin, tempSliceSize);
            node.SetColorFromTemp(temps[i]);
            
            nodes.Add(node);
            CheckForMinsAndMaxes(points[i]);
        }
        particleManager.maxes = maxes;
        particleManager.mins = mins;
        nodeSlices = SliceNodeField(nodes);
        particleManager.SetSlices(nodeSlices);
        particleManager.CreateParticles();
    }

    private List<List<Node>> SliceNodeField(List<Node> nodes){
        float low = mins.x;
        float high = maxes.x;
        float diff = high - low;
        List<List<Node>> slices = new List<List<Node>>();
        List<float> sliceLocationsHigh = new List<float>();
        int numOfSlices = (int)(diff / sliceWidth);

        for (int i = 0; i < numOfSlices; i++){
            slices.Add(new List<Node>());
            sliceLocationsHigh.Add(low + (i+1) * sliceWidth);
        }

        foreach (Node node in nodes){
            for (int i = 0; i < sliceLocationsHigh.Count; i++){
                if (node.GetPosition().x < sliceLocationsHigh[i]){
                    slices[i].Add(node);
                    break;
                }
            }
        }
        return slices;
    }

    private void CheckForMinsAndMaxes(Vector3 point){
        if (point.x < mins.x){
            mins.x = point.x;
        }
        if (point.y < mins.y){
            mins.y = point.y;
        }
        if (point.z < mins.z){
            mins.z = point.z;
        }
        if (point.x > maxes.x){
            maxes.x = point.x;
        }
        if (point.y > maxes.y){
            maxes.y = point.y;
        }
        if (point.z > maxes.z){
            maxes.z = point.z;
        }
    }

    public List<Node> GetNodes()
    {
        return nodes;
    }
}
