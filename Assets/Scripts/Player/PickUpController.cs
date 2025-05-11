using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public Interactable ActiveInteractable { get; set; }
    public Wall Wall { get; set; }
    public Door Door { get; set; }
    public EnemyController Enemy { get; set; }
    public Mock Mockup { get; set; } = null;

    private LayerMask enemyLayerMask;
    private LayerMask mockupLayerMask;
    private LayerMask wallAndDoorLayerMask;
    private LayerMask doorLayerMask;
    private LayerMask itemLayerMask;

    private void Start()
    {
        doorLayerMask = LayerMask.GetMask("Door");
        wallAndDoorLayerMask = LayerMask.GetMask("Wall") | doorLayerMask;
        enemyLayerMask = LayerMask.GetMask("Enemy");
        mockupLayerMask = LayerMask.GetMask("Mock");
        itemLayerMask = LayerMask.GetMask("Items","ItemsSeeThrough") ;
        UpdateColliders();
        StartCoroutine(UpdateCollidersInterval());
    }

    private WaitForSeconds wait = new WaitForSeconds(0.1f);
    private IEnumerator UpdateCollidersInterval()
    {
        while (true)
        {
            UpdateColliders();
            yield return wait;
        }
    }

    public void UpdateColliders(bool wait = false)
    {
        //Debug.Log("* Updating Colliders "+(wait?" after waiting *":"*"));
        UpdateInteractables();
        UpdateWall();        
        UpdateEnemy();        
    }

    public IEnumerator UpdateCollidersWait()
    {
        yield return null;
        yield return null;
        UpdateColliders(true);
        PlayerController.Instance.MotionActionCompleted();
    }

    public void UpdateEnemy()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(Convert.Align(transform.position), Game.boxSize,Quaternion.identity, enemyLayerMask);

        //UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponentInParent<EnemyController>().EnemyData as ItemData).ToList());

        if (colliders.Length == 0)
            Enemy = null;
        else
        {
            EnemyController enemy = colliders[0].gameObject.GetComponentInParent<EnemyController>();
            if (!enemy.Dead)
                Enemy = enemy;
            else 
                Enemy = null;
        }


        // Get enemy mockup
        //Mockup = colliders.Where(x => x.GetComponentInParent<Interactable>() == null).ToArray().Length > 0?true:false;
        colliders = Physics.OverlapBox(Convert.Align(transform.position), Game.boxSize, Quaternion.identity, mockupLayerMask);

        Mock candidate = colliders.Where(x => x.GetComponent<Mock>() != null).ToArray().FirstOrDefault()?.GetComponent<Mock>();
        if (candidate != null && !candidate.IsPlayer)
        {
            //Debug.Log("Found a Mock at position:" + candidate.transform.position + " pos:" + candidate.pos + " name:" + candidate.name);
            Mockup = candidate;
        }
        else
            Mockup = null;
    }
    
    public void UpdateWall()
    {
        // Align box with grid before casting

        Vector3 alignedPos = Convert.Align(transform.position);

        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(alignedPos, Game.boxSize,Quaternion.identity, wallAndDoorLayerMask);


        if (colliders.Length == 0) {
            Wall = null;
            Door = null;
        }
        else if(colliders[0].gameObject.TryGetComponent(out Wall FoundWall)) {
            this.Wall = FoundWall;
            Door = null;
            // Found A wall
        }else if(colliders[0].gameObject.TryGetComponent(out Door FoundDoor)) {
            // Found A wall
             this.Door = FoundDoor;
            Wall = null;
        }
    }
    
    public void UpdateInteractables()
    {
        // Get list of interactable items
        Collider[] colliders = Physics.OverlapBox(Convert.Align(transform.position), Game.boxSize,Quaternion.identity, itemLayerMask);
        
        //UIController.Instance.UpdateShownItemsUI(colliders.Select(x => x.GetComponent<InteractableItem>()?.Data).ToList(),true);
        if (colliders.Length == 0)
        {
            //Debug.LogError("No Interactable found. box centered at "+transform.position+" size "+Game.boxSize);
            ActiveInteractable = null;
        }
        else
        {
            //Debug.Log("Active Interactable set to: " + colliders[0].name);
            ActiveInteractable = colliders[0].gameObject.GetComponent<Interactable>();
        }
    }

    public bool InteractWithEnemy()
    {
        if (Enemy == null && Mockup == null) return false;

        if (Enemy != null)
            Enemy.TakeDamage(Stats.Instance.Damage);
        else if (Mockup.owner.TryGetComponent(out EnemyController enemy))
            enemy.TakeDamage(Stats.Instance.Damage);
        
        UpdateColliders();

        return true;
    }
    public bool InteractWithWall()
    {
        if (Wall == null) return false;

        if (Wall.WallData == null)
        {
            Debug.Log("Interact with Wall without Data = Bedrock");
            return false;
        }
        
        // Lets player hit and destroy walls

        //if(Wall.Damage(Stats.Instance.Damage))
        //    UpdateColliders();

        return true;
    }
}
