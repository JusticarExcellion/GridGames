using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPather : MonoBehaviour
{
    AIManager Aim;
    private List<Transform> Markers;

    void Start()
    {
        Aim = FindObjectOfType<AIManager>();
        Markers = new List<Transform>();
    }

    void Update()
    {
        if( Markers.Count <= 0 )
        {
            Markers = Aim.GetAllWaypoints();
        }
        else
        {
            DrawPath();
        }
    }

    private void
    DrawPath()
    {
        Vector3 LastMarker = Markers[ Markers.Count - 1 ].position;
        foreach( Transform Marker in Markers )
        {
            if( LastMarker != Vector3.zero )
            {
                Debug.DrawLine( LastMarker, Marker.position, Color.blue );
            }

            LastMarker = Marker.position;
        }
    }
}
