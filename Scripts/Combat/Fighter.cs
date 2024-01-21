using RPG.Utils;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using System;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {
        public event EventHandler OnPlayerAttack;
        public event EventHandler OnPlayerStopAttack;


        [SerializeField] private float timeBetweenAttack = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfigSO defaultWeaponSO = null;
        [SerializeField] private string defaultWeaponName = "UnarmedSO";

        private Equipment equipment;
        private float fighterSpeedFraction = 1f;
        private float timeSinceLastAttack = Mathf.Infinity;
        private Health target;
        private Mover moverAgent;
        private WeaponConfigSO currentWeaponConfigSO;
        private LazyValue<Weapon> currentWeapon;

        #region Unity default
        private void Awake()
        {
            moverAgent = GetComponent<Mover>();
            currentWeaponConfigSO = defaultWeaponSO;
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += Equipment_UpdateWeapon;
            }
        }


        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.GetHealth() == 0) return;
            if (!GetIsInRange(target.transform))
            {
                moverAgent.MoveTo(target.transform.position, fighterSpeedFraction);
            }
            else
            {
                moverAgent.Cancel();
                AttackBehaviour();
            }
        }
        #endregion

        #region Private

        private Weapon SetDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponSO);
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttack)
            {
                //This will trigger the animation, which will then trigger the Hit() animation event.
                OnPlayerAttack?.Invoke(this, EventArgs.Empty);
                timeSinceLastAttack = 0f;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfigSO.GetWeaponRange();
        }
        private Weapon AttachWeapon(WeaponConfigSO weaponSO)
        {
            Animator animator = GetComponent<Animator>();
            return weaponSO.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
        }
        #endregion

        #region Public
        public void EquipWeapon(WeaponConfigSO weaponSO)
        {
            currentWeaponConfigSO = weaponSO;
            currentWeapon.value = AttachWeapon(weaponSO);
        }

        private void Equipment_UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfigSO;
            if (weapon != null)
            {
                EquipWeapon(weapon);
            }
            else
            {
                EquipWeapon(defaultWeaponSO);
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            //Checks if the target is dead, if it is we can not attack it
            if (combatTarget == null)
            {
                return false;
            }
            if (!moverAgent.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && targetToTest.GetHealth() != 0;
        }

        public Health GetTarget()
        {
            return target;
        }

        public WeaponConfigSO GetCurrentWeapon()
        {
            return currentWeaponConfigSO;
        }

        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }
        #endregion

        //Animation Events
        #region Animation Events
        private void Hit()
        {
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            if (target == null) return;
            if (currentWeaponConfigSO.HasProjectile())
            {
                currentWeaponConfigSO.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private void Shoot()
        {
            Hit();
        }
        #endregion

        #region IAction
        public void Cancel()
        {
            //Canceling the attacking
            OnPlayerStopAttack?.Invoke(this, EventArgs.Empty);
            moverAgent.Cancel();
            target = null;
        }
        #endregion

        #region IJsonSaveable
        public JToken CaptureAsJToken()
        {
            return currentWeaponConfigSO.name;
        }

        public void RestoreFromJToken(JToken state)
        {
            WeaponConfigSO weaponSO = Resources.Load<WeaponConfigSO>(state.ToString());
            EquipWeapon(weaponSO);
        }
        #endregion
    }
}