using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnProjectileHit;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] GameObject hitEffect;
        [SerializeField] GameObject[] destroyOnHit;

        private float lifeAfterImpact = .2f;
        private float destroyTimerMax = 3f;
        private Health target;
        private float damage = 0;
        GameObject instigator = null;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            Destroy(gameObject, destroyTimerMax);
            if (target == null) return;
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        #region Private
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);

            moveSpeed = 0;

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            OnProjectileHit.Invoke();
            foreach (GameObject toDestroy in destroyOnHit)
            {

                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
        #endregion

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }
    }
}