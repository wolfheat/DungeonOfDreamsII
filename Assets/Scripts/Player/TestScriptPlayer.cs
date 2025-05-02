using UnityEngine;

public class TestScriptPlayer : MonoBehaviour
{
    [SerializeField] GameObject terget;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Quaternion rotations = Quaternion.LookRotation(terget.transform.position - transform.position,-Vector3.forward);

        // Figure out angle to target
        Debug.Log("Rotations are "+rotations.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
