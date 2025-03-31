using UnityEngine;
using System.Collections;

public class WeepingAngel : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float reducedSpeed = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float rotationSpeed = 5f;
    private float originalRotationSpeed;

    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    private float originalMoveSpeed;
    private Transform target;
    private Animator anim;
    private Rigidbody rb;
    private bool hasEnteredAttackRange = false;
    private WeaponHitbox swordHitbox;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        swordHitbox = GetComponentInChildren<WeaponHitbox>();
        if (swordHitbox != null)
            swordHitbox.DisableHitbox(); // make sure it's off by default
        originalMoveSpeed = moveSpeed;
        originalRotationSpeed = rotationSpeed;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        float currentSpeed = isAttacking ? reducedSpeed : moveSpeed;

        if (distance > attackRange && currentSpeed > 0f)
        {
            float moveStep = currentSpeed * Time.fixedDeltaTime;

            // Prevent overshooting
            if (moveStep > distance - attackRange)
                moveStep = distance - attackRange;

            rb.MovePosition(rb.position + direction * moveStep);
        }


        if (direction != Vector3.zero && rotationSpeed > 0f) // rotate to player
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
        }


        // walk animation toggle
        if (anim != null)
            anim.SetBool("IsMoving", !isAttacking && currentSpeed > 0f && distance > attackRange + 0.1f);

        if (distance <= attackRange + 0.05f && attackCooldownTimer <= 0f && !isAttacking)
        {
            StartCoroutine(Attack());
        }



        attackCooldownTimer -= Time.fixedDeltaTime;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        if (swordHitbox != null)
            swordHitbox.EnableHitbox();
        if (anim != null)
        {
            int index = Random.Range(0, 2);
            anim.SetInteger("AttackIndex", index);
            anim.SetTrigger("AttackTrigger");
        }

        attackCooldownTimer = attackCooldown;

        yield return new WaitForSeconds(1.1f); 

        if (swordHitbox != null)
            swordHitbox.DisableHitbox();

        isAttacking = false;
    }

    public void Freeze()
    {
        Debug.Log($"[Angel] {name} frozen.");
        moveSpeed = 0f;
        rotationSpeed = 0f;
        if (anim != null) anim.speed = 0f;
    }

    public void Unfreeze()
    {
        Debug.Log($"[Angel] {name} unfrozen.");
        moveSpeed = originalMoveSpeed;
        rotationSpeed = originalRotationSpeed;
        if (anim != null) anim.speed = 1f;
    }

}
