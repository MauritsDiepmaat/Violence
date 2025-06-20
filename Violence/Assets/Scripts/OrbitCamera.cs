<<<<<<< Updated upstream
=======
using System;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("References")]
    public Transform target;

    [Header("Camera Settings")]
=======
    public Transform target;
>>>>>>> Stashed changes
    public Vector2 sensitivity = new Vector2(100f, 80f);
    public Vector2 pitchLimits = new Vector2(-30f, 60f);
    public float defaultDistance = 5f;
    public float minDistance = 1f;
    public float smoothSpeed = 10f;
    public LayerMask collisionLayers;

<<<<<<< Updated upstream
    [Header("Height Settings")]
    public float shoulderHeight = 1.7f;

=======
>>>>>>> Stashed changes
    [Header("Player See-through")]
    public Renderer[] playerRenderers;
    public float transparentThreshold = 1.5f;
    [SerializeField, Range(0, 1)] private float minTransparancy = 0.2f;

<<<<<<< Updated upstream
    [Header("Lock-On Settings")]
    public GameObject focusIndicatorPrefab; // Prefab voor het symbool boven de enemy
    public float lockOnRange = 10f; // Bereik om enemies te detecteren
    public float lockOnAngle = 45f; // Maximale hoek om een enemy te targeten (FOV)
    public LayerMask enemyLayer; // Laag van de enemies
    public float lockOnSmoothSpeed = 5f; // Snelheid waarmee de camera naar de enemy draait

    private float yaw;
    private float pitch;
    private float currentDistance;
    private bool isLockedOn; // Is lock-on actief?
    private Transform lockedTarget; // Huidige lock-on target
    public Transform LockedTarget => lockedTarget; // Publieke getter
    private GameObject currentFocusIndicator; // Huidige indicator boven de target
    private Transform[] availableTargets; // Lijst met beschikbare targets
    private int currentTargetIndex = -1; // Huidige index van de locked target
=======
    private float yaw;
    private float pitch;
    private float currentDistance;
>>>>>>> Stashed changes

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDistance = defaultDistance;
<<<<<<< Updated upstream

        if (!target) Debug.LogError("[OrbitCamera] Target not assigned!");
        if (!focusIndicatorPrefab) Debug.LogWarning("[OrbitCamera] focusIndicatorPrefab niet ingesteld!");
        if (enemyLayer == 0) Debug.LogWarning("[OrbitCamera] enemyLayer niet ingesteld!");
=======
>>>>>>> Stashed changes
    }

    void Update()
    {
<<<<<<< Updated upstream
        HandleLockOnInput();
        UpdateCamera();
        HandleTransparency();
        HandleTargetSwitching();
    }

    void HandleLockOnInput()
    {
        // Toggle lock-on met de middelste muisknop
        if (Mouse.current != null && Mouse.current.middleButton.wasPressedThisFrame)
        {
            if (!isLockedOn)
            {
                // Zoek alle beschikbare targets
                UpdateAvailableTargets();
                if (availableTargets.Length > 0)
                {
                    isLockedOn = true;
                    currentTargetIndex = 0; // Start met de eerste target
                    lockedTarget = availableTargets[currentTargetIndex];
                    // Plaats een indicator boven de target
                    Vector3 indicatorPosition = lockedTarget.position + Vector3.up * 1.5f;
                    currentFocusIndicator = Instantiate(focusIndicatorPrefab, indicatorPosition, Quaternion.Euler(90f, 0f, 0f));
                    currentFocusIndicator.transform.SetParent(lockedTarget);
                    Debug.Log($"[OrbitCamera] Locked on to {lockedTarget.name} (Index: {currentTargetIndex})");
                }
            }
            else
            {
                // Schakel lock-on uit
                isLockedOn = false;
                lockedTarget = null;
                currentTargetIndex = -1;
                if (currentFocusIndicator != null)
                {
                    Destroy(currentFocusIndicator);
                    currentFocusIndicator = null;
                }
                Debug.Log("[OrbitCamera] Lock-on disabled");
            }
        }
    }

    void HandleTargetSwitching()
    {
        if (isLockedOn && Mouse.current != null && Mouse.current.scroll.ReadValue().y != 0f)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            UpdateAvailableTargets(); // Vernieuw de lijst met targets

            if (availableTargets.Length > 0)
            {
                // Verwijder de oude indicator
                if (currentFocusIndicator != null)
                {
                    Destroy(currentFocusIndicator);
                    currentFocusIndicator = null;
                }

                // Pas de index aan gebaseerd op scroll-richting
                if (scrollValue > 0f) // Scroll omhoog: ga naar de vorige target
                {
                    currentTargetIndex = (currentTargetIndex - 1 + availableTargets.Length) % availableTargets.Length;
                }
                else if (scrollValue < 0f) // Scroll omlaag: ga naar de volgende target
                {
                    currentTargetIndex = (currentTargetIndex + 1) % availableTargets.Length;
                }

                lockedTarget = availableTargets[currentTargetIndex];
                // Plaats een nieuwe indicator boven de nieuwe target
                Vector3 indicatorPosition = lockedTarget.position + Vector3.up * 1.5f;
                currentFocusIndicator = Instantiate(focusIndicatorPrefab, indicatorPosition, Quaternion.Euler(90f, 0f, 0f));
                currentFocusIndicator.transform.SetParent(lockedTarget);
                Debug.Log($"[OrbitCamera] Switched to {lockedTarget.name} (Index: {currentTargetIndex})");
            }
        }
    }

    void UpdateAvailableTargets()
    {
        // Zoek alle enemies binnen bereik en update de lijst
        Collider[] enemies = Physics.OverlapSphere(target.position, lockOnRange, enemyLayer);
        System.Collections.Generic.List<Transform> tempTargets = new System.Collections.Generic.List<Transform>();

        foreach (Collider enemyCollider in enemies)
        {
            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null && enemy.IsAlerted())
            {
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

                if (angleToEnemy <= lockOnAngle)
                {
                    tempTargets.Add(enemy.transform);
                }
            }
        }

        availableTargets = tempTargets.ToArray();
        // Als de huidige target niet meer beschikbaar is, reset de index
        if (isLockedOn && lockedTarget != null && !System.Array.Exists(availableTargets, t => t == lockedTarget))
        {
            currentTargetIndex = -1;
            isLockedOn = false;
            if (currentFocusIndicator != null)
            {
                Destroy(currentFocusIndicator);
                currentFocusIndicator = null;
            }
            Debug.Log("[OrbitCamera] Current target out of range, lock-on disabled");
        }
    }

    void UpdateCamera()
    {
        // Verwerk muisinput voor yaw en pitch
=======
>>>>>>> Stashed changes
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * sensitivity.x * Time.deltaTime;
            pitch -= mouseDelta.y * sensitivity.y * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        }

<<<<<<< Updated upstream
        // Bepaal de rotatie en gewenste positie
        Quaternion rotation;
        Vector3 desiredDirection;
        float targetDistance = defaultDistance;

        if (isLockedOn && lockedTarget != null)
        {
            // Roteer de camera om naar de locked target te kijken
            Vector3 shoulderTarget = target.position + Vector3.up * shoulderHeight;
            Vector3 directionToTarget = (lockedTarget.position - shoulderTarget).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Soepel overgangen naar de nieuwe rotatie
            rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnSmoothSpeed * Time.deltaTime);
            desiredDirection = rotation * Vector3.back;

            // Overschrijf yaw en pitch om de camera consistent te houden
            Vector3 euler = rotation.eulerAngles;
            yaw = euler.y;
            pitch = euler.x;
            if (pitch > 180f) pitch -= 360f; // Zorg dat pitch binnen -180 tot 180 blijft
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        }
        else
        {
            // Normale orbit camera logica
            rotation = Quaternion.Euler(pitch, yaw, 0f);
            desiredDirection = rotation * Vector3.back;
        }

        // Adjust target position to shoulder height
        Vector3 shoulderTargetFinal = target.position + Vector3.up * shoulderHeight;

        // Raycast om clipping te voorkomen
        Ray ray = new Ray(shoulderTargetFinal, desiredDirection);
=======
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredDirection = rotation * Vector3.back;
        float targetDistance = defaultDistance;

        // Raycast to avoid clipping
        Ray ray = new Ray(target.position, desiredDirection);
>>>>>>> Stashed changes
        if (Physics.Raycast(ray, out RaycastHit hit, defaultDistance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, defaultDistance);
        }

        // Smooth camera distance
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);
<<<<<<< Updated upstream
        transform.position = shoulderTargetFinal + desiredDirection * currentDistance;
        transform.rotation = rotation;
=======
        transform.position = target.position + desiredDirection * currentDistance;
        transform.rotation = rotation;

        HandleTransparency();
>>>>>>> Stashed changes
    }

    void HandleTransparency()
    {
        if (playerRenderers == null) return;

<<<<<<< Updated upstream
        if (currentDistance <= 2 * transparentThreshold)
        {
            float transparancy = Mathf.Max(minTransparancy + (currentDistance - transparentThreshold) / transparentThreshold * (1 - minTransparancy), minTransparancy);
=======
        if (currentDistance <=  2*transparentThreshold)
        {
            //playerRenderer.material = transparentMaterial;
            float transparancy = MathF.Max(minTransparancy + (currentDistance - transparentThreshold) / transparentThreshold * (1-minTransparancy), minTransparancy);
>>>>>>> Stashed changes

            for (int i = 0; i < playerRenderers.Length; i++)
            {
                foreach (Material mat in playerRenderers[i].materials) mat.SetFloat("_Alpha", transparancy);
            }
<<<<<<< Updated upstream
=======

            
>>>>>>> Stashed changes
        }
        else
        {
            for (int i = 0; i < playerRenderers.Length; i++)
            {
                foreach (Material mat in playerRenderers[i].materials) mat.SetFloat("_Alpha", 1);
            }
        }
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
