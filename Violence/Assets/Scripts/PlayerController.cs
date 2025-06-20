using UnityEngine;
<<<<<<< Updated upstream
using UnityEngine.VFX; // Voor VisualEffect
using System.Collections; // Zorgt voor IEnumerator ondersteuning

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public struct AttackData
    {
        public string attackParam; // Naam van de animatieparameter (bijv. "Attack1")
        public float damage; // Schade van de aanval
        public float duration; // Duur van de aanval in seconden
        public float hitWindowStart; // Start van het hit-frame (0.0 tot 1.0 van de duur)
        public float hitWindowEnd; // Einde van het hit-frame (0.0 tot 1.0 van de duur)
    }

=======
using System.Collections;

public class PlayerController : MonoBehaviour
{
>>>>>>> Stashed changes
    [Header("Movement")]
    public float moveSpeed = 5f;
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("Gravity & Jumping")]
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
<<<<<<< Updated upstream
    public Transform groundCheck; // Gecorrigeerd van Transport naar Transform
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
=======
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private Vector3 externalVelocity = Vector3.zero;
    private bool isGrounded;
>>>>>>> Stashed changes

    [Header("Dodge Roll")]
    public float rollDistance = 5f;
    public float rollDuration = 0.4f;
    public float rollCooldown = 1f;
<<<<<<< Updated upstream

    [Header("Crouch & Slide")]
    public float crouchHeight = 1f;
    public float slideInitialMultiplier = 2f;
    public float slideMinimumSpeed = 1f;
    public float slideFriction = 5f;

    [Header("Slide Physics")]
    public float slideStopCheckDistance = 0.5f;
    public LayerMask obstacleMask;

    [Header("Bullet Jump")]
    public float bulletJumpSpeedMultiplier = 20f;
    public float bulletJumpMinAngle = 15f;
    public float bulletJumpMaxAngle = 75f;
    public float bulletJumpUpwardForce = 20f;

    [Header("Aim Glide & Wall Latching")]
    public float glideGravityScale = 0.3f;
    public float glideLerpTime = 0.5f; // Tijd om opwaartse snelheid te verminderen (in seconden)
    public float wallCheckDistance = 0.1f;
    public float maxAirTime = 5f;
    public float wallJumpUpwardForce = 100f;
    public float wallJumpHorizontalForce = 20f;
    public float wallJumpGlideCooldown = 0.2f;
    public float aimGlideDelayAfterWall = 0.1f;
    public float wallJumpAimGlideCooldown = 1.0f;
    public float wallJumpGravityDelay = 1.0f;
    public float wallJumpCooldown = 0.2f;

    [Header("Smoothing")]
    public float directionSmoothTime = 0.1f;
    public float rotationSpeed = 10f; // Aanpasbare snelheid voor rotatie tijdens aanval

    [Header("References")]
    public Transform characterModel;
    public Animator animator;
    public OrbitCamera orbitCamera; // Referentie naar de camera voor lock-on info

    [Header("Melee Attack")]
    public AttackData[] attackData; // Array met attack-configuraties (voor grondaanvallen)
    public AttackData[] midAirAttacks; // Array met mid-air attack-configuraties
    public AttackData plungingAttack; // Speciale plunging attack voor mid-air
    public AttackData heavyAttack; // Heavy melee attack configuratie
    public float comboTimeWindow = 0.5f; // Tijd om de volgende aanval in te drukken
    public KeyCode attackKey = KeyCode.Mouse0; // Aanvalstoets (linker muisknop)
    public float plungingAttackAngleThreshold = 45f; // Maximale camera-hoek voor plunging attack (in graden)
    public float heavyAttackHoldTime = 0.5f; // Tijd dat de knop ingedrukt moet worden voor een heavy attack
    public float heavyAttackCooldown = 0.3f; // Cooldown na een heavy attack om normale aanval te voorkomen

    [Header("Hurtbox Settings")]
    public Transform meleeHurtbox; // Referentie naar de hurtbox Transform (bijv. een lege GameObject)
    [Range(0.5f, 3.0f)] public Vector3 hurtboxSize = new Vector3(1.0f, 1.0f, 1.0f); // Aanpasbare grootte van de hurtbox
    public LayerMask enemyLayer; // Laag voor vijanden

    [Header("Visual Effects")]
    public VisualEffect swordSlamVFX; // Referentie naar de SwordSlam VFX Graph
    public Transform swordTip; // Referentie naar het punt van het zwaard om de impactpositie te bepalen
    public float maxGroundCheckDistance = 2f; // Maximale afstand voor raycast naar de grond
    public float swordSlamVFXDelay = 0.5f; // Delay als fractie van de aanvalsduur (0.0 tot 1.0)

    [Header("Debug Toggles")]
    public bool debugMelee = true;
    public bool debugMovement = true;
    public bool debugWallLatch = true;
    public bool debugJump = true;
    public bool debugVFX = true;

    // Private state
    private Vector3 velocity;
    private Vector3 externalVelocity;
    private Vector3 moveDirection;
    private Vector3 smoothDirVelocity;
    private Vector2 input;
    private float lastRollTime;
    private bool isGrounded;
    private bool isRolling;
    private bool isSliding;
    private bool isCrouching;
    private float originalHeight;
    private Vector3 originalCenter;
    private bool blockBulletJump;
    private bool blockCrouchSlide;
    private bool isBulletJumping;
    private bool isWallLatching;
    private bool isAimGliding;
    private Vector3 wallNormal;
    private float airTime;
    private float lastWallJumpTime;
    private float lastWallLatchExitTime;
    private bool canWallJump;
    private float lastDebugTime;
    private bool isWallJumping;
    private float lastAttackTime; // Tijd van de laatste aanval
    private int comboIndex = 0; // Huidige combo-stap (0-based index)
    private bool isAttacking = false; // Is er momenteel een aanval bezig?
    private bool isPlunging = false; // Is er momenteel een plunging attack bezig?
    private float glideStartTime; // Tijdstip waarop aimGlide begon
    private float initialGlideVelocityY; // Initiële opwaartse snelheid bij starten van aimGlide
    private float attackHoldTime = 0f; // Tijd dat de aanvalsknop is ingedrukt
    private bool isHoldingAttack = false; // Of de speler de aanvalsknop ingedrukt houdt
    private bool heavyAttackTriggered = false; // Nieuwe vlag om te controleren of de heavy attack al is getriggerd
    private float lastHeavyAttackTime = -100f; // Tijdstip van de laatste heavy attack (toegevoegd)

    private void Awake()
    {
        originalHeight = controller ? controller.height : 2f;
        originalCenter = controller ? controller.center : Vector3.zero;
        if (!animator)
            animator = GetComponentInChildren<Animator>();
        if (!controller) Debug.LogError("[PlayerController] CharacterController niet ingesteld!");
        if (!cameraTransform) Debug.LogError("[PlayerController] cameraTransform niet ingesteld!");
        if (!groundCheck) Debug.LogError("[PlayerController] groundCheck niet ingesteld!");
        if (!characterModel) Debug.LogError("[PlayerController] characterModel niet ingesteld!");
        if (!animator) Debug.LogError("[PlayerController] Animator niet ingesteld!");
        if (!orbitCamera) Debug.LogWarning("[PlayerController] orbitCamera niet ingesteld!");
        if (attackData == null || attackData.Length == 0)
            Debug.LogWarning("[PlayerController] Geen attackData ingesteld! Voeg attacks toe in de Inspector.");
        if (midAirAttacks == null || midAirAttacks.Length == 0)
            Debug.LogWarning("[PlayerController] Geen midAirAttacks ingesteld! Voeg mid-air attacks toe in de Inspector.");
        if (string.IsNullOrEmpty(plungingAttack.attackParam))
            Debug.LogWarning("[PlayerController] Plunging attack niet ingesteld! Configureer in de Inspector.");
        if (string.IsNullOrEmpty(heavyAttack.attackParam))
            Debug.LogWarning("[PlayerController] Heavy attack niet ingesteld! Configureer in de Inspector.");
        if (!swordSlamVFX) Debug.LogWarning("[PlayerController] SwordSlam VFX niet ingesteld! Voeg de VisualEffect toe in de Inspector.");
        if (!swordTip) Debug.LogWarning("[PlayerController] SwordTip niet ingesteld! Stel een Transform in voor het zwaardpunt in de Inspector.");
        if (!meleeHurtbox) Debug.LogWarning("[PlayerController] meleeHurtbox niet ingesteld! Voeg een Transform toe voor de hurtbox in de Inspector.");
        SetupHurtbox();
    }

    private void Update()
    {
        CheckGround();
        CheckWallLatch();
        ReadInput();
        HandleCtrlInput();
        HandleRoll();
        HandleCrouchAndSlide();
        HandleMovement();
        HandleJumpAndGravity();
        HandleMeleeAttack();
    }

    private void HandleMeleeAttack()
    {
        if (isRolling || isSliding || !animator || (attackData == null || attackData.Length == 0) || (midAirAttacks == null || midAirAttacks.Length == 0)) return;

        // Controleer of de speler de aanvalsknop ingedrukt houdt
        if (Input.GetKey(attackKey) && !isAttacking && !heavyAttackTriggered && Time.time >= lastHeavyAttackTime + heavyAttackCooldown)
        {
            if (!isHoldingAttack)
            {
                isHoldingAttack = true;
                attackHoldTime = 0f;
            }
            attackHoldTime += Time.deltaTime;

            // Als de hold-tijd de drempel overschrijdt, start een heavy attack
            if (attackHoldTime >= heavyAttackHoldTime)
            {
                StartHeavyAttack();
                heavyAttackTriggered = true;
            }
        }

        // Als de knop wordt losgelaten
        if (Input.GetKeyUp(attackKey))
        {
            if (isHoldingAttack && !heavyAttackTriggered && Time.time >= lastHeavyAttackTime + heavyAttackCooldown)
            {
                PerformNormalAttack();
            }
            isHoldingAttack = false;
            heavyAttackTriggered = false; // Reset de vlag bij het loslaten van de knop
        }

        // Reset combo naar 0 als de time-out is overschreden
        if (Input.GetKeyDown(attackKey) && Time.time >= lastAttackTime + comboTimeWindow)
        {
            comboIndex = 0;
        }
    }

    private void PerformNormalAttack()
    {
        lastAttackTime = Time.time;

        // Bepaal de huidige bewegingsrichting op basis van input
        Vector3 camF = cameraTransform.forward; camF.y = 0f;
        Vector3 camR = cameraTransform.right; camR.y = 0f;
        Vector3 targetDir = (camF.normalized * input.y + camR.normalized * input.x).normalized;

        // Controleer of we in de lucht zijn en of de camera-hoek een plunging attack vereist
        if (!isGrounded)
        {
            float cameraAngle = Vector3.Angle(Vector3.down, cameraTransform.forward);
            if (cameraAngle <= plungingAttackAngleThreshold && !isPlunging)
            {
                animator.SetTrigger("plungeMelee");
                StartCoroutine(PerformPlungingAttack());
                if (debugMelee) Debug.Log("[Melee] Triggered Plunging Attack");
                return;
            }
            else if (comboIndex < midAirAttacks.Length)
            {
                string attackParam = midAirAttacks[comboIndex].attackParam;
                float attackDuration = midAirAttacks[comboIndex].duration;

                animator.SetTrigger("midAirMelee");
                StartCoroutine(PerformAttack(comboIndex, attackDuration, true, targetDir));
                if (debugMelee) Debug.Log($"[Melee] Triggered Mid-Air {attackParam}, Combo Index: {comboIndex + 1}");
                comboIndex = (comboIndex + 1) % midAirAttacks.Length;
                return;
            }
        }

        // Normale aanval (op de grond)
        if (comboIndex < attackData.Length)
        {
            string attackParam = attackData[comboIndex].attackParam;
            float attackDuration = attackData[comboIndex].duration;

            animator.SetTrigger(attackParam);
            StartCoroutine(PerformAttack(comboIndex, attackDuration, false, targetDir));

            if (debugMelee) Debug.Log($"[Melee] Triggered {attackParam}, Combo Index: {comboIndex + 1}");
            comboIndex = (comboIndex + 1) % attackData.Length;
        }

        // Reset combo naar 0 na time-out
        if (Time.time >= lastAttackTime + comboTimeWindow && comboIndex > 0)
        {
            comboIndex = 0;
            animator.SetInteger("ComboIndex", 0);
            if (debugMelee) Debug.Log("[Melee] Combo reset due to timeout");
        }
    }

    private void StartHeavyAttack()
    {
        isAttacking = true; // Direct instellen om herhaling te voorkomen
        lastAttackTime = Time.time;
        lastHeavyAttackTime = Time.time; // Sla de tijd van de heavy attack op
        isHoldingAttack = false;

        // Bepaal de huidige bewegingsrichting op basis van input
        Vector3 camF = cameraTransform.forward; camF.y = 0f;
        Vector3 camR = cameraTransform.right; camR.y = 0f;
        Vector3 targetDir = (camF.normalized * input.y + camR.normalized * input.x).normalized;

        animator.SetTrigger("heavyMelee");
        StartCoroutine(PerformHeavyAttack(targetDir));
        if (debugMelee) Debug.Log("[Melee] Triggered Heavy Attack");
    }

    private void PlaySwordSlamVFX()
    {
        if (!swordSlamVFX || !swordTip) return;

        // Doe een raycast vanaf een iets hogere positie om de grond te vinden
        Vector3 raycastOrigin = swordTip.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, maxGroundCheckDistance + 0.5f, groundMask))
        {
            swordSlamVFX.transform.position = hit.point;
            if (debugVFX) Debug.Log($"[VFX] SwordSlam VFX afgespeeld op positie: {hit.point}");
        }
        else
        {
            // Fallback: gebruik een offset van de speler
            Vector3 fallbackPosition = transform.position + Vector3.down * (controller.height / 2);
            swordSlamVFX.transform.position = fallbackPosition;
            if (debugVFX) Debug.LogWarning($"[VFX] Geen grond gevonden voor SwordSlam VFX, fallback positie gebruikt: {fallbackPosition}");
        }

        swordSlamVFX.Play();
    }

    private IEnumerator PerformAttack(int attackIndex, float attackDuration, bool isMidAir, Vector3 targetDirection)
    {
        isAttacking = true;
        if (animator) animator.SetFloat("Speed", 0f);

        float elapsed = 0f;
        float hitWindowStart = attackDuration * (isMidAir ? midAirAttacks[attackIndex].hitWindowStart : attackData[attackIndex].hitWindowStart);
        float hitWindowEnd = attackDuration * (isMidAir ? midAirAttacks[attackIndex].hitWindowEnd : attackData[attackIndex].hitWindowEnd);
        float damage = isMidAir ? midAirAttacks[attackIndex].damage : attackData[attackIndex].damage;

        // Bereken de doelrotatie op basis van de targetDirection
        Quaternion targetRot = Quaternion.LookRotation(targetDirection.sqrMagnitude > 0.01f ? targetDirection : characterModel.forward);

        // Bereken de tijd voor de VFX (bijv. 50% van de aanvalsduur)
        float vfxTriggerTime = attackDuration * swordSlamVFXDelay;

        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;

            // Soepele rotatie naar de targetDirection
            if (targetDirection.sqrMagnitude > 0.01f)
            {
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // Speel de VFX af op het juiste moment, alleen voor de 4e aanval op de grond
            if (!isMidAir && attackIndex == 3 && elapsed >= vfxTriggerTime && elapsed - Time.deltaTime < vfxTriggerTime)
            {
                PlaySwordSlamVFX();
            }

            if (elapsed >= hitWindowStart && elapsed <= hitWindowEnd)
            {
                ActivateHurtbox(damage);
            }
            yield return null;
        }

        isAttacking = false;

        if (attackIndex == (isMidAir ? midAirAttacks.Length - 1 : attackData.Length - 1))
        {
            yield return new WaitForSeconds(comboTimeWindow);
            if (!Input.GetKeyDown(attackKey)) comboIndex = 0;
        }
    }

    private IEnumerator PerformHeavyAttack(Vector3 targetDirection)
    {
        if (animator) animator.SetFloat("Speed", 0f);

        float elapsed = 0f;
        float attackDuration = heavyAttack.duration;
        float hitWindowStart = attackDuration * heavyAttack.hitWindowStart;
        float hitWindowEnd = attackDuration * heavyAttack.hitWindowEnd;
        float damage = heavyAttack.damage;

        // Bereken de doelrotatie op basis van de targetDirection
        Quaternion targetRot = Quaternion.LookRotation(targetDirection.sqrMagnitude > 0.01f ? targetDirection : characterModel.forward);

        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;

            // Soepele rotatie naar de targetDirection
            if (targetDirection.sqrMagnitude > 0.01f)
            {
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            if (elapsed >= hitWindowStart && elapsed <= hitWindowEnd)
            {
                ActivateHurtbox(damage);
            }
            yield return null;
        }

        isAttacking = false;
    }

    private IEnumerator PerformPlungingAttack()
    {
        isAttacking = true;
        isPlunging = true;
        if (animator) animator.SetFloat("Speed", 0f);

        float elapsed = 0f;
        float hitWindowStart = plungingAttack.duration * plungingAttack.hitWindowStart;
        float hitWindowEnd = plungingAttack.duration * plungingAttack.hitWindowEnd;

        if (velocity.y > -10f)
        {
            velocity.y = -10f;
        }

        while (elapsed < plungingAttack.duration && !isGrounded)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= hitWindowStart && elapsed <= hitWindowEnd)
            {
                ActivateHurtbox(plungingAttack.damage);
            }
            yield return null;
        }

        if (isGrounded)
        {
            if (debugMelee) Debug.Log("[Melee] Plunging attack interrupted by landing");
        }

        isAttacking = false;
        isPlunging = false;
    }

    private void ActivateHurtbox(float damage)
    {
        if (!meleeHurtbox) return;

        // Zorg ervoor dat de hurtbox correct is gepositioneerd en geschaald
        meleeHurtbox.localPosition = Vector3.zero; // Center op de speler
        meleeHurtbox.localScale = hurtboxSize;

        // Activeer de hurtbox alleen tijdens het hit-window
        Collider[] hitColliders = Physics.OverlapBox(meleeHurtbox.position, hurtboxSize / 2f, meleeHurtbox.rotation, enemyLayer);
        foreach (Collider hit in hitColliders)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (debugMelee) Debug.Log($"[Melee] Hit {enemy.name} for {damage} damage with hurtbox");
            }
        }
    }

    private void SetupHurtbox()
    {
        if (!meleeHurtbox) return;

        // Voeg een BoxCollider toe aan de hurtbox als die er nog niet is
        BoxCollider collider = meleeHurtbox.GetComponent<BoxCollider>();
        if (!collider)
        {
            collider = meleeHurtbox.gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true; // Maak het een trigger
        collider.size = hurtboxSize; // Stel de initiële grootte in
    }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        bool physicsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool ccGrounded = controller.isGrounded;
        isGrounded = physicsGrounded || ccGrounded;

        if (isGrounded && velocity.y < 0f && !isWallJumping)
        {
            velocity.y = -2f;
            isWallJumping = false;
            animator.SetTrigger("landing");
        }

        if (!wasGrounded && isGrounded)
        {
            externalVelocity = Vector3.zero;
            blockBulletJump = false;
            blockCrouchSlide = false;
            isBulletJumping = false;
            isWallLatching = false;
            isAimGliding = false;
            canWallJump = false;
            airTime = 0f;
            if (debugMovement && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log("[CheckGround] Geland: velocities en flags gereset");
                lastDebugTime = Time.time;
            }
        }
        else if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }
    }

    private void CheckWallLatch()
    {
        if (Time.time - lastWallJumpTime < wallJumpCooldown)
            return;

        if (isGrounded || isRolling || isSliding)
        {
            if (isWallLatching)
            {
                isWallLatching = false;
                canWallJump = false;
                velocity.y = -2f;
                lastWallLatchExitTime = Time.time;
                if (debugWallLatch && Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log("[WallLatch] Uitgeschakeld: speler op grond of in andere staat");
                    lastDebugTime = Time.time;
                }
            }
            return;
        }

        bool latchKey = Input.GetMouseButton(1);
        Vector3 center = transform.position + controller.center;
        bool hitWall = Physics.SphereCast(center, controller.radius * 0.9f, characterModel.forward, out RaycastHit hit, wallCheckDistance, obstacleMask);

        if (hitWall)
        {
            if (debugWallLatch && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[WallLatch] Muur gedetecteerd: {hit.collider.name}, Laag: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Tag: {hit.collider.tag}");
                Debug.Log($"[WallLatch] Condities - normal.y: {hit.normal.y}, airTime: {airTime}, maxAirTime: {maxAirTime}, latchKey: {latchKey}");
                lastDebugTime = Time.time;
            }
        }
        else if (debugWallLatch && Time.time - lastDebugTime >= 1f)
        {
            Debug.Log("[WallLatch] Geen muur gedetecteerd binnen bereik.");
            lastDebugTime = Time.time;
        }

        if (hitWall && !isGrounded && latchKey && airTime < maxAirTime && hit.normal.y < 0.8f)
        {
            canWallJump = true;
            wallNormal = hit.normal;
            isWallLatching = true;
            isAimGliding = false;
            velocity.y = 0f;
            externalVelocity = Vector3.zero;
            moveDirection = Vector3.zero;
            Vector3 pushToWall = -wallNormal * 0.1f;
            controller.Move(pushToWall);
            if (debugWallLatch && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[WallLatch] Actief: wallNormal={wallNormal}, position={transform.position}, airTime={airTime}, velocity.y={velocity.y}");
                lastDebugTime = Time.time;
            }
        }
        else if (isWallLatching)
        {
            isWallLatching = false;
            canWallJump = false;
            velocity.y = -2f;
            lastWallLatchExitTime = Time.time;
            if (debugWallLatch && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log("[WallLatch] Uitgeschakeld: voorwaarden niet meer voldaan");
                lastDebugTime = Time.time;
            }
        }
    }

    private void ReadInput()
    {
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    private void HandleCtrlInput()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (isSliding)
            {
                StopSlide();
            }
            blockBulletJump = false;
            blockCrouchSlide = false;
        }
    }

    private void HandleRoll()
    {
        if (isRolling || isSliding)
            return;
        if (Time.time < lastRollTime + rollCooldown)
            return;
        if (input.magnitude > 0.1f && Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (input.x < 0)
                animator.SetTrigger("dodgerollLeft");
            else if (input.x > 0)
                animator.SetTrigger("dodgerollRight");
            else
                animator.SetTrigger("dodgeroll");
            StartCoroutine(RollRoutine());
        }
    }

    private IEnumerator RollRoutine()
    {
        isRolling = true;
        lastRollTime = Time.time;
        Vector3 camF = cameraTransform.forward; camF.y = 0f;
        Vector3 dir = (camF.normalized * input.y + cameraTransform.right.normalized * input.x).normalized;
        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            controller.Move(dir * (rollDistance / rollDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isRolling = false;
    }

    private void HandleCrouchAndSlide()
    {
        bool moving = input.magnitude > 0.1f;
        bool crouchKey = Input.GetKey(KeyCode.LeftControl);
        bool crouchDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool crouchUp = Input.GetKeyUp(KeyCode.LeftControl);

        if (blockCrouchSlide)
            return;

        if (crouchDown && moving && isGrounded && !isSliding && !isRolling)
            StartSlide();
        else if (crouchKey && isGrounded && !isSliding)
            StartCrouch();
        else if ((crouchUp || (!crouchKey && isCrouching)) && !isSliding && CanStandUp())
            StopCrouch();

        if (animator)
            animator.SetBool("Crouching", isCrouching);
    }

    private void StartCrouch()
    {
        controller.height = crouchHeight;
        controller.center = new Vector3(0f, crouchHeight / 2f, 0f);
        isCrouching = true;
    }

    private void StopCrouch()
    {
        if (!CanStandUp())
            return;
        controller.height = originalHeight;
        controller.center = originalCenter;
        isCrouching = false;
    }

    private void StartSlide()
    {
        StartCrouch();
        isSliding = true;
        externalVelocity = GetCameraRelativeDirection() * moveSpeed * slideInitialMultiplier;
        isBulletJumping = false;
    }

    private void StopSlide()
    {
        isSliding = false;
        externalVelocity = Vector3.zero;
    }

    private bool CanStandUp()
    {
        Vector3 start = transform.position + controller.center;
        float checkHeight = originalHeight - crouchHeight;
        bool headBlocked = Physics.Raycast(start, Vector3.up, checkHeight, obstacleMask);
        return !headBlocked;
    }

    private Vector3 GetCameraRelativeDirection()
    {
        Vector3 camF = cameraTransform.forward; camF.y = 0f;
        Vector3 camR = cameraTransform.right; camR.y = 0f;
        return (camF.normalized * input.y + camR.normalized * input.x).normalized;
    }

    private void ExitCrouchStates()
    {
        StopCrouch();
        isSliding = false;
        blockCrouchSlide = false;
    }

    private void HandleMovement()
    {
        if (isRolling || isWallLatching || isAttacking)
            return;

        Vector3 totalMove = Vector3.zero;

        if (input.y < 0)
            animator.SetTrigger("strafeBack");
        else if (input.x < 0)
            animator.SetTrigger("strafeLeft");
        else if (input.x > 0)
            animator.SetTrigger("strafeRight");

        if (externalVelocity.sqrMagnitude > 0.01f)
        {
            totalMove += externalVelocity * Time.deltaTime;
            if (isSliding && isGrounded)
            {
                externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, slideFriction * Time.deltaTime);
                if (externalVelocity.magnitude < slideMinimumSpeed)
                    StopSlide();
            }
            if (debugMovement && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[Movement] Speciale beweging: totalMove={totalMove}, velocity.y={velocity.y}, externalVelocity={externalVelocity}, position={transform.position}");
                lastDebugTime = Time.time;
            }
        }
        else
        {
            externalVelocity = Vector3.zero;
            Vector3 camF = cameraTransform.forward; camF.y = 0f;
            Vector3 camR = cameraTransform.right; camR.y = 0f;
            Vector3 targetDir = (camF.normalized * input.y + camR.normalized * input.x).normalized;
            if (targetDir.sqrMagnitude > 0.01f)
            {
                moveDirection = Vector3.SmoothDamp(moveDirection, targetDir, ref smoothDirVelocity, directionSmoothTime);
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
                float speed = blockCrouchSlide ? moveSpeed : (isCrouching ? moveSpeed * 0.5f : moveSpeed);
                totalMove += moveDirection * speed * Time.deltaTime;
            }
            else
                moveDirection = Vector3.zero;
        }

        totalMove += Vector3.up * velocity.y * Time.deltaTime;
        if (isWallJumping)
        {
            //velocity.y = Mathf.Max(velocity.y, 0);
        }
        controller.Move(totalMove);

        if (isWallJumping && debugMovement && Time.time - lastDebugTime >= 0.1f)
        {
            Debug.Log($"[Movement] Post-wall jump: velocity.y={velocity.y}, position={transform.position}");
            lastDebugTime = Time.time;
        }

        if (animator)
            animator.SetFloat("Speed", moveDirection.magnitude);
    }

    private void HandleJumpAndGravity()
    {
        bool headClear = !isCrouching || CanStandUp();
        bool glideKey = Input.GetMouseButton(1);

        if (canWallJump && Input.GetKeyDown(KeyCode.Space))
        {
            isWallLatching = false;
            canWallJump = false;
            isAimGliding = false;
            isBulletJumping = false;
            isWallJumping = true;
            lastWallJumpTime = Time.time;
            lastWallLatchExitTime = Time.time;

            controller.Move(-wallNormal * 2.0f);

            if (debugJump && Time.time - lastDebugTime >= 0.1f)
            {
                Debug.Log($"[WallJump] Pre-velocity: isWallLatching={isWallLatching}, position={transform.position}");
                lastDebugTime = Time.time;
            }

            Vector3 jumpDirection = Vector3.zero;

            if (input.y > 0)
            {
                jumpDirection = Vector3.up * wallJumpUpwardForce + (-wallNormal * wallJumpHorizontalForce);
                animator.SetTrigger("wallJumpUp");
            }
            else if (input.x < 0)
            {
                Vector3 sideDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
                jumpDirection = (Vector3.up * wallJumpUpwardForce * 0.7f) + (sideDirection * input.x * wallJumpHorizontalForce);
                animator.SetTrigger("wallJumpLeft");
            }
            else if (input.x > 0)
            {
                Vector3 sideDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
                jumpDirection = (Vector3.up * wallJumpUpwardForce * 0.7f) + (sideDirection * input.x * wallJumpHorizontalForce);
                animator.SetTrigger("wallJumpRight");
            }
            else
            {
                jumpDirection = Vector3.up * wallJumpUpwardForce + (-wallNormal * wallJumpHorizontalForce);
                animator.SetTrigger("wallJumpUp");
            }

            velocity = jumpDirection;
            externalVelocity = Vector3.zero;

            if (debugJump && Time.time - lastDebugTime >= 0.1f)
            {
                Debug.Log($"[WallJump] Uitgevoerd: velocity={velocity}, wallJumpUpwardForce={wallJumpUpwardForce}, wallNormal={wallNormal}, input={input}");
                lastDebugTime = Time.time;
            }
            return;
        }

        if (!blockBulletJump && Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            Vector3 camForward = cameraTransform.forward;
            float angle = Vector3.Angle(camForward, Vector3.up);
            float minAngle = bulletJumpMinAngle;

            // Als de hoek kleiner is dan de minimale hoek, pas de richting aan
            if (angle < minAngle)
            {
                Vector3 adjustedDirection = Vector3.Slerp(camForward, Vector3.up, (minAngle - angle) / minAngle);
                camForward = adjustedDirection.normalized;
            }

            // Stel de horizontale richting in (alleen x en z componenten)
            Vector3 horizontalDirection = new Vector3(camForward.x, 0f, camForward.z).normalized;

            // Stel de opwaartse velocity in
            velocity = Vector3.up * bulletJumpUpwardForce;

            // Stel de horizontale velocity in
            externalVelocity = horizontalDirection * bulletJumpSpeedMultiplier;

            // Zet flags en resets voor de bullet jump
            isBulletJumping = true;
            animator.SetTrigger("bulletJump");
            StopSlide();
            if (CanStandUp())
                StopCrouch();
            blockBulletJump = true;
            blockCrouchSlide = true;

            // Debug informatie om te controleren
            if (debugJump && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[BulletJump] Uitgevoerd: horizontalDirection={horizontalDirection}, velocity={velocity}, externalVelocity={externalVelocity}, bulletJumpSpeedMultiplier={bulletJumpSpeedMultiplier}, camForward={camForward}");
                lastDebugTime = Time.time;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && headClear)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("jump");
            isBulletJumping = false;
            ExitCrouchStates();
            if (debugJump && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[NormalJump] Uitgevoerd: velocity.y={velocity.y}");
                lastDebugTime = Time.time;
            }
        }

        if (isWallLatching)
        {
            velocity.y = 0f;
            if (debugWallLatch && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[WallLatch] Speler blijft aan muur hangen: velocity.y={velocity.y} (gravity uitgeschakeld)");
                lastDebugTime = Time.time;
            }
        }
        else if (!isGrounded)
        {
            if (glideKey && airTime < maxAirTime &&
                (Time.time - lastWallJumpTime > wallJumpGlideCooldown) &&
                (Time.time - lastWallJumpTime > wallJumpAimGlideCooldown) &&
                (Time.time - lastWallLatchExitTime > aimGlideDelayAfterWall))
            {
                if (!isAimGliding)
                {
                    isAimGliding = true;
                    glideStartTime = Time.time;
                    initialGlideVelocityY = velocity.y;
                    animator.SetTrigger("aimGlide");
                }

                float glideElapsed = Time.time - glideStartTime;
                float lerpFraction = Mathf.Clamp01(glideElapsed / glideLerpTime);

                if (velocity.y > 0)
                {
                    velocity.y = Mathf.Lerp(initialGlideVelocityY, 0, lerpFraction);
                }

                float grav = gravity * glideGravityScale;
                velocity.y += grav * Time.deltaTime;

                if (debugJump && Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log($"[AimGlide] Actief: velocity.y={velocity.y}, gravity={grav}, lerpFraction={lerpFraction}, initialGlideVelocityY={initialGlideVelocityY}");
                    lastDebugTime = Time.time;
                }
            }
            else if (Time.time - lastWallJumpTime > wallJumpGravityDelay || !isWallJumping)
            {
                isAimGliding = false;
                velocity.y += gravity * Time.deltaTime;
                if (debugJump && Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log($"[Gravity] Toegepast: velocity.y={velocity.y}, gravity={gravity}, isAimGliding={isAimGliding}, isWallJumping={isWallJumping}");
                    lastDebugTime = Time.time;
                }
            }
            else if (debugJump && Time.time - lastDebugTime >= 0.1f)
            {
                Debug.Log($"[WallJump] Gravity uitgesteld: velocity.y={velocity.y}");
                lastDebugTime = Time.time;
            }
        }
        else if (isWallJumping && (Time.time - lastWallJumpTime > wallJumpGravityDelay || velocity.y <= 0))
        {
            isWallJumping = false;
            if (debugJump && Time.time - lastDebugTime >= 1f)
            {
                Debug.Log($"[WallJump] Gravity hervat: velocity.y={velocity.y}");
                lastDebugTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controller != null && characterModel != null)
        {
            Vector3 center = transform.position + controller.center;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, controller.radius * 0.9f);
            Gizmos.DrawRay(center, characterModel.forward * wallCheckDistance);
        }

        // Teken de hurtbox voor visualisatie in de editor
        if (meleeHurtbox != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(meleeHurtbox.position, hurtboxSize);
        }
    }
}
=======
    private bool isRolling = false;
    public bool isInvincible = false;
    private float lastRollTime;
    private Vector3 rollDirection;

    [Header("Crouch & Slide")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float originalHeight;
    private Vector3 originalCenter;
    private bool isCrouching = false;

    public float slideDistance = 6f;
    public float slideDuration = 0.5f;
    private bool isSliding = false;
    private float lastSlideTime;

    [Header("Slide Physics")]
    public float slopeDrag = 10f;
    public float slideStopCheckDistance = 0.5f;
    public LayerMask obstacleMask;

    private Vector2 input;
    private Vector3 moveDirection;
    [SerializeField] private Transform characterModel;

    void Start()
    {
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();

        if (!isRolling && Time.time >= lastRollTime + rollCooldown)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && input.magnitude > 0.1f)
            {
                StartCoroutine(PerformRoll());
            }
        }

        HandleJumpAndGravity();
        HandleCrouchAndSlide();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (!isRolling && !isSliding)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * input.y + camRight * input.x;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            if (moveDirection != Vector3.zero)
                characterModel.LookAt(characterModel.position + moveDirection);
        }
    }

    void HandleJumpAndGravity()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        Vector3 totalVelocity = velocity + externalVelocity;
        controller.Move(totalVelocity * Time.deltaTime);

        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 5f * Time.deltaTime);
    }

    void HandleCrouchAndSlide()
    {
        bool isCtrlHeld = Input.GetKey(KeyCode.LeftControl);
        bool isCtrlDown = Input.GetKeyDown(KeyCode.LeftControl);

        if (isCtrlHeld && !isCrouching && !isSliding && input.magnitude < 0.1f)
        {
            StartCrouch();
        }
        else if (!isCtrlHeld && isCrouching)
        {
            StopCrouch();
        }

        if (isCtrlDown && input.magnitude > 0.1f && isGrounded && !isSliding && !isRolling)
        {
            StartCoroutine(PerformSlide());
        }
    }

    void StartCrouch()
    {
        isCrouching = true;
        controller.height = crouchHeight;
        controller.center = new Vector3(0, crouchHeight / 2f, 0);
    }

    void StopCrouch()
    {
        isCrouching = false;
        controller.height = originalHeight;
        controller.center = new Vector3(0, originalHeight / 2f, 0);
    }

    IEnumerator PerformSlide()
    {
        isSliding = true;
        controller.height = crouchHeight;
        controller.center = new Vector3(0, crouchHeight / 2f, 0);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 slideDirection = (camForward * input.y + camRight * input.x).normalized;

        // Hou momentum aan, tenzij slide sneller is
        Vector3 currentHorizontal = new Vector3(velocity.x + externalVelocity.x, 0, velocity.z + externalVelocity.z);
        Vector3 desiredSlideVelocity = slideDirection * (slideDistance / slideDuration);

        if (desiredSlideVelocity.magnitude > currentHorizontal.magnitude)
        {
            externalVelocity = desiredSlideVelocity;
        }

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            // Check voor muur voor speler
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, slideDirection, slideStopCheckDistance, obstacleMask))
            {
                Debug.Log("Slide gestopt door muur");
                break;
            }

            // Extra vertraging op hellingen
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, slopeDrag * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = originalHeight;
        controller.center = new Vector3(0, originalHeight / 2f, 0);
        isSliding = false;
    }

    IEnumerator PerformRoll()
    {
        isRolling = true;
        isInvincible = true;
        lastRollTime = Time.time;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        rollDirection = (camForward * input.y + camRight * input.x).normalized;

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            controller.Move(rollDirection * (rollDistance / rollDuration) * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
        yield return new WaitForSeconds(0.1f);
        isInvincible = false;
    }

    public void AddExternalForce(Vector3 force)
    {
        externalVelocity += force;
    }
}
>>>>>>> Stashed changes
