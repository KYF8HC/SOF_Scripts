using RPG.Control;
using RPG.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            PortalPairOne,
            PortalPairTwo
        }
        [SerializeField] private int sceneIDToLoad = 1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutWaitingTime = 1f;
        [SerializeField] private float fadeInWaitingTime = 2f;
        [SerializeField] private float fadeWaitingTime = .5f;

        private Fader fader;
        private SavingWrapper savingWrapper;

        private void Awake()
        {
            fader = FindObjectOfType<Fader>();
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerTag>() != null)
            {
                StartCoroutine(Transition());
            }
        }
        private IEnumerator Transition()
        {
            if (sceneIDToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            yield return fader.FadeOut(fadeOutWaitingTime);
            PlayerController player = FindObjectOfType<PlayerController>();
            player.enabled = false;

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneIDToLoad);
            PlayerController newPlayer = FindObjectOfType<PlayerController>();
            newPlayer.enabled = false;

            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitingTime);
            fader.FadeIn(fadeInWaitingTime);
            
            newPlayer.enabled = true;
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            foreach (Portal portal in portals)
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }
    }
}