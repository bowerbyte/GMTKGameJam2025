using UnityEngine;

namespace Project.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform orbitCenter;

        [Header("Zoom Settings")]
        [SerializeField] private float minZoomDistance = 5f;
        [SerializeField] private float maxZoomDistance = 30f;
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float zoomInertia = 5f; // Higher = smoother

        [Header("Orbit Settings")]
        [SerializeField] private float orbitSpeedMouse = 180f; // Degrees/sec
        [SerializeField] private float orbitSpeedKeyboard = 90f;
        [SerializeField] private float orbitInertia = 5f;

        [Header("Elevation Settings")]
        [SerializeField] private float elevationSpeed = 60f; // Degrees/sec
        [SerializeField] private float elevationInertia = 5f;
        [SerializeField] private float minElevationAngle = 10f;
        [SerializeField] private float maxElevationAngle = 80f;

        [Header("Input")]
        [SerializeField] private KeyCode orbitKey = KeyCode.Mouse0; // Left mouse button

        private float currentZoom;
        private float targetZoom;

        private float currentAzimuthAngle;
        private float targetAzimuthAngle;

        private float currentElevationAngle;
        private float targetElevationAngle;

        private Vector2 previousMousePosition;

        void Start()
        {
            if (orbitCenter == null)
            {
                Debug.LogError("CameraController requires an orbitCenter transform.");
                enabled = false;
                return;
            }

            currentZoom = Vector3.Distance(transform.position, orbitCenter.position);
            targetZoom = currentZoom;

            Vector3 toCamera = transform.position - orbitCenter.position;
            currentAzimuthAngle = Mathf.Atan2(toCamera.x, toCamera.z) * Mathf.Rad2Deg;
            targetAzimuthAngle = currentAzimuthAngle;

            float flatDist = new Vector2(toCamera.x, toCamera.z).magnitude;
            currentElevationAngle = Mathf.Atan2(toCamera.y, flatDist) * Mathf.Rad2Deg;
            currentElevationAngle = Mathf.Clamp(currentElevationAngle, minElevationAngle, maxElevationAngle);
            targetElevationAngle = currentElevationAngle;
        }

        void Update()
        {
            HandleZoomInput();
            HandleOrbitInput();

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomInertia);
            currentAzimuthAngle = Mathf.LerpAngle(currentAzimuthAngle, targetAzimuthAngle, Time.deltaTime * orbitInertia);
            currentElevationAngle = Mathf.Lerp(currentElevationAngle, targetElevationAngle, Time.deltaTime * elevationInertia);

            UpdateCameraPosition();
        }

        private void HandleZoomInput()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                float zoomFactor = Mathf.Clamp01((currentZoom - minZoomDistance) / (maxZoomDistance - minZoomDistance));
                float scaledZoomSpeed = zoomSpeed * Mathf.Lerp(0.2f, 1f, zoomFactor);
                targetZoom -= scrollInput * scaledZoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoomDistance, maxZoomDistance);
            }
        }

        private void HandleOrbitInput()
        {
            if (Input.GetKey(orbitKey))
            {
                Vector2 delta = (Vector2)Input.mousePosition - previousMousePosition;
                targetAzimuthAngle += delta.x * orbitSpeedMouse * Time.deltaTime * 0.1f;
                targetElevationAngle -= delta.y * elevationSpeed * Time.deltaTime * 0.1f;
            }
            else
            {
                float horizontal = -Input.GetAxis("Horizontal");
                targetAzimuthAngle += horizontal * orbitSpeedKeyboard * Time.deltaTime;

                float vertical = Input.GetAxis("Vertical");
                targetElevationAngle += vertical * elevationSpeed * Time.deltaTime;
            }

            targetElevationAngle = Mathf.Clamp(targetElevationAngle, minElevationAngle, maxElevationAngle);
            previousMousePosition = Input.mousePosition;
        }

        private void UpdateCameraPosition()
        {
            float azimuthRad = currentAzimuthAngle * Mathf.Deg2Rad;
            float elevationRad = currentElevationAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                currentZoom * Mathf.Cos(elevationRad) * Mathf.Sin(azimuthRad),
                currentZoom * Mathf.Sin(elevationRad),
                currentZoom * Mathf.Cos(elevationRad) * Mathf.Cos(azimuthRad)
            );

            transform.position = orbitCenter.position + offset;
            transform.LookAt(orbitCenter.position, Vector3.up);
        }
    }
}