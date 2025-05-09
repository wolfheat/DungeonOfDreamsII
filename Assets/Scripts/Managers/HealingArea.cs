using System;
using System.Collections;
using UnityEngine;

public class HealingArea : MonoBehaviour
{

    private bool active = false;
    private const float BombGeneratorTime = 60f;

    [SerializeField] private GameObject bombItem;
    
    private Coroutine coroutine;

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerColliderController player)) {
            ActivateBombGenerator();
        }
    }

    public void DisableGenerator()
    {
        // Disable the coroutine
        if (coroutine != null)
            StopCoroutine(coroutine);
        active = false;

        // Disable the bomb
        if (bombItem.activeSelf)
            bombItem.SetActive(false);
    }
    
    public void ActivateBombGenerator()
    {
        if (active || bombItem.activeSelf) 
            return; // Allready active or has a bomb        

        coroutine = StartCoroutine(Generator());
    }

    private IEnumerator Generator()
    {
        active = true;
        yield return new WaitForSeconds(BombGeneratorTime);
        bombItem.SetActive(true);
        //ItemSpawner.Instance.SpawnSoftlockHolderBombItemAt(this.transform.position+Vector3.up*0.4f);
        active = false;
        coroutine = null;
    }
}
