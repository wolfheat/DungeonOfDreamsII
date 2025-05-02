using UnityEngine;

public class MainMenuZoom : MonoBehaviour
{
    private float zoomSpeed = 0.03f;
    private float zoomOutSpeed = -6f;
    private const float ZoomMax = 2.7f;
    private const float ZoomMin = 1.25f;
    private float zoom = ZoomMin;

    private bool zoomIn = true;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(zoom, zoom, zoom);
        zoom += (zoomIn ? zoomSpeed:zoomOutSpeed)*Time.deltaTime;

        if (zoomIn) {
            if(zoom >= ZoomMax) {
                zoom = ZoomMax;
                zoomIn = false;
            }
        }
        else {
            if (zoom <= ZoomMin) {
                zoom = ZoomMin;
                zoomIn = true;
            }
        }
    }
}
