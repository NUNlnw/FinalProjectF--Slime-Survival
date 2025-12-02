using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : Identity, Idestoryable
{
    int _health;

    public int health
    {
        get => _health;
        set => _health = Mathf.Clamp(value, 0, _maxHealth);
    }

    public int maxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    [SerializeField]
    private int _maxHealth = 100;

    public int Damage = 10;
    public int Deffent = 10;         // defense เป็น %
    public float movementSpeed;

    protected Animator animator;
    protected Rigidbody rb;
    Quaternion newRotation;

    public event Action<Idestoryable> OnDestory;

    public override void SetUP()
    {
        base.SetUP();
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    public virtual void TakeDamage(int amount)
    {
        // defense เป็นเปอร์เซ็น เช่น 25 = ลด 25%
        float damageMultiplier = 1f - (Deffent / 100f);
        damageMultiplier = Mathf.Clamp(damageMultiplier, 0f, 1f);

        int finalDamage = Mathf.RoundToInt(amount * damageMultiplier);
        finalDamage = Mathf.Max(finalDamage, 1); // อย่างน้อย 1

        health -= finalDamage;

        health -= finalDamage;

        if (health <= 0)
        {
            OnDestory?.Invoke(this);   // <<< สำคัญมาก อย่าลืมบรรทัดนี้
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} takes {finalDamage} damage. Remaining health: {health}");
        }
    }

    public virtual void Heal(int amount)
    {
        health += amount;
    }

    protected virtual void Turn(Vector3 direction)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 15f);

        if (rb.linearVelocity.magnitude < 0.1f || direction == Vector3.zero) return;
        newRotation = Quaternion.LookRotation(direction);
    }

    protected virtual void Move(Vector3 direction)
    {
        rb.linearVelocity = new Vector3(direction.x * movementSpeed, rb.linearVelocity.y, direction.z * movementSpeed);
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }
}
