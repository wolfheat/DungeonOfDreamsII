using UnityEngine;

public class EndRoom : MonoBehaviour
{
    [SerializeField] private GameObject[] bananas;

	public static EndRoom Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void ActivateBananas(bool doActivate = true)
	{
		foreach (var banana in bananas) {
			banana.SetActive(doActivate);
		}
	}

}
