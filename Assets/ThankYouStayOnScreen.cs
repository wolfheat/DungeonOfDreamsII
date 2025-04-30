using System;
using System.Collections;
using UnityEngine;

public class ThankYouStayOnScreen : MonoBehaviour
{
    [SerializeField] private GameObject centerParent;
    private float centerPosition = 0f;
    private bool rising = true;

    private void Start()
    {
        rising = true;
        centerPosition = Screen.height / 2;                    
    }

    void Update()
    {       
        if(transform.position.y > centerPosition && rising) {
            rising = false;
            //Center
            transform.SetParent(centerParent.transform);
            Debug.Log("Start Grow coroutine");
            StartCoroutine(Grow());
        }    
    }

    private IEnumerator Grow()
    {
        Debug.Log(" Grow ");
        yield return null;
        float currentScale = 1f;
        const float MaxScale = 2.1f;
        const float GrowSpeed = 0.003f;
        while(currentScale < MaxScale) {
            Debug.Log("currentscale is "+currentScale);
            transform.localScale = new Vector3(currentScale,currentScale,currentScale);
            currentScale += GrowSpeed;
            Debug.Log("currentscale becomes "+currentScale+" max = "+MaxScale);
            //yield return new WaitForSeconds(GrowSpeed);
            yield return null;
        }
    }
}
