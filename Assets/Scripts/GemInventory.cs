using UnityEngine;

public class GemInventory : MonoBehaviour
{
    [SerializeField] private Gem[] gems; 

    public void ActiveateGem(int index)
    {
        Debug.Log("Show gem index "+index);
        Debug.Log("Size "+gems.Length);
        Debug.Log("item " + gems[index]);
        Debug.Log("item " + gems[index].name);
        gems[index].Show(true);
    }
    public void InactivateAllGems()
    {
        foreach (var gem in gems) gem.Show(false);
    }

}
