using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Wolfheat.Inputs;
using Wolfheat.StartMenu;
using static UnityEngine.InputSystem.InputAction;

public enum MoveActionType{Step,SideStep,Rotate}
public class MoveAction
{
    public MoveActionType moveType;
    public int dir = 0;
    public Vector2Int move;
    public MoveAction(MoveActionType t, int d)
    {
        moveType = t;
        dir = d;
    }
    public MoveAction(MoveActionType t, Vector2Int m)
    {
        moveType = t;
        move = m;
    }
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] Mock playerMock;
    [SerializeField] PlayerAnimationController playerAnimationController;
    [SerializeField] TakeFireDamage takeFireDamage;
    public PickUpController pickupController;
    public bool DoingAction { get; set; } = false;
    private MoveAction savedAction = null;
    private MoveAction lastAction = null;

    private float timer = 0;
    private const float MoveTime = 0.2f;    

    public Action PlayerReachedNewTile;
    public Action<Vector2Int> MovedToNewSquare;
    public static PlayerController Instance { get; private set; }
    public bool IsDead { get { return Stats.Instance.IsDead; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // set up input actions
        //Inputs.Instance.Controls.Player.Move.performed += NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed += Step;
        Inputs.Instance.Controls.Player.SideStep.performed += SideStep;
        Inputs.Instance.Controls.Player.Turn.performed += TurnPerformed;    
        Inputs.Instance.Controls.Player.Click.performed += InterractWith;   
        Inputs.Instance.Controls.Player.RightClick.performed += RightClick;
        Inputs.Instance.Controls.Player.Y.performed += InstantDeath;
        TakeFireDamage.PlayerTakeFireDamage += FireDamage;   

        playerAnimationController.HitComplete += HitWithTool;
            
    }

    private void OnDisable()
    {
        //Inputs.Instance.Controls.Player.Move.performed -= NewMoveInput;
        Inputs.Instance.Controls.Player.Step.performed -= Step;
        Inputs.Instance.Controls.Player.SideStep.performed -= SideStep;
        Inputs.Instance.Controls.Player.Turn.performed -= TurnPerformed;
        Inputs.Instance.Controls.Player.Click.performed -= InterractWith;
        Inputs.Instance.Controls.Player.RightClick.performed -= RightClick;   
        playerAnimationController.HitComplete -= HitWithTool;
        TakeFireDamage.PlayerTakeFireDamage -= FireDamage;
    }


    public void InstantDeath(CallbackContext context)
    {
        Debug.Log("Instant Death");
        TakeDamage(10);
    }
    public void RightClick(CallbackContext context)
    {
        Debug.Log("Right Click Place Bomb");
        PlaceBomb();
    }

    private void PlaceBomb()
    {
        // Removed DoingAction to be able to place bomb when moving
        if (playerAnimationController.IsAttacking || IsDead)
        {
            Debug.Log("Cant place Bomb, doing action or Dead");
            return;
        }

        Debug.Log("PLACE NEW BOMB");
        if (Inventory.Instance.BombsHeld <= 0)
        {
            Debug.Log("You Got No Bombs");
            SoundMaster.Instance.PlaySound(SoundName.NoBombs);
            return;
        }

        // Change target to be centered
        
        Vector3 target = Convert.Align(transform.position + transform.forward);
        if (LevelCreator.Instance.TargetHasWall(target) == null && !LevelCreator.Instance.TargetHasPlacedBomb(target))
        {
            Debug.Log("No Walls or Enemies ahead - Place bomb at "+target+" player at "+transform.position);
            
            SoundMaster.Instance.PlaySound(SoundName.DropItem);
            ItemSpawner.Instance.PlaceBomb(target);
            bool hadBomb = Inventory.Instance.RemoveBombs();
            
            if (Inventory.Instance.BombsHeld == 0)
            {
                //SoundMaster.Instance.PlaySound(SoundName.ThatWasTheLastOne);
                Debug.Log("LAST ONE");
            }
        }
        else
        {
            // Something is in the way
            Debug.Log("CANT DO THAT");
            //SoundMaster.Instance.PlaySound(SoundName.CantDoThat);
        }
    }

    public void InterractWith(CallbackContext context)
    {
        InterractWith();
    }
    

    public void InterractWith()
    {
        if (Stats.Instance.IsDead) return;
        pickupController.UpdateColliders();

        // Disable interact when inventory
        //if (UIController.CraftingActive || UIController.InventoryActive || GameState.IsPaused)

        // Check if item exists to pick up
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Interacting over UI element");
            return;
        }

        //toolHolder.ChangeTool(DestructType.Breakable);


        // Interact with closest visible item 
        if (pickupController.ActiveInteractable != null)
        {
            pickupController.InteractWithActiveItem();
        }
        else
        {
            if (pickupController.Wall != null)
            {
                if (!Stats.Instance.HasSledgeHammer && pickupController.Wall.GetComponent<Door>() != null)
                {
                    SoundMaster.Instance.PlaySound(SoundName.ICantBreakThisWithMyBareHands);
                    return;
                }
                else if (pickupController.Wall.gameObject.TryGetComponent(out Altar altar)) {
                    Debug.Log("Interacting with Altar");     
                }
                else if (pickupController.Wall.gameObject.TryGetComponent(out Gloria gloria))
                {
                    Debug.Log("ACTIVATE GLORIA COMPLETION");
                    gloria.ActivateCompletion();   
                    
                }
                else {
                    playerAnimationController.SetState(PlayerState.Hit);
                    Debug.Log("Setstate HIT");

                }
                Debug.Log("Wall "+pickupController.Wall?.name);
            }
            else if (pickupController.Enemy != null)
            {
                Debug.Log("Player has an Enemy in front "+pickupController.Enemy, pickupController.Enemy);
                playerAnimationController.SetState(PlayerState.Attack);
            }
            else if (pickupController.Mockup != null)
            {
                Debug.Log("Hit Enemy Mock "+pickupController.Mockup.name, pickupController.Mockup); 
                playerAnimationController.SetState(PlayerState.Attack);
            }

            //else Debug.Log("No Block to crush");
        }

    }

    public void HitWithTool()
    {
        if (Stats.Instance.IsDead) return;

        if(!pickupController.InteractWithWall() && !pickupController.InteractWithEnemy()) {
            Debug.Log("Player has no wall or enemy ahead, go to IDLE");
            playerAnimationController.SetState(PlayerState.Idle);
        }

        // If player has mouse button down attack again?
        if (!Inputs.Instance.Controls.Player.Click.IsPressed() || (pickupController.Wall == null && pickupController.Enemy == null && pickupController.Mockup == null)) {
            Debug.Log("Player is not holding mouse (or there is no wall enemy or mockup ahead), go to IDLE");
            playerAnimationController.SetState(PlayerState.Idle);
        }

    }


    // ---------------------------------------------


    private void Update()
    {
        if(DoingAction) return;

        if (savedAction != null)
        {
            if (savedAction.moveType == MoveActionType.Step || savedAction.moveType == MoveActionType.SideStep)
            {

                Vector3 target = EndPositionForMotion(savedAction);
                
                //Debug.Log("Executing step movement = " + savedAction.moveType+" target is "+target);

                if (!LevelCreator.Instance.Occupied(target) && Mocks.Instance.IsTileFree(Convert.V3ToV2Int(target)))
                {
                    lastAction = savedAction;
                    //Debug.Log("Storing saved Movement as Last movement and start new movement");
                    StartCoroutine(Move(target));
                }
                else
                {
                    CenterPlayerPosition();
                    Debug.Log("Walls or Enemies ahead");

                    Door door = LevelCreator.Instance.TargetHasDoor(target);
                    Altar altar = LevelCreator.Instance.TargetHasAltar(target);
                    // If door is ahead unlock it if player has correct key
                    if (door != null) {

                        if (door.IsBossDoor) {
                            Debug.Log("Boss Door place gems");


                            if (door.TryGetComponent<BossDoor>(out BossDoor bossDoor)) {
                                if (bossDoor.IsUnlocked) {
                                    Debug.Log("Opening Boss Door");
                                    door.OpenDoor();
                                }
                                if (bossDoor.PlaceGems()) {
                                    Debug.Log("Player Placed all his gems. Unlocked:");
                                }
                            }
                        }
                        else {
                            bool playerCanUnlock = Inventory.Instance.KeysHeld > 0;
                            Debug.Log("DOOR AHEAD keys: "+Inventory.Instance.KeysHeld);

                            if (playerCanUnlock) {
                                Debug.Log("Unlocked");
                                door.OpenDoor();

                                Inventory.Instance.RemoveKey();
                            }
                            else {
                                SoundMaster.Instance.PlaySound(SoundName.LockedDoor);
                            }
                        }
                    }else if (altar != null) {
                        Debug.Log("There is an altar here, open its shop Menu");
                        Debug.Log("Menu "+altar.MineralAccepted);

                        Debug.Log("Entering Shop");
                        Shop.Instance.ShowPanel(altar.MineralAccepted);
                    }

                }

            }
            else if (savedAction.moveType == MoveActionType.Rotate)
            {
                lastAction = savedAction;
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));
            }

            // Remove last attempted motion
            savedAction = null;
        }
    }


    private void TurnPerformed(InputAction.CallbackContext obj)
    {
        TurnPerformed();
    }
    private bool TurnPerformed()
    {
        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused

        float movement = Inputs.Instance.Controls.Player.Turn.ReadValue<float>();
        if (movement == 0) return false;

        MoveAction moveAction = new MoveAction(MoveActionType.Rotate, (int)movement);
        savedAction = moveAction;
        return true;
    }

    private void SideStep(CallbackContext obj)
    {
        SideStep();
    }
    
    private void Step(CallbackContext obj)
    {
        Step();
    }

    private bool SideStep()
    {
        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused

        // Return if no movement input currently held 
        float movement = Inputs.Instance.Controls.Player.SideStep.ReadValue<float>();
        if (movement == 0) return false;

        // Write or overwrite next action
        MoveAction moveAction;
        moveAction = new MoveAction(MoveActionType.SideStep, Mathf.RoundToInt(movement));
        savedAction = moveAction;
        return true;
    }
    

    private bool Step()
    {
        //Debug.Log(" Player position " + transform.position);

        if (GameState.state == GameStates.Paused || Stats.Instance.IsDead) return false; // No input while paused


        // Return if no movement input currently held 
        float movement = Inputs.Instance.Controls.Player.Step.ReadValue<float>();
        
        //Debug.Log("Adding step movement = " + movement);

        if (movement == 0) return false;

        // Write or overwrite next action
        MoveAction moveAction;
        moveAction = new MoveAction(MoveActionType.Step, Mathf.RoundToInt(movement));
        savedAction = moveAction;
        return true;
    }


    private void HeldMovementInput()
    {
        // Check if player is holding a movement button
        if (Step())
        {
            // If held button is not same as last input center player
            if (savedAction != null && savedAction.moveType != lastAction.moveType)
                CenterPlayerPosition();
            return;
        }
        if(SideStep())
        {
            // If held button is not same as last input center player
            if (savedAction != null && savedAction.moveType != lastAction.moveType)
                CenterPlayerPosition();
            return;
        }
        CenterPlayerPosition();
        // Check for interact


        return;
    }

    private void CenterPlayerPosition()
    {
        //Debug.Log("Center player "+transform.position);
        transform.position = Convert.Align(transform.position);
        //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        //Debug.Log("Centered player " + transform.position);

    }

    private IEnumerator Move(Vector3 target)
    {
        int stepSoundFromTerrain = TerrainChecker.ProminentTerrainType(transform.position,LevelCreator.Instance.ActiveTerrain);
        SoundMaster.Instance.PlayStepSound(stepSoundFromTerrain);

        // Place mock
        PlaceMock(target);

        DoingAction = true;
        Vector3 start = transform.position;
        Vector3 end = target;
        timer = 0;
        while (timer < MoveTime*Stats.Instance.MovingSpeedMultiplier)
        {
            yield return null;
            transform.position = Vector3.LerpUnclamped(start,end,timer/(MoveTime*Stats.Instance.MovingSpeedMultiplier));
            timer += Time.deltaTime;
        }
        //Debug.Log("Moving player "+(transform.position-target).magnitude);
        //transform.position = target;
        DoingAction = false;

        MotionActionCompleted();

    }

    private Vector3 EndPositionForMotion(MoveAction motion)
    {
        // Round the answer
        Vector3 target = transform.position + motion.dir * (motion.moveType == MoveActionType.Step ? transform.forward : transform.right);
        target = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
        return target;
    }

    private Quaternion EndRotationForMotion(MoveAction motion)
    {
        return Quaternion.LookRotation(transform.right * motion.dir, Vector3.up);
    }

    private IEnumerator Rotate(Quaternion target)
    {
        DoingAction = true;
        Quaternion start = transform.rotation;
        Quaternion end = target;
        timer = 0;
        while (timer < MoveTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(start,end,timer/MoveTime);
            timer += Time.deltaTime;
        }
        transform.rotation = target;
        DoingAction = false;
        MotionActionCompleted();
    }

    public void UpdateInputDelayed()
    {
        StartCoroutine(UpdatePlayerInputCO());
    }
    
    public IEnumerator UpdatePlayerInputCO()
    {
        yield return null;
        UpdatePlayerInput();
    }
    
    public void UpdatePlayerInput()
    {
        if (Stats.Instance.IsDead) return;

        // Check to align player
        if (savedAction != null && savedAction.moveType != lastAction.moveType)
            CenterPlayerPosition();

        // Player has no movement saved check if button is held
        if (savedAction == null)
            HeldMovementInput();

        pickupController.UpdateColliders();

        //if (Inputs.Instance.Controls.Player.Click.IsPressed() && pickupController.Wall != null)
        if (Inputs.Instance.Controls.Player.Click.IsPressed())
        {
            CenterPlayerPosition();
            Debug.Log("Mouse is held, interact");
            InterractWith();
        }

    }
    public void MotionActionCompleted()
    {
        if (Stats.Instance.IsDead) return;

        //Debug.Log("Motion completed, has stored action: "+savedAction);
        PlayerReachedNewTile?.Invoke();
        MovedToNewSquare?.Invoke(Convert.V3ToV2Int(transform.position));
        UpdatePlayerInput();
    }

    private void FireDamage(int amt)
    {
        //Debug.Log("Player take fire damage from burning "+amt);
        TakeDamage(amt,null,true);
    }

    public void TakeDamage(int amt,EnemyController enemy = null, bool bombDamage = false)
    {
        if (Stats.Instance.IsDead) return;
                
        if (Stats.Instance.TakeDamage(amt))
        {
            // If there is an enemy that killed player turn to look at it when dying
            if(enemy != null)
                StartCoroutine(Rotate(Quaternion.LookRotation(enemy.transform.position - transform.position)));

            // Set player back to idle 
            playerAnimationController.SetState(PlayerState.Idle);

            // Show death screen
            UIController.Instance.ShowDeathScreen();
            SoundMaster.Instance.PlaySound(bombDamage?SoundName.BoomPlayerDies:SoundName.DieByFire);
        }
        else
        {
            // Player still alive
            SoundMaster.Instance.PlayGetHitSound();
        }
    }


    public void OxygenDeath()
    {
        Stats.Instance.IsDead = true;

        Stats.Instance.OxygenHealthRemoval();
            
            
        SoundMaster.Instance.StopMusic();

        // Set player back to idle 
        playerAnimationController.SetState(PlayerState.Idle);

        // Show death screen
        UIController.Instance.ShowDeathScreenInstant();
        //SoundMaster.Instance.PlaySound(bombDamage?SoundName.BoomPlayerDies:SoundName.DieByFire);
        SoundMaster.Instance.StopMusic();

        StopAllCoroutines();
        DoingAction = false;
    }   


    public void GotoNextStartPosition()
    {
        Debug.Log("NEXT POSITIOON");
        if (Stats.Instance.GetNextStartPosition()) {
            UIController.Instance.ShowWinScreen();
        }
        else {
            ResetPlayerPosition();
            Stats.Instance.DeActivateMap();
        }
    }

    public void Reset()
    {
        Debug.Log("Reset Player");

        ResetPlayerPosition(); 
        
        Stats.Instance.Revive();
        PlaceMock(transform.position);
    }

    private void ResetPlayerPosition()
    {
        // Setting PLayer to init position with forward rotation

        // Make sure the coroutines are stopped
        StopAllCoroutines();

        transform.position = new Vector3(Stats.Instance.SavedStartPosition.x, 0 ,Stats.Instance.SavedStartPosition.z);
        Debug.Log("Player moved to "+transform.position);

        transform.rotation = Quaternion.identity;

        Debug.Log(" Player position " + transform.position);

        savedAction = null;
        lastAction = null;
        DoingAction = false;

        takeFireDamage.StopFire();
    }

    private void PlaceMock(Vector3 position)
    {
        playerMock.pos = Convert.V3ToV2Int(position);        
        playerMock.transform.position = position;
    }

    internal void GotoStartPosition(int leadsTo)
    {
        Debug.Log("Specific position");
        Stats.Instance.SetSpecificPosition(leadsTo);
        ResetPlayerPosition();
        Stats.Instance.DeActivateMap();
    }
}
