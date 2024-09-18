using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Node : MonoBehaviour
{
    Vector3 position;
    public Vector3 velocity;
    float rad = 0.30f;
    Color color = new Color();
    float[] maxAndMinTemps;
    float tempSliceSize;

    public void Initialize(Vector3 pos, Vector3 vel)
    {
        position = pos;
        velocity = vel;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public bool IsPointInside(Vector3 pos)
    {
        bool x = false;
        bool y = false;
        bool z = false;
        if ((pos.x <= position.x + rad) && (pos.x >= position.x - rad)){ x = true; }
        if ((pos.y <= position.y + rad) && (pos.y >= position.y - rad)){ y = true; }
        if ((pos.z <= position.z + rad) && (pos.z >= position.z - rad)){ z = true; }
        return x && y && z;
    }

    public void SetColorFromTemp(float temp)
    {
        int colorIndex = 0;
        float minTemp = maxAndMinTemps[1];
        float maxTemp = maxAndMinTemps[0];
        float floor = minTemp;
        while (floor < maxTemp){
            floor += tempSliceSize;
            if (temp < floor){
                break;
            }
            colorIndex++;
        }

        switch(colorIndex){
            case 0:
                color = new Color(0f, 0f, 0.5019f, 1f);
                break;
            case 1:
                color = new Color(0, 0, 1, 1);
                break;
            case 2:
                color = new Color(0, 0.3764f, 1, 1);
                break;
            case 3:
                color = new Color(0, 0.7529f, 1, 1);
                break;
            case 4:
                color = new Color(0.1254f, 1, 1, 1);
                break;
            case 5:
                color = new Color(0.5019f, 1, 0.2509f, 1);
                break;
            case 6:
                color = new Color(1, 1, 0, 1);
                break;
            case 7:
                color = new Color(1, 0.6274f, 0, 1);
                break;
            case 8:
                color = new Color(1, 0.2509f, 0, 1);
                break;
            case 9:
                color = new Color(1, 0, 0, 1);
                break;
            case 10:
                color = new Color(0.5019f, 0, 0, 1);
                break;
        }
    }

    public void SetTempData(float[] maxAndMin, float sliceSize){
        maxAndMinTemps = maxAndMin;
        tempSliceSize = sliceSize;
    }

    public Color GetColor(){
        return color;
    }
}
