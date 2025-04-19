using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] GameObject red;
    //[SerializeField] Animator animator;
    [SerializeField] GameObject grey;

//    public float AnimationTime { get { return animator.GetCurrentAnimatorStateInfo(0).normalizedTime; }}

    internal void Show(bool show)
    {
        Debug.Log("Showing active gem "+red);

        red.SetActive(show);    
        grey.SetActive(!show);
  //      if (show)
   //         animator.Play("Pulsating",-1,HealthUIController.Instance.AnimationTime);
    }
}
