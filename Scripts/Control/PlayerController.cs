using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        public event EventHandler<OnCursorHitEventArgs> OnCursorHit;

        public class OnCursorHitEventArgs : EventArgs
        {
            public CursorType cursorType;
        }

        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;

        private Mover playerMover;
        private Fighter playerFighter;
        private Health health;
        private float playerSpeedFraction = 1f;
        private bool isDraggingUIElement = false;

        private void Awake()
        {
            playerMover = GetComponent<Mover>();
            playerFighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (InteractWithUI())
            {
                return;
            }

            if (health.GetHealth() == 0)
            {
                OnCursorHit?.Invoke(this, new OnCursorHitEventArgs
                {
                    cursorType = CursorType.None
                });
                return;
            }

            if (InteractWithComponent())
            {
                return;
            }

            if (InteractWithMovement())
            {
                return;
            }

            OnCursorHit?.Invoke(this, new OnCursorHitEventArgs
            {
                cursorType = CursorType.None
            });
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSortedByDepthInScene();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        OnCursorHit?.Invoke(this, new OnCursorHitEventArgs
                        {
                            cursorType = raycastable.GetCursorType(),
                        });
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSortedByDepthInScene()
        {

            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            //It does not matter if we are over the UI or not, since we can release item over the terrain to drop them
            if(Input.GetMouseButtonUp(0))
            {
                isDraggingUIElement = false;
            }
            //Checking if the Cursor is over a UI! GameObject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //Checking if we are dragging an UI Element
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUIElement = true;
                }
                OnCursorHit?.Invoke(this, new OnCursorHitEventArgs
                {
                    cursorType = CursorType.UI
                });
                return true;
            }
            if (isDraggingUIElement)
            {
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!playerMover.CanMoveTo(target))
                {
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                    playerMover.StartMoveAction(target, playerSpeedFraction);
                }
                OnCursorHit?.Invoke(this, new OnCursorHitEventArgs
                {
                    cursorType = CursorType.Movement
                });
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit)
            {
                return false;
            }

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh)
            {
                return false;
            }

            target = navMeshHit.position;

            return true;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}