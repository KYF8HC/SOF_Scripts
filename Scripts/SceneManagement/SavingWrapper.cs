using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string defaultSaveFile = "Saves";

        [SerializeField] private float fadeInTime = .5f;
        [SerializeField] private float waitBeforeFade = .5f;

        JsonSavingSystem jsonSavingSystem;

        private void Awake()
        {
            jsonSavingSystem = GetComponent<JsonSavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return jsonSavingSystem.LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return new WaitForSeconds(waitBeforeFade);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            jsonSavingSystem.Load(defaultSaveFile);
        }

        public void Save()
        {
            jsonSavingSystem.Save(defaultSaveFile);
        }
        public void Delete()
        {
            jsonSavingSystem.Delete(defaultSaveFile);
        }
    }
}