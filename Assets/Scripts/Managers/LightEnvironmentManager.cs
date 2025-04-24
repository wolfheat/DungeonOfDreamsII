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

	public void SetNormalColor()
	{
        RenderSettings.ambientLight = normalColor;
    }
	
	public void SetBossColor()
    {
        RenderSettings.ambientLight = bossColor;
    }

}
