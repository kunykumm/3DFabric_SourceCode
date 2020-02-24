using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UNUSED

public class CameraUpdatePosition : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 wantedPosition;
    private bool allowUpdate;

    private void Start()
    {
        offset = Vector3.zero;
        wantedPosition = Vector3.zero;
        allowUpdate = false;
    }

    private void Update()
    {
        if (allowUpdate)
        {
            Vector3 velocity = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, wantedPosition + offset, ref velocity, 0.1f);
            allowUpdate = false;
        }

    }

    public void SetOffset(Vector3 newPosition)
    {
        wantedPosition = newPosition;
        allowUpdate = true;
        offset = transform.position - newPosition;
    }
}
