using UnityEngine;

namespace Environment.Parallax
{
    /// <summary>
    /// Jednostavna i optimizirana 2D Parallax skripta za pozadine (side-scroller platformer).
    /// Postavlja se direktno na GameObject pozadine ili pojedinog sloja (layer-a).
    /// </summary>
    [DefaultExecutionOrder(150)]
    [AddComponentMenu("Environment/Parallax Background")]
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("Referenca na kameru. Ako se ostavi prazno, skripta automatski koristi Camera.main.")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Parallax Effect")]
        [Tooltip("0 = kreće se jednako kao svijet (stoji u mjestu).\n" +
                 "1 = kreće se potpuno zajedno s kamerom (privid beskonačne udaljenosti, npr. sunce/nebo).\n" +
                 "0.5 = kreće se upola sporije od kamere (srednja udaljenost, npr. brda/planine).")]
        [SerializeField] private Vector2 _parallaxFactor = new Vector2(0.5f, 0.05f);

        [Header("Vertical Movement")]
        [Tooltip("Uključi ako želiš potpuno zaključati vertikalni pomak (pozadina se neće pomicati gore/dolje kad igrač skoči).")]
        [SerializeField] private bool _lockVerticalPosition = false;

        [Header("Infinite Scrolling (Ponavljanje)")]
        [Tooltip("Uključi ako želiš da se pozadina beskonačno ponavlja/reponira po X osi dok se kamera kreće.")]
        [SerializeField] private bool _infiniteHorizontal = true;

        [Tooltip("Širina jednog segmenta u Unity jedinicama. Ako je 0, automatski se preuzima iz SpriteRenderer komponente.")]
        [SerializeField] private float _textureUnitSizeX;

        private Vector3 _startPosition;
        private Vector3 _cameraStartPosition;
        private float _offsetLoopX;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_cameraTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    _cameraTransform = mainCam.transform;
                }
                else
                {
                    Debug.LogWarning($"[ParallaxBackground] Camera.main nije pronađena za '{gameObject.name}'. Ručno dodijeli kameru u inspectoru.");
                    return;
                }
            }

            _startPosition = transform.position;
            _cameraStartPosition = _cameraTransform.position;
            _offsetLoopX = 0f;

            // Automatski dohvat širine iz SpriteRenderer-a ako nije ručno postavljena
            if (_infiniteHorizontal && _textureUnitSizeX <= 0f)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    _textureUnitSizeX = spriteRenderer.bounds.size.x;
                }
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            // Ukupan pomak kamere od početne pozicije
            Vector3 cameraTravel = _cameraTransform.position - _cameraStartPosition;

            // Izračun nove pozicije sloja
            float distanceX = cameraTravel.x * _parallaxFactor.x;
            float distanceY = _lockVerticalPosition ? 0f : (cameraTravel.y * _parallaxFactor.y);

            transform.position = new Vector3(_startPosition.x + distanceX, _startPosition.y + distanceY, _startPosition.z);

            // Beskonačno ponavljanje (horizontalno)
            if (_infiniteHorizontal && _textureUnitSizeX > 0.001f)
            {
                // Koliko se kamera prividno pomaknula u odnosu na pozadinski sloj
                float relativeTravelX = cameraTravel.x * (1f - _parallaxFactor.x);

                while (relativeTravelX - _offsetLoopX >= _textureUnitSizeX)
                {
                    _offsetLoopX += _textureUnitSizeX;
                    _startPosition.x += _textureUnitSizeX;
                }

                while (relativeTravelX - _offsetLoopX <= -_textureUnitSizeX)
                {
                    _offsetLoopX -= _textureUnitSizeX;
                    _startPosition.x -= _textureUnitSizeX;
                }
            }
        }
    }
}
