using UnityEngine;

public class MainMenuZoom : MonoBehaviour
{
    private float zoom = 1f;
    private float zoomSpeed = 0.01f;
    private const float ZoomMax = 2.7f;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(zoom, zoom, zoom);
        zoom += zoomSpeed*Time.deltaTime;
        if(zoom >= ZoomMax)
            zoom = 1f;
    }
}
