using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, IJsonSaveable
    {
        private bool isTriggeredAlready = false;


        private void OnTriggerEnter(Collider other)
        {
            if (isTriggeredAlready) return;
            if (other.GetComponent<PlayerController>() != null)
            {
                isTriggeredAlready = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(isTriggeredAlready);
        }

        public void RestoreFromJToken(JToken state)
        {
            isTriggeredAlready = state.ToObject<bool>();
        }
    }
}