using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************************
 * Cam
 * Camera controller class. Controls camera shake when using
 * light speed.
 * *********************************************************/
public class Cam : MonoBehaviour
{
    public float shakeFactor, zoomSpeed, maxZoom;

    private Vector3 offset;
    private Player player;
    private float zoom;

    void Start()
    {
        player = GetComponentInParent<Player>();
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        if (player.useLightSpeed)
        {
            // Zoom out further behind the player and shake the camera
            transform.localPosition = offset + new Vector3(Random.Range(0, shakeFactor), Random.Range(0, shakeFactor), -zoom);
            zoom = Mathf.Min(maxZoom, zoom + zoomSpeed * Time.deltaTime);
        }
        else
        {
            // Reset the position
            zoom = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, offset, Time.deltaTime * 10);
        }
    }
}
