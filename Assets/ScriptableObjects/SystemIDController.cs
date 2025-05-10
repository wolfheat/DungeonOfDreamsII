using UnityEngine;

public class SystemIDController : MonoBehaviour
{
    [SerializeField] SystemID systemID;
    public SystemID SystemID => systemID;


    public static SystemIDController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
