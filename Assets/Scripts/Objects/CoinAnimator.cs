using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoinAnimator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;
    private int totalFrames = 0;

    private int activeSprite = 0;

    private WaitForSeconds spinSpeed = new WaitForSeconds(0.08f);

    private void Start()
    {
        totalFrames = sprites.Length;
        StartCoroutine(SpinTheCoin());
    }

    private IEnumerator SpinTheCoin()
    {
        while (true) {
            yield return spinSpeed;
            activeSprite = (activeSprite + totalFrames + 1) % totalFrames;
            image.sprite = sprites[activeSprite];
        }
    }
}
