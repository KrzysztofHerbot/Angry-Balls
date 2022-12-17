using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererScript : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] pivotPoints;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        //SetUpLine(pivotPoints);
    }

    public void SetUpLine(Transform[] pivotPoints)
    {
        lr.enabled = true;
        lr.positionCount = pivotPoints.Length;
        this.pivotPoints = pivotPoints;
    }

    private void Update()
    {
        if (pivotPoints == null) { return; }
        for(int i=0;i<pivotPoints.Length;i++)
        {
            lr.SetPosition(i, pivotPoints[i].position);
        }
    }

    public void DestroyLine()
    {
        lr.enabled = false;
    }
}
