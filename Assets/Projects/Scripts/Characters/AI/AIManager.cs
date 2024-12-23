using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class AIManager : CharacterManager
    {
        //In-Built Components
        public NavMeshPath navMeshPath;
        public NavMeshAgent navMeshAgent {get; private set;}

        //Creotly Components
        public AILogicHandler aiLogicHandler {get; private set;}
        public AIStatsManager aIStatsManager {get; private set;}
        public AICombatManager aICombatManager {get; private set;}
        public AIAnimationManager aIAnimationManager {get; private set;}
        public AIInventoryManager aIInventoryManager {get; private set;}
        public AILocomotionManager aILocomotionManager {get; private set;}
        public EnemyDetectionScript enemyDetectionScript {get; private set;}
        

        //Private Parameters
        public float AngleOfTarget {get; private set;}
        public float DistanceToTarget {get; private set;}
        public Vector3 DirectionToTarget {get; private set;}

        [Header("States")]
        public AIState currentState;
        public PatrolState patrolState;
        public CombatState combatState;
        public PursueState pursueState;
        public AttackState attackState;

        [Header("Target Properties")]
        public Target target;
        public BoundaryFloat angleLimit;
        public List<Target> possibleVisualTargets = new List<Target>();

        [Header("Enemy Status")]
        public float coolDownTimer;
        public EnemyType enemyType = EnemyType.Mech;

        protected override void Awake()
        {
            animator = GetComponent<Animator>();
            
            base.Awake();

            patrolState = Instantiate(patrolState);
            pursueState = Instantiate(pursueState);
            combatState = Instantiate(combatState);

            healthBarUI = GetComponentInChildren<UIBar>();
            aiLogicHandler = GetComponent<AILogicHandler>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyDetectionScript = GetComponent<EnemyDetectionScript>();

            aIStatsManager = characterStatsManager as AIStatsManager;
            aICombatManager = characterCombatManager as AICombatManager;
            aIAnimationManager = characterAnimationManager as AIAnimationManager;
            aIInventoryManager = characterInventoryManager as AIInventoryManager;
            aILocomotionManager = characterLocomotionManager as AILocomotionManager;

            navMeshAgent.enabled = false;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            navMeshAgent.enabled = false;
            currentState = patrolState.SwitchState(patrolState, this);
        }

        // Update is called once per frame
        protected override void Update()
        {
            if(isDead)
            {
                return;
            }
            float delta = Time.deltaTime;

            aiLogicHandler.AILogicHandler_Updater();
            SetCurrentTargetDetails();
            
            HandleStateChange(delta);
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            aICombatManager.AICombatManager_LateUpdate();
        }

        private void ReduceCoolDownTimer(float delta)
        {
            if(currentState == attackState)
            {
                return;
            }

            if(coolDownTimer <= 0.0f)
            {
                coolDownTimer = 0.0f;
                return;
            }
            coolDownTimer -= delta;
        }

        private void SetCurrentTargetDetails()
        {
            if(target.visualTarget == null && target.audioTarget == null)
            {
                return;
            }
            target.CalculateParameters(transform);
            AngleOfTarget = target.targetDetectAngle;
            DistanceToTarget = target.targetDistance;
            DirectionToTarget = target.targetDirection;
        }

        public void SetPersonalTargetDetails(Vector3 targetPosition)
        {
            if(target.visualTarget != null || target.audioTarget != null)
            {
                return;
            }
            DirectionToTarget = (transform.position - targetPosition).normalized;
            DistanceToTarget = DirectionToTarget.magnitude;
            AngleOfTarget = Maths_PhysicsHelper.CalculateViewAngle(transform.forward, DirectionToTarget);
        }

        private void HandleStateChange(float delta)
        {
            if (currentState != null)
            {
                var nextState = currentState.AISate_Updater(this);

                if (nextState != null)
                {
                    currentState = nextState;
                }
            }
            navMeshAgent.transform.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            CheckIfMoving();
            ReduceCoolDownTimer(delta);
        }

        private void CheckIfMoving()
        {
            if(currentState == combatState)
            {
                return;
            }
            
            if(navMeshAgent.enabled == false)
            {
                isMoving = false;
                return;
            }
            isMoving = SetMoving();
        }

        private bool SetMoving()
        {
            if(DistanceToTarget > navMeshAgent.stoppingDistance)
            {
                return true;
            }

            if(currentState == patrolState)
            {
                if(patrolState.patrolMode == PatrolMode.Walk)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
