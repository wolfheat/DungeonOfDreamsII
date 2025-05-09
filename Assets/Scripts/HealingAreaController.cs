using UnityEngine;

public class HealingAreaController : MonoBehaviour
{
	public static HealingAreaController Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void DisableAllHealingAreas()
	{
		foreach (Transform t in transform.GetComponentInChildren<Transform>()) {
			if(t.TryGetComponent(out HealingArea healingArea)) {
				healingArea.DisableGenerator();
			}
		}
	} 



}
