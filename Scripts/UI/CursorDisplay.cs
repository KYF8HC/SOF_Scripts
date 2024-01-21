using System;
using UnityEngine;

namespace RPG.Control
{
    public class CursorDisplay : MonoBehaviour
    {

        [Serializable]
        private struct CursorMapping
        {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings = null;

        private PlayerController playerController;

        private void Awake()
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        private void OnEnable()
        {
            playerController.OnCursorHit += PlayerController_OnCursorHit;
        }

        private void PlayerController_OnCursorHit(object sender, PlayerController.OnCursorHitEventArgs e)
        {
            SetCursor(e.cursorType);
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if(mapping.cursorType == cursorType)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
    }
}
