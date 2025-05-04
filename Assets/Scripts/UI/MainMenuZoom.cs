using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuZoom : MonoBehaviour
{
    [SerializeField] private Image hiderImage;
    
    private float fadeSpeed = 0.03f;
    private float currentAlpha = 0f;

    private float zoomSpeed = 0.02f;
    private float zoomOutSpeed = -6f;
    private const float ZoomMax = 2.7f;
    private const float ZoomMin = 1.15f;
    private bool fading;
    private float zoom = ZoomMin;

    private bool zoomIn = true;

    [SerializeField] private Color FromColor; 
    [SerializeField] private Color ToColor;
    private float animationTimer = 0;
    private const float AnimationTime = 1.2f;


    private void Start()
    {
        hiderImage.gameObject.SetActive(false);
        fading = false;
    }

    // Update is called once per frame
    void Update()
    {
        // General zooming
        transform.localScale = new Vector3(zoom, zoom, zoom);
        zoom += zoomSpeed * Time.deltaTime;

        if(zoom >= ZoomMax) {
            if (!fading) {
                StartCoroutine(Transition());
                fading = true;
            }
        }
        
    }


    private IEnumerator Transition()
    {
        // Make Image visible
        hiderImage.gameObject.SetActive(true);

        hiderImage.color = FromColor;

        // Darkening
        animationTimer = 0;
        while (animationTimer < AnimationTime) {
            hiderImage.color = Color.Lerp(FromColor, ToColor, animationTimer / AnimationTime);
            animationTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Move background
        zoom = ZoomMin;

        // Lightening
        animationTimer = 0;
        while (animationTimer < AnimationTime) {
            hiderImage.color = Color.Lerp(ToColor, FromColor, animationTimer / AnimationTime);
            animationTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Hide the image again
        hiderImage.gameObject.SetActive(false);
        fading = false;
    }
}
