using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    /*[Header("Hand setting")]
    public Transform RightHand;
    public Transform LeftHand;
    public List<Item> inventory = new List<Item>();*/

    Vector3 _inputDirection;
    bool _isAttacking = false;
    bool _isInteract = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float jumpForce = 5f;
    private bool _isJump = false;
    private bool _isGrounded = true;

    bool _isMovingForSound = false;
    // --- hit SFX cooldown ---
    public float hitSfxCooldown = 0.1f;   // เวลาขั้นต่ำระหว่างเสียงโดนตีแต่ละครั้ง
    float lastHitSfxTime = -999f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = maxHealth;
    }

    public void FixedUpdate()
    {
        Move(_inputDirection);
        Turn(_inputDirection);
        Attack(_isAttacking);
        Interact(_isInteract);
        Jump(_isJump);
    }
    public void Update()
    {
        HandleInput();
        HandleFootstepSound();
    }
    /*public void AddItem(Item item) {
        inventory.Add(item);
    }*/
    
    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        _inputDirection = new Vector3(x, 0, y);
        /*if (Input.GetMouseButtonDown(0)) {
            _isAttacking = true;
        }*/
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isInteract = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isJump = true;
        }

    }
    public void Attack(bool isAttacking) {
        if (isAttacking) {
            animator.SetTrigger("Attack");
            var e = InFront as Idestoryable;
            if (e != null)
            {
                e.TakeDamage(Damage);
                Debug.Log($"{gameObject.name} attacks for {Damage} damage.");
            }
            _isAttacking = false;
        }
    }
    private void Interact(bool interactable)
    {
        if (interactable)
        {
            IInteractable e = InFront as IInteractable;
            if (e != null) {
                e.Interact(this);
            }
            _isInteract = false;

        }
    }
    //เพิ่มเติมฟังก์ชันการรักษาและรับความเสียหาย
    private void Jump(bool isJump)
    {
        if (isJump && _isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
        _isJump = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
    void HandleFootstepSound()
    {
        // ถือว่าเคลื่อนไหว ถ้า input มีความยาวพอ
        bool movingNow = _inputDirection.sqrMagnitude > 0.01f;

        if (movingNow && !_isMovingForSound)
        {
            // เพิ่งเริ่มเดิน
            if (SoundManager.Instance != null)
                SoundManager.Instance.StartFootstepLoop();
        }
        else if (!movingNow && _isMovingForSound)
        {
            // เพิ่งหยุดเดิน
            if (SoundManager.Instance != null)
                SoundManager.Instance.StopFootstepLoop();
        }

        _isMovingForSound = movingNow;
    }
    // ---------- เล่นเสียงตอนโดนดาเมจ ----------
    public override void TakeDamage(int amount)
    {
        // ให้ Character จัดการลดเลือด + เช็คตายก่อน
        base.TakeDamage(amount);

        // ถ้ายังไม่ตายค่อยเล่นเสียง
        if (health > 0 && SoundManager.Instance != null)
        {
            if (Time.time - lastHitSfxTime > hitSfxCooldown)
            {
                SoundManager.Instance.PlayPlayerHit();
                lastHitSfxTime = Time.time;
            }
        }
        // ถ้าอยากให้ตอนตายก็ยังเล่นเสียงนี้อยู่ด้วย ก็ไม่ต้องเช็ค health > 0
    }
}
