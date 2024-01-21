using UnityEngine;

namespace RPG.Core
{

    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        private void LateUpdate()
        {
            transform.position = targetTransform.position;
        }
    }
}