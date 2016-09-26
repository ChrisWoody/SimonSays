using UnityEngine;

namespace Assets.Scripts
{
    public class Tile : MonoBehaviour
    {
        private const float HightlightTimeout = 0.2f;
        private float _hightlightedElapsed;
        private bool _highlighted;

        private Color _origColour;

        private bool _allowToBeClicked;
        public bool IsClicked { get; private set; }

        public int Index { get; set; }

        void Start()
        {
            _origColour = GetComponent<SpriteRenderer>().color;
        }

        void Update()
        {
            if (_highlighted)
            {
                _hightlightedElapsed += Time.deltaTime;
                if (_hightlightedElapsed >= HightlightTimeout)
                {
                    _hightlightedElapsed = 0f;
                    _highlighted = false;
                    GetComponent<SpriteRenderer>().color = _origColour;
                }
            }
        }

        public void Highlight()
        {
            _highlighted = true;
            _hightlightedElapsed = 0f;

            GetComponent<SpriteRenderer>().color = Color.white;
        }

        void OnMouseDown()
        {
            if (_allowToBeClicked)
            {
                IsClicked = true;
                Highlight();
            }
        }

        public void ClearIsClicked()
        {
            IsClicked = false;
        }

        public void SetAllowToBeClicked(bool allow)
        {
            _allowToBeClicked = allow;
        }
    }
}