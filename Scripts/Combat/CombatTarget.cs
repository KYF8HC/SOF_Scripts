using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using static RPG.Control.PlayerController;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            Fighter playerFighter = callingController.GetComponent<Fighter>();
            if (!playerFighter.CanAttack(gameObject))
            {
                return false;
            }
            if (Input.GetMouseButton(0))
            {
                //If the target is dead the character
                //just going to move to the place we clicked.
                //If it is not dead we are going to attack it.
                playerFighter.Attack(gameObject);
            }
            return true;
        }
    }
}