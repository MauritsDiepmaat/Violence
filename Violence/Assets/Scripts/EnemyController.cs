using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f; // Afstand waarin de enemy de speler kan detecteren
    public float alertRange = 5f; // Afstand waarin de enemy andere enemies kan waarschuwen
    public LayerMask playerLayer; // Laag van de speler
    public LayerMask obstacleLayer; // Lagen die het zicht kunnen blokkeren (bijv. muren)

    [Header("Alert Settings")]
    public float alertCooldown = 5f; // Tijd in seconden voordat de enemy niet meer alert is

    [Header("Strafe Settings")]
    public float strafeSpeed = 3f; // Snelheid van de strafe-beweging (graden per seconde)
    public float minStrafeDistance = 3f; // Minimale strafe-afstand tot de speler
    public float maxStrafeDistance = 6f; // Maximale strafe-afstand tot de speler
    public float repulsionForce = 2f; // Kracht waarmee enemies elkaar afstoten als ze te dichtbij komen
    public float repulsionDistance = 1f; // Afstand waarop enemies elkaar beginnen af te stoten

    [Header("Attack Settings")]
    public float backAttackAngle = 45f; // Maximale hoek (in graden) om te bepalen of de enemy achter de speler is
    public float backAttackCooldown = 3f; // Cooldown voor de back attack in seconden
    public float periodicAttackInterval = 5f; // Interval voor periodieke aanval in seconden
    public float backJumpDistance = 2f; // Afstand die de enemy terug springt na een back attack
    public float attackApproachDistance = 1f; // Afstand waarop de enemy de speler benadert voor de aanval

    [Header("Health Settings")]
    public float maxHealth = 100f; // Maximale gezondheid van de enemy

    [Header("Explosion Settings")]
    public ParticleSystem explosionParticle; // Referentie naar de explosie Particle System

    [Header("References")]
    public Transform player; // Referentie naar de speler
    public CharacterController controller; // CharacterController voor beweging

    // Private state
    private float currentHealth;
    private bool isAlerted = false; // Is de enemy alert?
    private float lastDebugTime;
    private float timeSinceLastSeen; // Tijd sinds de speler voor het laatst is gezien
    private bool hasLineOfSight; // Heeft de enemy direct zicht op de speler?
    private float strafeAngle; // Huidige hoek voor de strafe-beweging
    private float strafeDistance; // Dynamische afstand tot de speler tijdens strafen
    private float uniqueOffset; // Unieke offset om strafeDistance te variëren
    private float lastBackAttackTime; // Tijd van de laatste back attack
    private float lastPeriodicAttackTime; // Tijd van de laatste periodieke aanval
    private bool isAttacking; // Is de enemy bezig met een aanval (inclusief terugspringen)?
    private Vector3 startPosition; // Oorspronkelijke positie vóór de aanval
    private bool stayBehindPlayer; // Moet de enemy achter de speler blijven na de aanval?
    private bool hasExploded = false; // Nieuwe vlag om te voorkomen dat de explosie meerdere keren afspeelt

    private void Awake()
    {
        // Zoek de speler automatisch als deze niet is ingesteld
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
                Debug.LogError("[EnemyController] Speler niet gevonden! Zorg ervoor dat de speler de tag 'Player' heeft.");
        }

        // Haal de CharacterController op
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("[EnemyController] CharacterController niet gevonden! Voeg een CharacterController toe aan de enemy.");

        // Controleer of playerLayer is ingesteld
        if (playerLayer == 0)
            Debug.LogWarning("[EnemyController] playerLayer niet ingesteld! Stel een geldige laag in via de Inspector.");

        // Controleer of obstacleLayer is ingesteld
        if (obstacleLayer == 0)
            Debug.LogWarning("[EnemyController] obstacleLayer niet ingesteld! Stel een geldige laag in via de Inspector.");

        // Stel een unieke offset in voor deze enemy om de strafeDistance te variëren
        uniqueOffset = Random.Range(0f, 1f); // Willekeurige waarde tussen 0 en 1
        strafeDistance = Mathf.Lerp(minStrafeDistance, maxStrafeDistance, uniqueOffset);
        strafeAngle = Random.Range(0f, 360f); // Start met een willekeurige hoek
        currentHealth = maxHealth; // Initialiseer gezondheid
    }

    private void Update()
    {
        // Controleer of de speler binnen detectieafstand is en in zicht
        CheckPlayerInRange();

        // Als de enemy alert is, waarschuw andere enemies en voer beweging/aanval uit
        if (isAlerted)
        {
            AlertNearbyEnemies();

            if (!isAttacking) // Voer strafe en aanvallen alleen uit als de enemy niet aan het aanvallen is
            {
                if (!stayBehindPlayer)
                {
                    StrafeAroundPlayer();
                }
                else
                {
                    StayBehindPlayer();
                }
                HandleAttacks();
            }
            else
            {
                // Blijf kijken naar de speler tijdens de aanval of terugsprong
                LookAtPlayer();
            }
        }

        // Beheer de cooldown van de alerted state
        ManageAlertCooldown();
    }

    private void CheckPlayerInRange()
    {
        // Bereken de afstand tot de speler
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Controleer of de speler binnen de detectieafstand is
        if (distanceToPlayer <= detectionRange)
        {
            // Voer een Raycast uit om te controleren of er direct zicht is op de speler
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayerRay = Vector3.Distance(transform.position, player.position);
            hasLineOfSight = false;

            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayerRay, obstacleLayer))
            {
                // Geen obstakels tussen de enemy en de speler, dus er is direct zicht
                hasLineOfSight = true;
                if (!isAlerted)
                {
                    isAlerted = true;
                    if (Time.time - lastDebugTime >= 1f)
                    {
                        Debug.Log($"[EnemyController] Speler gedetecteerd op afstand {distanceToPlayer:F2}m met direct zicht! Enemy is nu alert.");
                        lastDebugTime = Time.time;
                    }
                }
                timeSinceLastSeen = 0f; // Reset de timer omdat de speler in zicht is
            }
            else
            {
                // Obstakel in de weg, geen direct zicht
                if (Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log($"[EnemyController] Speler binnen bereik ({distanceToPlayer:F2}m), maar geen direct zicht door obstakel.");
                    lastDebugTime = Time.time;
                }
            }
        }
        else
        {
            hasLineOfSight = false;
            if (Time.time - lastDebugTime >= 1f && isAlerted)
            {
                Debug.Log($"[EnemyController] Speler buiten detectieafstand ({distanceToPlayer:F2}m).");
                lastDebugTime = Time.time;
            }
        }
    }

    private void AlertNearbyEnemies()
    {
        // Zoek alle objecten binnen de alertRange
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, alertRange, playerLayer.value);

        foreach (Collider obj in nearbyObjects)
        {
            // Controleer of het object een andere enemy is
            EnemyController otherEnemy = obj.GetComponent<EnemyController>();
            if (otherEnemy != null && otherEnemy != this && !otherEnemy.IsAlerted())
            {
                otherEnemy.SetAlerted(true);
                if (Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log($"[EnemyController] Waarschuwde andere enemy {otherEnemy.name} op positie {otherEnemy.transform.position}");
                    lastDebugTime = Time.time;
                }
            }
        }
    }

    private void ManageAlertCooldown()
    {
        if (!isAlerted) return;

        // Als de speler niet in zicht is, verhoog de timer
        if (!hasLineOfSight)
        {
            timeSinceLastSeen += Time.deltaTime;
            if (timeSinceLastSeen >= alertCooldown)
            {
                isAlerted = false;
                timeSinceLastSeen = 0f;
                stayBehindPlayer = false; // Reset stayBehindPlayer als de enemy niet meer alert is
                if (Time.time - lastDebugTime >= 1f)
                {
                    Debug.Log($"[EnemyController] Speler te lang buiten bereik, alerted state uitgeschakeld.");
                    lastDebugTime = Time.time;
                }
            }
        }
    }

    private void StrafeAroundPlayer()
    {
        // Bereken de gewenste positie op de cirkel rond de speler
        strafeAngle += strafeSpeed * Time.deltaTime; // Verhoog de hoek voor de cirkelbeweging
        if (strafeAngle >= 360f) strafeAngle -= 360f; // Houd de hoek tussen 0 en 360

        Vector3 offset = new Vector3(Mathf.Cos(strafeAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(strafeAngle * Mathf.Deg2Rad)) * strafeDistance;
        Vector3 targetPosition = player.position + offset;

        // Bereken de richting naar de target positie
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Beweeg naar de target positie
        if (distanceToTarget > 0.1f)
        {
            Vector3 move = directionToTarget * strafeSpeed * Time.deltaTime;
            controller.Move(move);
        }

        // Voeg afstotingslogica toe om botsingen met andere enemies te voorkomen
        ApplyRepulsion();

        // Laat de enemy naar de speler kijken
        LookAtPlayer();

        if (Time.time - lastDebugTime >= 1f)
        {
            Debug.Log($"[EnemyController] Strafing: strafeDistance={strafeDistance:F2}, position={transform.position}, angle={strafeAngle:F2}");
            lastDebugTime = Time.time;
        }
    }

    private void StayBehindPlayer()
    {
        // Bereken de gewenste positie achter de speler
        Vector3 playerBackward = -player.forward * strafeDistance;
        Vector3 targetPosition = player.position + playerBackward;

        // Bereken de richting naar de target positie
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Beweeg naar de target positie
        if (distanceToTarget > 0.1f)
        {
            Vector3 move = directionToTarget * strafeSpeed * Time.deltaTime;
            controller.Move(move);
        }

        // Voeg afstotingslogica toe
        ApplyRepulsion();

        // Laat de enemy naar de speler kijken
        LookAtPlayer();

        if (Time.time - lastDebugTime >= 1f)
        {
            Debug.Log($"[EnemyController] Staying behind player: targetPosition={targetPosition}, position={transform.position}");
            lastDebugTime = Time.time;
        }
    }

    private void ApplyRepulsion()
    {
        // Zoek andere enemies in de buurt
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, repulsionDistance, playerLayer.value);

        foreach (Collider other in nearbyEnemies)
        {
            EnemyController otherEnemy = other.GetComponent<EnemyController>();
            if (otherEnemy != null && otherEnemy != this)
            {
                // Bereken de richting weg van de andere enemy
                Vector3 directionAway = (transform.position - otherEnemy.transform.position).normalized;
                float distanceToOther = Vector3.Distance(transform.position, otherEnemy.transform.position);

                // Pas een afstotingskracht toe als de enemies te dicht bij elkaar zijn
                if (distanceToOther < repulsionDistance && distanceToOther > 0.01f)
                {
                    Vector3 repulsion = directionAway * (repulsionForce * (repulsionDistance - distanceToOther) / repulsionDistance) * Time.deltaTime;
                    controller.Move(repulsion);
                }
            }
        }
    }

    private void HandleAttacks()
    {
        if (!hasLineOfSight) return; // Alleen aanvallen als de speler in zicht is

        // Controleer of de enemy achter de speler is voor een back attack
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 playerForward = player.forward;
        float angleToPlayer = Vector3.Angle(playerForward, directionToPlayer);

        if (angleToPlayer <= backAttackAngle && Time.time - lastBackAttackTime >= backAttackCooldown)
        {
            PerformBackAttack();
        }
        else if (Time.time - lastPeriodicAttackTime >= periodicAttackInterval)
        {
            PerformPeriodicAttack();
        }
    }

    private void PerformBackAttack()
    {
        lastBackAttackTime = Time.time;
        startPosition = transform.position; // Sla de startpositie op voordat de aanval begint
        Debug.Log($"[EnemyController] Back Attack uitgevoerd op speler! Startpositie={startPosition}");

        // Bereken een aanvalspositie dicht bij de speler (achter hem)
        Vector3 attackPosition = player.position - player.forward * attackApproachDistance;
        StartCoroutine(PerformAttackSequence(attackPosition));
    }

    private void PerformPeriodicAttack()
    {
        lastPeriodicAttackTime = Time.time;
        Debug.Log($"[EnemyController] Periodieke aanval uitgevoerd op speler! Positie={transform.position}");
    }

    private System.Collections.IEnumerator PerformAttackSequence(Vector3 attackPosition)
    {
        isAttacking = true;

        // Stap 1: Spring naar de aanvalspositie dicht bij de speler
        float approachDuration = 0.3f; // Kortere duur voor de aanvalssprong
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < approachDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / approachDuration;
            Vector3 newPosition = Vector3.Lerp(startPosition, attackPosition, t);
            Vector3 move = newPosition - transform.position;
            controller.Move(move);
            yield return null;
        }
        controller.Move(attackPosition - transform.position); // Zorg voor exacte positie

        // Simuleer de aanval (bijv. schade of animatie hier toevoegen)
        Debug.Log($"[EnemyController] Aanval uitgevoerd op positie {attackPosition}");

        // Stap 2: Spring terug naar de startpositie
        elapsed = 0f;
        while (elapsed < approachDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / approachDuration;
            Vector3 newPosition = Vector3.Lerp(attackPosition, startPosition, t);
            Vector3 move = newPosition - transform.position;
            controller.Move(move);
            yield return null;
        }
        controller.Move(startPosition - transform.position); // Zorg voor exacte terugkeer

        // Schakel aanval en stayBehindPlayer modus in
        isAttacking = false;
        stayBehindPlayer = true; // Blijf nu achter de speler
    }

    private void LookAtPlayer()
    {
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0f; // Behoud de y-positie van de enemy
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        }
    }

    // Getter om te controleren of de enemy alert is
    public bool IsAlerted()
    {
        return isAlerted;
    }

    // Setter om de alerted state te wijzigen (bijv. door andere enemies)
    public void SetAlerted(bool value)
    {
        isAlerted = value;
        if (isAlerted)
        {
            timeSinceLastSeen = 0f; // Reset de timer als de enemy wordt gealert
        }
    }

    // Methode om schade toe te passen
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"[Enemy] {gameObject.name} took {damage} damage, remaining health: {currentHealth}");

        if (currentHealth <= 0 && !hasExploded)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[Enemy] {gameObject.name} has died!");
        if (explosionParticle != null && !hasExploded)
        {
            ParticleSystem particleInstance = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            if (particleInstance != null)
            {
                particleInstance.Play();
                Debug.Log($"[Enemy] Explosion particle instantiated and played at {transform.position}");
                Destroy(particleInstance.gameObject, particleInstance.main.duration);
                hasExploded = true; // Markeer dat de explosie heeft plaatsgevonden
            }
            else
            {
                Debug.LogError("[Enemy] Failed to instantiate explosion particle!");
            }
        }
        else if (hasExploded)
        {
            Debug.LogWarning("[Enemy] Explosion already triggered, skipping additional calls.");
        }
        else
        {
            Debug.LogWarning("[Enemy] explosionParticle is not assigned in the Inspector!");
        }
        Destroy(gameObject); // Verwijder de enemy
    }

    private void OnDrawGizmos()
    {
        // Visualiseer de detectie- en alertranges in de editor
        Gizmos.color = isAlerted ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (isAlerted)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, alertRange);

            // Teken een lijn naar de speler om line-of-sight te visualiseren
            if (player != null)
            {
                Gizmos.color = hasLineOfSight ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, player.position);

                // Teken de strafe-afstand
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(player.position, strafeDistance);
            }
        }

        // Teken de repulsionDistance
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, repulsionDistance);
    }
}