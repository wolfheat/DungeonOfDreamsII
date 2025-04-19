using UnityEngine;

public class BossWinController : MonoBehaviour
{
    [SerializeField] Wall[] wallsToRemove;


    public static BossWinController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void WinRemoveWalls()
    {
        foreach (var wall in wallsToRemove) {
            wall.Shrink();
        }
    }
}
