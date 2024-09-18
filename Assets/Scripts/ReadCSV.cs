using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ReadCSV : MonoBehaviour
{
    public event EventHandler finishedParsing;
    List<Vector3> points;
    List<Vector3> vels;
    List<float> temps;
    int numOfTempSlices = 11;
    float tempSliceSize = 0;
    float[] maxAndMinTemps;

    private void Start() {
        points = new List<Vector3>();
        vels = new List<Vector3>();
        temps = new List<float>();
        ReadCSVFile(1);
        finishedParsing?.Invoke(this, EventArgs.Empty);
    }

    private List<StreamReader> InitializeFiles()
    {
        List<StreamReader> streams = new List<StreamReader>();
        string[] streamStrings = Directory.GetFiles("Assets/CSV Files/Current", "*.csv", SearchOption.AllDirectories);
        foreach (string path in streamStrings)
        {
            streams.Add(new StreamReader(path));
        }
        return streams;
    }

    void ReadCSVFile(int fillerLines)
    {
        List<StreamReader> files = InitializeFiles();

        for (int i = 0; i < files.Count; i++)
        {
            bool eof = false;
            for (int x = 0; x < fillerLines; x++) { files[i].ReadLine(); }
            while(!eof)
            {
                string data_String = files[i].ReadLine();
                if (data_String == null)
                {
                    eof = true;
                    break;
                }
                string[] data_values = data_String.Split(',');
                points.Add(ParseVector3(data_values, 1));
                vels.Add(ParseVector3(data_values, 5));
                temps.Add((float)Double.Parse(data_values[8], System.Globalization.NumberStyles.Float));
            }
        }
        tempSliceSize = GetTempSliceSize(temps, numOfTempSlices);
    }

    private Vector3 ParseVector3(string[] values, int low)
    {
        float x = (float)Double.Parse(values[low], System.Globalization.NumberStyles.Float);
        float y = (float)Double.Parse(values[low + 1], System.Globalization.NumberStyles.Float);
        float z = (float)Double.Parse(values[low + 2], System.Globalization.NumberStyles.Float);
        return new Vector3 (x, y, z);
    }

    private float GetTempSliceSize(List<float> temps, int numOfSlices){
        //get max and min temps
        float[] maxAndMin = { float.MinValue, float.MaxValue };
        for (int i = 0; i < temps.Count; i++){
            if (temps[i] > maxAndMin[0]){
                maxAndMin[0] = temps[i];
            }
            else if(temps[i] < maxAndMin[1]){
                maxAndMin[1] = temps[i];
            }
        }

        float range = maxAndMin[0] - maxAndMin[1];
        float sliceSize = range / numOfSlices;
        maxAndMinTemps = maxAndMin;
        return sliceSize;
    }

    public List<float> GetTemps(){
        return temps;
    }

    public List<Vector3> GetPoints()
    {
        return points;
    }

    public List<Vector3> GetVels()
    {
        return vels;
    }

    public float GetTempSliceSize(){
        return tempSliceSize;
    }
    public float[] GetMaxAndMinTemps(){
        return maxAndMinTemps;
    }
}
