using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;


public class EnemyController : Interactable
{
    public bool IsBoss = false;// { get; set; }
    public EnemyData EnemyData;// { get; set; }

    [SerializeField] BarController healthBar;
    [SerializeField] Collider enemyCollider;
    [SerializeField] LayerMask playerLayerMask;

    PlayerController player;
    [SerializeField] private float playerDistance;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask obstructions;
    [SerializeField] Mock mock;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] int EnemySight = 10;

    private float timer = 0;
    private const float MoveTime = 2f;
    private const float RotateTime = 0.4f;
    public bool DoingMovementAction { get; set; } = false;

    private EnemyStateController enemyStateController;

    private Stack<Vector2Int> path = new Stack<Vector2Int>();

    private Vector2Int playerLastPosition = Vector2Int.zero;


    public int Health { get; private set; }
    public bool Dead { get; private set; }

    [SerializeField] public bool Activated = true;
    [SerializeField] public int StartHealth = 3;
    [SerializeField] private Vector3 StartPosition;

    private float FirestormTimeout = 3f;
    private float FirestormTimer = 3f;

    private float PassiveTimeout = 8f;
    private float PassiveTimer = 8f;

    private int attackCounter = 0;
    private int attackCounterMax = 6;

    private void OnEnable()
    {
        Health = StartHealth; // Change to data health later
        Dead = false;
        enemyStateController.ChangeState(EnemyState.Idle,true);
        path.Clear();
        DoingMovementAction = false;
        mock.gameObject.SetActive(true);
        EnableColliders();
        StartPosition = transform.position;
    }

    public void DisableColliders()
    {
        //Debug.Log("Disabling all Enemys colliders ",this);
        if (enemyCollider != null)
            enemyCollider.enabled = false;
        if (mock != null)
        {
            //Debug.Log("Disable Mock object for ",this);
            mock.gameObject.SetActive(false);
        }
        player?.UpdateInputDelayed();
    }
    private void EnableColliders()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = true;
        if (mock != null)
            mock.gameObject.SetActive(true);
    }

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        enemyStateController = new EnemyStateController(animator);
        
    }

    private void Start()
    {
        mock.transform.parent = LevelCreator.Instance.mockHolder?.transform;
        PlaceMock(transform.position,false);
    }

    private MoveAction savedAction = null;
    private void Update()
    {
        if (!Activated || Stats.Instance.IsDead) return;

        if (PassiveTimer > 0) {
            PassiveTimer -= Time.deltaTime;
        }
        if (PassiveTimer <= 0) {

            enemyStateController.ChangeState(EnemyState.ThrowAttack);
            //path.Clear();
            savedAction = null;
            return;
        }

        if (FirestormTimer > 0 && enemyStateController.currentState != EnemyState.FireStorm)
            FirestormTimer -= Time.deltaTime;

        // Limit Enemy from doing anything new if allready doing an action or is dead
        if (DoingMovementAction || Dead) return;
        // Check if there is a stored action to perform
        if (savedAction != null)
        {
            // The unit has a stored movement

            // Step movement is stored
            if (savedAction.moveType == MoveActionType.Step)
            {
                Vector3 target = Convert.V2IntToV3(savedAction.move);
                if (!LevelCreator.Instance.Occupied(target) && Mocks.Instance.IsTileFree(Convert.V3ToV2Int(target)))
                {
                    //Debug.Log(this.name+"Moving to next path point it is not blocked "+target+" ",this);
                    StartCoroutine(Move(target));
                }
                else
                {
                    // Position is blocked
                    // remove actions if have a path then remove the path, need to figure out something else to do
                    if (path.Count > 0)
                    {
                        ForgetPath();
                        savedAction = null;
                        //Debug.Log(this.name + "Forgeting path and updating it, path is blocked",this);
                        //Debug.Log(this.name + " Set to Idle state", this);
                        enemyStateController.ChangeState(EnemyState.Idle);
                        // Issue here that enemy gets the same path since the position ahead is not considered occupied from the level array but from boxcasting

                        UpdatePlayerDistanceAndPath();
                    }
                    else
                    {
                        Debug.Log(this.name + " Path is blocked have no path savedaction = "+savedAction?.moveType+" move:"+ savedAction?.move+" dir: "+savedAction?.dir+" ", this);
                        if (savedAction != null)
                            savedAction = null;
                        Debug.Log(this.name + " Set to Idle state",this);
                        enemyStateController.ChangeState(EnemyState.Idle);
                    }
                    return;
                }
            }
            // Rotate movement is stored
            else if (savedAction.moveType == MoveActionType.Rotate)
                StartCoroutine(Rotate(EndRotationForMotion(savedAction)));

            // Motion is handled, remove it
            savedAction = null;
        }
        // No action stored
        else if (enemyStateController.currentState == EnemyState.Idle && PlayerHasNewPosition())
        {
            //Debug.Log("Enemy is idle, check everytime player moves to update stored position");
            // Player Moved 
            if (!Activated) return;
            if(!Stats.Instance.IsDead && UpdatePlayerPosition())
                UpdatePlayerDistanceAndPath();
            if (path.Count > 0)
                ActivateNextPoint();
        }else if ((enemyStateController.currentState == EnemyState.Attack || enemyStateController.currentState == EnemyState.FireStorm || enemyStateController.currentState == EnemyState.ThrowAttack )&& player.IsDead)
        {
            // Player is dead
            ForgetPath();
            enemyStateController.ChangeState(EnemyState.Idle);
        }
    }

    private void ForgetPath() => path.Clear();
    private Quaternion EndRotationForMotion(MoveAction motion) => Quaternion.LookRotation(Convert.V2IntToV3(motion.move) - transform.position, Vector3.up);
    private Vector3 GetNextStepTarget() => path.Count == 0 ? Vector3.zero : Convert.V2IntToV3(path.Pop());
    public void Remove() => ItemSpawner.Instance.ReturnEnemy(this);
    private bool PlayerIsInLookingDirection() => transform.forward == (player.transform.position - transform.position).normalized;


    // SKELETON SPECIFICS
    private bool NormalBehaviour()
    {
        //Debug.Log("This is a Skeleton");
        playerDistance = PlayersLastPositionDistance();
        if (playerDistance < 1.1f)
        {
            // if next to player but not facing player rotate towards player
            if (!PlayerIsInLookingDirection())
            {
                savedAction = new MoveAction(MoveActionType.Rotate, playerLastPosition);
                return true;
            }
            // remove any path
            ForgetPath();
            enemyStateController.ChangeState(EnemyData.enemyType == EnemyType.Bomber ? EnemyState.Exploding : EnemyState.Attack);
            return true;
        }
        else if (path.Count == 0)
        {            
            if (enemyStateController.currentState != EnemyState.Dying && enemyStateController.currentState != EnemyState.Dead)
                enemyStateController.ChangeState(EnemyState.Idle);

            return true;
        }else
        {
            //Debug.Log("Player is not close enough to attack, but there is a path");
        }
        return false;
    }

    // CAT SPECIFICS
    private bool CatBehaviour()
    {
        Debug.Log("ENEMY Cat behaviour - current state: "+enemyStateController.currentState);
        // Prohibit state to change if cat is attacking
        if(enemyStateController.currentState == EnemyState.Attack || enemyStateController.currentState == EnemyState.ThrowAttack || enemyStateController.currentState == EnemyState.FireStorm)
            return true; 

        // Check if player is on same X or Z coordinate
        if (PlayerOnSameGridCross() && PlayerVisibleForEnemy())
        {
            // if next to player but not facing player rotate towards player
            if (!PlayerIsInLookingDirection())
            {
                savedAction = new MoveAction(MoveActionType.Rotate, playerLastPosition);
                return true;
            }
            if(attackCounter < attackCounterMax)
                enemyStateController.ChangeState(EnemyState.Attack);
            else
                enemyStateController.ChangeState(EnemyState.FireStorm);

            //path.Clear();
            savedAction = null;

            return true;
        }
        else if (FirestormTimer <= 0 && PlayerDistance() < 4)
        {
            Debug.Log("Player is closer than 4 do fireCircle");
            enemyStateController.ChangeState(EnemyState.FireStorm);
            //path.Clear();
            savedAction = null;

            return true;
        }
        /*
        else if (PassiveTimer <= 0)
        {
            Debug.Log("ENEMY - Passive timer ended - do Throw attack");
            enemyStateController.ChangeState(EnemyState.ThrowAttack);

            // Need to make sure enemy does not get stuck going to this change state
            Debug.Log(" Save action was "+savedAction?.moveType);
            Debug.Log(" Setting savedaction = null");

            // getting stuck in walk animation here

            //path.Clear();
            savedAction = null;

            return true;
        }*/
        else if (HasPath())
        {
            ActivateNextPoint();
            return true;
        }
        else
        {
            Debug.Log("Changing enemystate for cat to idle");
            enemyStateController.ChangeState(EnemyState.Idle);
        }
        return false;
    }

    // BOMBER SPECIFICS
    public void Explode()
    {
        //Debug.Log("Enemy Explodes");
        DisableColliders();
        Explosion.Instance.ExplodeNineAround(ParticleType.Explode, transform.position);        
        //SoundMaster.Instance.PlaySound(SoundName.RockExplosion);
        StopAllCoroutines();
    }
    // -------------------------------

    public void ActionCompleted()
    {
        // Have new saved action updated with motion
        //Debug.Log(" * Action completed * Moved or Rotated to end up here ");

        // Exploding disregard player
        if (enemyStateController.currentState == EnemyState.Exploding) return;

        // Player is dead go to idle
        if (Stats.Instance.IsDead)
        {
            enemyStateController.ChangeState(EnemyState.Idle);
            return;
        }

        // Attacking disregard player if not skeleton
        if (enemyStateController.currentState == EnemyState.Attack && EnemyData.enemyType != EnemyType.Skeleton) return;

        // Update path to player if player is alive and got a new Position
        if (!Stats.Instance.IsDead && UpdatePlayerPosition())
        {
            Debug.Log(this.name+" Action complete - Player change position",this);
            UpdatePlayerDistanceAndPath();
        }//else
            //Debug.Log("Action complete - Player did not change position");  

        if(EnemyData.enemyType != EnemyType.Cat && PlayersLastPositionDistance() < 1.1f)
            ForgetPath();  

        switch (EnemyData.enemyType)
        {
            case EnemyType.Bomber:
            case EnemyType.Skeleton:
            case EnemyType.Dino:
                if (HasPath())
                {
                    //Debug.Log("Activate next point");
                    ActivateNextPoint();
                    //Debug.Log("saved action is now "+savedAction?.moveType+""+savedAction?.move);
                }
                else
                    NormalBehaviour();
                break;
            case EnemyType.Cat:
                if (CatBehaviour())
                    return;
                break;
            default: 
                break;
        }
    }


    private bool UpdatePlayerPosition()
    {
        bool changing = LevelCreator.Instance.PlayersLastPosition != playerLastPosition;
        //Debug.Log("Updating players last position from "+playerLastPosition+" to "+ LevelCreator.Instance.PlayersLastPosition+" changed: "+changing);
        playerLastPosition = LevelCreator.Instance.PlayersLastPosition;
        return changing;
    }

    private bool PlayerHasNewPosition() => LevelCreator.Instance.PlayersLastPosition != playerLastPosition;
    private bool EnemyFacingDirection(Vector2Int lookPoint) => Convert.V3ToV2Int(transform.position + transform.forward) == lookPoint;
    private bool HasPath() => path != null && path.Count > 0;
    private void ActivateNextPoint()
    {
        savedAction = EnemyFacingDirection(path.Peek()) ? new MoveAction(MoveActionType.Step, path.Pop()) : new MoveAction(MoveActionType.Rotate, path.Peek());
        //Debug.Log(this.name+" Activating next point "+savedAction?.moveType+" "+savedAction?.move);
    }

    public void Reset()
    {
        if (transform.position == StartPosition) return;
        Activated = false;
        savedAction = null;
        StopAllCoroutines();
        transform.position = StartPosition;
        PlaceMock(StartPosition);
        DoingMovementAction = false;
        Health = StartHealth;
        healthBar.SetBar(Health, StartHealth);
        UIController.Instance.ShowBossHealth(false);

        enemyStateController.ChangeState(EnemyState.Idle);
    }

    public IEnumerator Move(Vector3 target)
    {
        // Also change animation here?
        // Always want player to animate walk when moving?
        enemyStateController.ChangeState(EnemyState.Chase);

        // Lock action from enemy
        DoingMovementAction = true; 

        Vector3 start = transform.position;
        Vector3 end = target;
        PlaceMock(end);        

        timer = 0;
        while (timer < MoveTime/moveSpeed)
        {
            yield return null;
            transform.position = Vector3.LerpUnclamped(start, end, timer / (MoveTime / moveSpeed));
            timer += Time.deltaTime;
        }

        DoingMovementAction = false;
        ActionCompleted();
    }

    private void PlaceMock(Vector3 position,bool noticePlayer = true)
    {
        mock.pos = Convert.V3ToV2Int(position);
        mock.transform.position = position;
        if(noticePlayer)
            player?.UpdateInputDelayed();
    }

    private IEnumerator RotateLockOnPlayer()
    {
        while (true)
        {
            yield return null;
            transform.rotation = Quaternion.LookRotation(player.transform.position-transform.position,Vector3.up);
        }
    }
    private IEnumerator Rotate(Quaternion target)
    {
        EnemyState beginState = enemyStateController.currentState;
        enemyStateController.ChangeState(EnemyState.Rotate);

        DoingMovementAction = true;
        Quaternion start = transform.rotation;
        Quaternion end = target;
        timer = 0;
        while (timer < RotateTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(start, end, timer / RotateTime);
            timer += Time.deltaTime;
        }
        transform.rotation = end;

        if(enemyStateController.currentState != EnemyState.Dying && enemyStateController.currentState != EnemyState.Dead && enemyStateController.currentState != EnemyState.Exploding)
            enemyStateController.ChangeState(beginState);

        DoingMovementAction = false;
        ActionCompleted();
    }


    private void UpdatePlayerDistanceAndPath()
    {
        //Debug.Log(this.name+" - UpdatePlayerDistanceAndPath",this);    
        if (Dead || enemyStateController.currentState == EnemyState.Exploding || Stats.Instance.IsDead)
            return;
        
        // Update player distance when player or enemy reaches a new position
        if (EnemyData.enemyType == EnemyType.Bomber && CheckForExplosion())
            return;

        // If Player close enough and valid path to player chase player        
        playerDistance = PlayersLastPositionDistance();
                
        // If visible and close enough get path
        if (playerDistance < EnemySight && PlayerVisibleForEnemy())
        {
            Debug.Log(this.name+" - gets new Path to player",this);
            GetPath();
        }
        else
        {
            Debug.Log("Player not visible");
            // This clears the path if player is to far away and not visible
            if (path.Count > 0){
                //Debug.Log(this.name + " - forgets Path (not visible to to far)", this);
                ForgetPath();
                // Set to idle
                enemyStateController.ChangeState(EnemyState.Idle);
            }
        }

    }

    private void GetPath() => path = LevelCreator.Instance.CanReach(this, player);

    private bool PlayerVisibleForEnemy()
    {
        Vector3 rayDirection = (player.transform.position - transform.position).normalized * playerDistance;
        Ray ray = new Ray(transform.position, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, EnemySight, obstructions))
        {
            Collider collider = hit.collider;

            //Hit player
            return collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        // Hit nothing
        return false;
    }

    public void LoadUpAttack()
    {
        //Debug.Log("Skeleton loades to attack");
        SoundMaster.Instance.PlaySound(SoundName.SkeletonBuildUpAttack);
    }

    public void SpellFirestormSoundTrigger()
    {
        SoundMaster.Instance.PlaySound(SoundName.EnemyHitGroundEffect);
        CameraShakeController.Instance.Shake(ShakeDataType.BossLand);
    }
    public void SpellFirestormCastOccured()
    {
        //Debug.Log("Spell cast by Cat");
        attackCounter = 0;
        // Create Wildfire Object from cat
        ItemSpawner.Instance.SpawnFireStormAt(transform.position,transform.forward);

        FirestormTimer = FirestormTimeout;
    }
    
    public void SpellCastOccured()
    {
        //Debug.Log("Spell cast by Cat");
        attackCounter++;

        // Create Wildfire Object from cat
        ItemSpawner.Instance.SpawnWildfireAt(transform.position,transform.forward);
    }
    
    public void SpellCastThrowOccured()
    {
        attackCounter++;
        // Create Wildfire Object from cat
        ItemSpawner.Instance.SpawnWildfireAt(transform.position,transform.forward);

        ItemSpawner.Instance.SpawnWildfireAt(transform.position,(transform.right + transform.forward));
        ItemSpawner.Instance.SpawnWildfireAt(transform.position,(-transform.right + transform.forward));
    }
    
    public void SpellCastAnimationComplete()
    {
        Debug.Log("Spell cast animation completed by Cat, go to Idle");
        enemyStateController.ChangeState(EnemyState.Idle);
        PassiveTimer = PassiveTimeout;

        Debug.Log("Going to Catbehaviour from spell casting complete");
        CatBehaviour();
    }

    public void PerformAttack()
    {
        //Debug.Log("Skeleton performes attack");

        // Attack entire square infront of enemy if player is there its hit
        Vector3 pos = transform.position + transform.forward;

        Collider[] colliders = Physics.OverlapBox(pos, Game.boxSize, Quaternion.identity, playerLayerMask);
        if (colliders.Length > 0)
        {
            //Debug.Log("Enemy Hit Player");
            SoundMaster.Instance.PlaySound(SoundName.EnemyStabs);
            player.TakeDamage(1,this);
        }

    }
    EnemyState enemyGetHitBeginState = EnemyState.Idle;
    public void TakeDamageCompleted()
    {
        enemyStateController.ChangeState(enemyGetHitBeginState);
    }

    public void NormalAttackCompleted()
    {
        if (!Stats.Instance.IsDead && UpdatePlayerPosition())
        {
            enemyStateController.ChangeState(EnemyState.Idle);
            UpdatePlayerDistanceAndPath();
            Debug.Log("Action Complete - Player Path updated");
            if (path.Count > 0)
                ActivateNextPoint();
        }
        else
        {
            Debug.Log("Action Complete - Keep attcking");
        }
    }

    /*
    private bool CheckForAttack()
    {
        playerDistance = Vector3.Distance(transform.position, Convert.V2IntToV3(playerLastPosition));
        if (playerDistance < 1.1f)
        {
            //Debug.Log("Skeleton close enough to attack");
            path.Clear();
            enemyStateController.ChangeState(EnemyState.Exploding);
            
            return true;
        }
        return false;
    }*/
    
    private bool CheckForExplosion()
    {   
        if (PlayersLastPositionDistance() >= 1.1f)
            return false;
        //Debug.Log("Player close enough to explode");
        path.Clear();
        enemyStateController.ChangeState(EnemyState.Exploding);
        StartCoroutine(RotateLockOnPlayer());
        return true;
    }

    private int PlayerDistance()
    {
        Vector2Int pos = Convert.V3ToV2Int(transform.position);
        int XDist = Mathf.RoundToInt(Mathf.Abs(playerLastPosition.x - pos.x));
        int YDist = Mathf.RoundToInt(Mathf.Abs(playerLastPosition.y - pos.y));
        return Math.Max(XDist,YDist);
    }
    private bool PlayerOnSameGridCross()
    {
        Vector2Int pos = Convert.V3ToV2Int(transform.position);
        return pos.x == playerLastPosition.x || pos.y == playerLastPosition.y;
    }
    private float PlayersLastPositionDistance() => Vector3.Distance(transform.position, Convert.V2IntToV3(playerLastPosition));

    public bool TakeDamage(int amt, bool explosionDamage = false)
    {
        if(Dead) return false;

        Health -= amt;

        if (healthBar != null)
            healthBar.SetBar(Health,StartHealth);

        //Debug.Log("Enemy took damage, "+amt+" current health: "+Health);
        SoundMaster.Instance.PlaySound(SoundName.EnemyGetHit);

        if(Health > 0)
        {
            if (EnemyData.enemyType == EnemyType.Bomber)
            {
                if(enemyStateController.currentState != EnemyState.Exploding)
                {
                    //Debug.Log("Bomber took damage enough to start explode");
                    enemyStateController.ChangeState(EnemyState.Exploding);

                    return true;
                }
            }else if (EnemyData.enemyType == EnemyType.Skeleton)
            {
                if(enemyStateController.currentState != EnemyState.TakeHit)
                    enemyGetHitBeginState = enemyStateController.currentState;
                enemyStateController.ChangeState(EnemyState.TakeHit);

                return true;
            }
            else if (EnemyData.enemyType == EnemyType.Cat) {
                //Debug.Log("Enemy cat take damage");
                
                enemyStateController.ChangeState(EnemyState.TakeHit);

                SoundMaster.Instance.PlayBossTakeDamageSound();

                return true;
            }
        }
        if (EnemyData.enemyType == EnemyType.Bomber)
        {
            if (!explosionDamage)
            {
                //Debug.Log("Enemy bomber dies");
                SoundMaster.Instance.StopSound(SoundName.Hissing);
                SoundMaster.Instance.StopSound(SoundName.EnemyGetHit);
                Dead = true;

                enemyStateController.ChangeState(EnemyState.Idle);
                ItemSpawner.Instance.ReturnEnemy(this);
                    
                //Debug.Log("Enemy returned to pool");

                CreateItem(EnemyData.storedUsable);
                DisableColliders();
                return true;
            }
            else
            {
                //Debug.Log("Enemy bomb died from explosion damage");
                if (enemyStateController.currentState != EnemyState.Exploding)
                {
                    //Debug.Log("Bomber took damage enough to start explode");
                    enemyStateController.ChangeState(EnemyState.Exploding);
                    return true;
                }
            }

        }
        else if (EnemyData.enemyType == EnemyType.Skeleton)
        {
            //Debug.Log("Enemy skeleton dies");
            Dead = true;
            enemyStateController.ChangeState(EnemyState.Dying);
            DisableColliders();
            return true;
        }
        else if (EnemyData.enemyType == EnemyType.Cat)
        {
            //Debug.Log("Enemy cat dies");
            Dead = true;
            enemyStateController.ChangeState(EnemyState.Dying);
            SoundMaster.Instance.PlaySound(SoundName.BossDying);

            Debug.Log("Boss Win, Cat dies ");

            DisableColliders();
            if (IsBoss) {
                Debug.Log("Boss Win Remove Walls");
                BossWinController.Instance.WinRemoveWalls();
                UIController.Instance.ShowBossHealth(false);
            }
            return true;
        }
        return false;
    }

    public void CatHitsFloorEvent()
    {
        CameraShakeController.Instance.Shake(ShakeDataType.BossLand);
    }
    public void CatDieAndLandEvent()
    {
        CameraShakeController.Instance.Shake(ShakeDataType.BossDie);
    }
    public void DyingAnimationComplete()
    {
        //Debug.Log("Animation complete");

        enemyStateController.ChangeState(EnemyState.Idle);
        ItemSpawner.Instance.ReturnEnemy(this);
        //Debug.Log("Enemy returned to pool");
        CreateItem(EnemyData.storedUsable);
    }
    private void CreateItem(UsableData data)
    {
        ItemSpawner.Instance.SpawnUsableAt(data, transform.position);
    }
}
