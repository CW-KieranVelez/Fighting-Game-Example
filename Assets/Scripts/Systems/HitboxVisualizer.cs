using UnityEngine;

namespace FightTest.Systems
{
    /// <summary>
    /// Spawns a wire-square overlay at _origin for the duration of active frames.
    /// Assign a solid-color sprite (or leave null to use a procedural quad) in the Inspector.
    /// Call Show(size) on active-frame start, Hide() on active-frame end / state Exit.
    /// </summary>
    public sealed class HitboxVisualizer : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private Color _color = new Color(1f, 0f, 0f, 0.35f);

        private GameObject _visual;

        private void Awake()
        {
            _visual = CreateQuad();
            _visual.SetActive(false);
        }

        public void Show(Vector2 size)
        {
            _visual.transform.position = _origin.position;
            _visual.transform.localScale = new Vector3(size.x, size.y, 1f);
            _visual.SetActive(true);
        }

        public void Hide()
        {
            _visual.SetActive(false);
        }

        // Follows origin each frame while visible (e.g. for moving hitboxes)
        private void LateUpdate()
        {
            if (_visual.activeSelf)
                _visual.transform.position = _origin.position;
        }

        private GameObject CreateQuad()
        {
            var go = new GameObject("HitboxVisual");
            go.transform.SetParent(null); // world space so scale isn't inherited

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = BuildWhiteSquareSprite();
            sr.color = _color;
            sr.sortingOrder = 100;

            return go;
        }

        private static Sprite BuildWhiteSquareSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private void OnDestroy()
        {
            if (_visual != null)
                Destroy(_visual);
        }
    }
}
