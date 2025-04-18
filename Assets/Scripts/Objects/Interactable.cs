using UnityEngine;
using Wolfheat.StartMenu;
public class Interactable : MonoBehaviour
{
    protected ParticleType particleType = ParticleType.PickUp;
    protected SoundName soundName = SoundName.PickUp;
    [SerializeField] protected bool usePool = true;
    [SerializeField] protected SoundName soundNameOverride = SoundName.None;

    public virtual void InteractWith()
    {        
        SoundMaster.Instance.PlaySound(soundNameOverride == SoundName.None ? soundName : soundNameOverride);
        ParticleEffects.Instance.PlayTypeAt(particleType, transform.position);
        if(usePool)
            ItemSpawner.Instance.ReturnItem(this);
        gameObject.SetActive(false);
    }
}
