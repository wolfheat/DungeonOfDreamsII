using System;
using System.Collections;
using UnityEngine;
public enum ShakeType{Perlin,AnimationCurve}
public enum ShakeDataType{Bomb,Door,BossLand,BossDie}

public class CameraShakeController : MonoBehaviour
{
    enum ShakeType{Perlin,AnimationCurve}

    [SerializeField] private ShakeType ShakeTypeSelected;

    [Header("Bomb, Door, Boss")]
    [SerializeField] private CameraShakeControllerData[] shakeDatas; 
    
    
    [Header("General Settings")]
    [Range(0,5)]
    [SerializeField] private float shakeTime = 0;
    [SerializeField] private float shakeDelay = 0;

    [Range(0,5)]
    [SerializeField] private float shakeStrengthX; 

    [SerializeField] private bool useDampening = true;
    
    [Header("Specific Settings")]
    // ANIMATION CURVE
    [ConditionalHide("ShakeTypeSelected",ShakeType.AnimationCurve)]
    [SerializeField] private AnimationCurve animationCurve; 


    // PERLIN
    [ConditionalHide("ShakeTypeSelected",ShakeType.Perlin)][Range(0,1)]
    [SerializeField] private float shakeStrengthY; 
    
    
    [ConditionalHide("ShakeTypeSelected",ShakeType.Perlin)][Range(0,1)]
    [SerializeField] private float FadeInTime = 0.05f;
    
    [ConditionalHide("ShakeTypeSelected",ShakeType.Perlin)][Range(0,1)]
    [SerializeField] private float FadeOutTime = 0.05f;

    [ConditionalHide("ShakeTypeSelected",ShakeType.Perlin)][Range(0,1)]
    [SerializeField] private float Damper = 0.05f;


    [Header("Position in perlin noise")]
    //[Range(0,1)]
    //[SerializeField] private float XShakePosition = 0.5f; 
    private float XShakePosition = 0f; 
    //[Range(0,1)]
    //[SerializeField] private float YShakePosition = 0.5f; 
    private float YShakePosition = 0.4398786f;
    //private const float ShakeTime = 0.2f;   
    private float shakeTimer;


    public static CameraShakeController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void Shake(ShakeDataType type)
    {
        SetShakeData(type);

        switch (ShakeTypeSelected) {
            case ShakeType.Perlin:
                StartCoroutine(PerlinShakeRoutine());
                break;
            case ShakeType.AnimationCurve:
                StartCoroutine(AnimationCurveShakeRoutine());
                break;
        }
    }

    private void SetShakeData(ShakeDataType type)
    {
        CameraShakeControllerData data = shakeDatas[(int)type];
        this.ShakeTypeSelected = (ShakeType)data.ShakeTypeSelected;

        this.shakeTime = data.shakeTime;
        this.shakeDelay = data.shakeDelay;
        this.shakeStrengthX = data.shakeStrengthX;
        this.useDampening = data.useDampening;

        switch (data.ShakeTypeSelected) {
            case global::ShakeType.Perlin:
                this.shakeStrengthY = data.shakeStrengthY;
                this.FadeInTime = data.FadeInTime;
                this.FadeOutTime = data.FadeOutTime;
                this.Damper = data.Damper;

                break;
            case global::ShakeType.AnimationCurve:
                this.animationCurve = data.animationCurve;

                break;
        }
    }

private IEnumerator AnimationCurveShakeRoutine()
    {
        if (shakeDelay > 0)
            yield return new WaitForSeconds(shakeDelay);

        Debug.Log("ANIMATION SHAKE");
        shakeTimer = shakeTime;
        // Return camera to start position
        while (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            float t = 1 - shakeTimer / shakeTime;
            transform.localPosition = GetAnimationCurvePosition(t);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        // Camera has now returned to initial position
    }
    
    private IEnumerator PerlinShakeRoutine()
    {

        if(shakeDelay > 0)
            yield return new WaitForSeconds(shakeDelay);

        Debug.Log("PERLIN SHAKE");
        // find 0.5 on x axis
        //float i = 0;
        //float step = 0.0001f;
        //
        //Debug.Log("Finder started");
        //float best = 1;
        //while (i < 1) { 
        //    float perl = Mathf.PerlinNoise(i, 0);
        //    float diff = Mathf.Abs(perl - 0.5f);
        //    
        //    if(diff < best) { 
        //        Debug.Log("Value "+i+" = "+perl+" diff = "+diff);
        //        best = diff;
        //    }
        //    i += step;
        //}
        //Debug.Log("Finder complete");




        // Setup Fade in - need to lerp into the initial values 
        shakeTimer = FadeInTime;
        Vector3 startPosition = transform.localPosition;
        Vector3 offset = GetPerlinPotion(0);
        Vector3 endPosition = GetPerlinPotion(0);
        Debug.Log("Startposition = "+startPosition+" Endposition "+endPosition);

        // Return camera to start position
        while (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            float t = 1 - shakeTimer / shakeTime;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // Setup Shake
        shakeTimer = shakeTime;

        // Do the camera shake
        while (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            float t = 1 - shakeTimer / shakeTime;
            transform.localPosition = GetPerlinPotion(t);
            yield return null;
        }

        // Setup Fade out 
        shakeTimer = FadeOutTime;
        startPosition = transform.localPosition;
        endPosition = Vector3.zero;

        // Return camera to start position
        while (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            float t = 1 - shakeTimer / shakeTime;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        // Camera has now returned to initial position
    }

    private Vector3 GetAnimationCurvePosition(float percentElapsed)
    {
        float xvalue = shakeStrengthX * (!useDampening  ? 1 : Mathf.Pow(1 - Damper, percentElapsed)) * animationCurve.Evaluate(percentElapsed);
        float yvalue = 0;
        Debug.Log("(X="+xvalue+" Y="+yvalue);
        return new Vector3( xvalue, yvalue , 0);
    }
    private Vector3 GetPerlinPotion(float percentElapsed)
    {
        float xvalue = shakeStrengthX * (!useDampening ? 1 : Mathf.Pow(1 - Damper, percentElapsed)) * (Mathf.PerlinNoise(percentElapsed, YShakePosition) - 0.5f);
        float yvalue = shakeStrengthY * (!useDampening ? 1 : Mathf.Pow(1 - Damper, percentElapsed)) * (Mathf.PerlinNoise(XShakePosition, percentElapsed) - 0.5f);
        Debug.Log("(X="+xvalue+" Y="+yvalue);
        return new Vector3( xvalue, yvalue , 0);
    }
}
