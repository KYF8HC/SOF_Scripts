using RPG.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float aggrevationTime = 7f;
        [SerializeField] private float waypointStopTime = 5f;
        [SerializeField] private float waypointTolerance = .5f;
        [Range(0f, 1f)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float shoutDistance = 5f;

        private GameObject player;
        private Fighter aiFighter;
        private Health health;
        private Mover aiMover;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceLastStopped = Mathf.Infinity;
        private float timeSinceLastAggrevated = Mathf.Infinity;
        private int patrolIndex = 0;

        #region Unity default
        private void Awake()
        {
            player = FindObjectOfType<PlayerTag>().gameObject;
            aiFighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            aiMover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }
        private void Update()
        {
            if (health.GetHealth() == 0)
            {
                return;
            }
            if (player == null)
            {
                return;
            }
            if (IsAggrevated() && aiFighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }
        #endregion

        #region Private

        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timeSinceLastStopped += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceLastStopped = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceLastStopped < waypointStopTime) return;
            aiMover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(patrolIndex);
        }

        private void CycleWaypoint()
        {
            patrolIndex = patrolPath.GetNextIndex(patrolIndex);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            aiFighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController enemyNearby = hit.transform.GetComponent<AIController>();
                if (enemyNearby != null && enemyNearby != this)
                {
                    enemyNearby.Aggrevate();
                }
            }
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            //Checking if the player is in the attack range
            return distanceToPlayer < chaseDistance || timeSinceLastAggrevated < aggrevationTime;
        }
        #endregion

        #region Public

        public void Aggrevate()
        {
            timeSinceLastAggrevated = 0;
        }

        public float GetChaseDistance()
        {
            return chaseDistance;
        }
        #endregion
    }
}