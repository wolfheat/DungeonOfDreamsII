using System;
using UnityEngine;


[CreateAssetMenu(menuName = "CameraShakes/ShakeData", fileName = "BombShake")]
public class CameraShakeControllerData : ScriptableObject
{

    public ShakeType ShakeTypeSelected;

    [Header("General Settings")]
    [Range(0, 5)]
    public  float shakeTime = 0;
    public  float shakeDelay = 0;

    [Range(0, 5)]   
    public  float shakeStrengthX;

    public  bool useDampening = true;

    [Header("Specific Settings")]
    // ANIMATION CURVE
    [ConditionalHide("ShakeTypeSelected", ShakeType.AnimationCurve)]
    public  AnimationCurve animationCurve;


    // PERLIN
    [ConditionalHide("ShakeTypeSelected", ShakeType.Perlin)]
    [Range(0, 1)]
    public  float shakeStrengthY;


    [ConditionalHide("ShakeTypeSelected", ShakeType.Perlin)]
    [Range(0, 1)]
    public  float FadeInTime = 0.05f;

    [ConditionalHide("ShakeTypeSelected", ShakeType.Perlin)]
    [Range(0, 1)]
    public  float FadeOutTime = 0.05f;

    [ConditionalHide("ShakeTypeSelected", ShakeType.Perlin)]
    [Range(0, 1)]
    public  float Damper = 0.05f;

}
