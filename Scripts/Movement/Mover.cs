using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] float moveSpeedMax = 6f;
        [SerializeField] private float maxNavPathLength = 40f;

        private bool isWalking;
        private NavMeshAgent moverAgent;
        private Health health;
        private void Awake()
        {
            moverAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        private void Update()
        {
            if(health.GetHealth() == 0)
            {
                moverAgent.enabled = false;
            }
        }
        private float GetPathLength(NavMeshPath path)
        {
            float totalLength = 0f;
            if (path.corners.Length < 2)
            {
                return totalLength;
            }
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return totalLength;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            if (!hasPath)
            {
                return false;
            }

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (GetPathLength(path) > maxNavPathLength)
            {
                return false;
            }
            return true;
        }


        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //To separate the simple movement action from the actual moving
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            //Actual moving
            moverAgent.isStopped = false;
            moverAgent.speed = moveSpeedMax * Mathf.Clamp01(speedFraction);
            moverAgent.destination = destination;
        }

        public void Cancel()
        {
            //Stoping the movement
            moverAgent.isStopped = true;
        }

        public NavMeshAgent GetMoverAgent()
        {
            return moverAgent;
        }

        #region IJsonSaveable
        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            moverAgent.Warp(state.ToVector3());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        #endregion
    }
}