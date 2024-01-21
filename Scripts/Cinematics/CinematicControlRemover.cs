using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayerController player;
        private PlayableDirector playableDirector;
        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
            player = FindObjectOfType<PlayerController>();
        }
        private void Start()
        {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        private void EnableControl(PlayableDirector obj)
        {
            player.enabled = true;
        }

        private void DisableControl(PlayableDirector obj)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.enabled = false;
        }
    }
}