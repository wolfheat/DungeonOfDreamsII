using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    [SerializeField] private int gotoSpecific = -1;

    public int LeadsTo => gotoSpecific;
}
