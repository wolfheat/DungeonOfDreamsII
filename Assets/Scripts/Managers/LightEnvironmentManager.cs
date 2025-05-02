using System;
using System.Collections;
using UnityEngine;

public class LightEnvironmentManager : MonoBehaviour
{

	public static LightEnvironmentManager Instance { get; private set; }

	[SerializeField] private Color32 normalColor; 
	[SerializeField] private Color32 bossColor; 

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	[ContextMenu("Set Normal Environment Color")]
	public void SetNormalColor()
	{
		Debug.Log(" ** ** ** Set Normal Environment Color ** ** ** ");
		StartCoroutine(DelayedEnvironementChange(normalColor));
        RenderSettings.ambientLight = normalColor;

    }

    private IEnumerator DelayedEnvironementChange(Color32 color)
    {
		yield return null;
		Debug.Log(" Delayed ** ** ** Set Normal Environment Color ** ** ** ");
		RenderSettings.ambientLight = color;
    }

    [ContextMenu("Set Boss Environement Color")]
	public void SetBossColor()
    {
		Debug.Log(" ** ** ** Set Boss Environment Color ** ** ** ");
        RenderSettings.ambientLight = bossColor;
    }

}
