using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("�÷��̾� �̵���")]
    public float            maxJumpForce;                                   // �ִ� ���� ��
    public float            minJumpForce;
    private float           maxClickTime = 2f;                              // �ִ� Ŭ�� �ð�
    private float           horizontalJumpForceMultiplier = 0.5f;           // ���� �� (���� �� * ���� ��0.5)
    private float           clickStartTime;                                 // Ŭ�� �ð�(Time.time)
    
    [Header("�����÷���Ʈ")]
    public float            jumpPlateForceMultiplier;                // ���� �÷���Ʈ ���� �� ���
    public float            jumpPlateStrongForceMultiplier;          // ���� �÷���Ʈ(���Ѱ�) �� ���

    [Header("�뽬")]
    public float            dashForce;
    public float            currentGravityScale;
    public float            dashingTime;                                    // gravityScale �� 0 ���� �����ϸ� ������ 0�̱� ������ �ð��� ������
        
    [Header("������ȣ�ۿ�")]                                                 // �߰� ���� ��ȣ�ۿ� ������Ʈ
    public float            horizontalJumpInteractForce;
    public float            verticalJumpInteractForce;


    private bool            isCharging;
    private bool            isGrounded;
    private bool            isJumping;
    private bool            hasDashed;
    private bool            canAirJump;

    [Header("�׷��ø�")]
    [SerializeField] private float grappleInputBufferTime = 0.2f;                            // �׷��� ���� �� ���� �ð� ; 0.2�� (60������ ���� 12������)
    [SerializeField] private float lastGrappleInputTime = -1f;                               // Time.time �̿� ����. �ʱⰪ ���� ����(-1)
    
    [Header("������Ʈ")]
    [SerializeField] private Rigidbody2D     rb;                                             // �÷��̾� rigidbody
    [SerializeField] private TrailRenderer   tr;                                             // �뽬           
    [SerializeField] private GrapplingSystem grapplingSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        grapplingSystem = GetComponent<GrapplingSystem>();       
        currentGravityScale = rb.gravityScale;

        // ������ �Ľ��� ���� �κ�
        // �÷��̾�
        if (JumpConfigLoad.configDic.TryGetValue("PlayerMin", out var minConfig))
            minJumpForce = minConfig.ForceValue;
        if (JumpConfigLoad.configDic.TryGetValue("PlayerMax", out var maxConfig))
            maxJumpForce = maxConfig.ForceValue;  

        // ���� ����
        if (JumpConfigLoad.configDic.TryGetValue("JumpPlate", out var plateConfig))
            jumpPlateForceMultiplier = plateConfig.Multiplier;

        if(JumpConfigLoad.configDic.TryGetValue("JumpPlateStrong", out var strongPlateConfig))
            jumpPlateStrongForceMultiplier = strongPlateConfig.Multiplier;

        // ���� ���� ��ȣ�ۿ�
        if (JumpConfigLoad.configDic.TryGetValue("AirJump", out var airJumpConfig))
        {
            verticalJumpInteractForce = airJumpConfig.ForceValue;
            horizontalJumpInteractForce = airJumpConfig.ForceValue * airJumpConfig.Multiplier;
        }
    }

    void Update()
    {
        Move();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            lastGrappleInputTime = Time.time;        
        }

        if(!isGrounded && Time.time - lastGrappleInputTime <= grappleInputBufferTime)
        {
            grapplingSystem.TryGrapple();
            lastGrappleInputTime = -1f;                                     // �ٽ� ��ǲ �ð� �ʱ�ȭ
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            grapplingSystem.StopGrapple();
        }

        // �뽬
        if (!isGrounded && isJumping && !hasDashed && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Dash());
        }

        grapplingSystem.UpdateGrappleLine();        
    }


    // �̵�
    #region Move
    private void Move()
    {
        // ������ (�⺻ ����)
        if (Input.GetMouseButtonDown(0) && isGrounded)                                      // ������ �����غ�. �������� ���� ����
        {
            clickStartTime = Time.time;
            isCharging = true;
        }
        // ���߿��� ���� �� ���콺 Ŭ�� (���� �߰� ������Ʈ)
        else if(Input.GetMouseButtonDown(0) && !isGrounded && canAirJump)
        {          
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(horizontalJumpInteractForce, verticalJumpInteractForce));

            isJumping = true;
            canAirJump = false;
        }


        if (Input.GetMouseButtonUp(0) && isCharging)                                        
        {
            float clickDuration = Mathf.Min(Time.time - clickStartTime, maxClickTime);      // Math.Min(a,b) a,b �� �� ���� ��
            float normalizedPower = clickDuration / maxClickTime;                           // jumpForce �� ���
            
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, normalizedPower);

            float horizontalJumpForce = jumpForce * horizontalJumpForceMultiplier;
            float verticalJumpForce = jumpForce;

            rb.AddForce(new Vector2(horizontalJumpForce, verticalJumpForce));

            isCharging = false;
            isGrounded = false;
            isJumping = true;
        }
    }

    // �뽬    
    private IEnumerator Dash()
    {
        hasDashed = true;
        currentGravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
        tr.emitting = true;


        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = currentGravityScale;
    }
    #endregion

    /*
    // �׷��ø�     
    #region Grappling
    private void TryGrapple()
    {
        Vector2 direction = new Vector2(1,1).normalized;                                // �⺻ �׷��ø� �� �߻� ��ġ (�밢�� ��)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grapplingRange, grappableLayer);

        if(hit.collider != null)                                                        // �������� ������
        {
            grappleObjectInRange = true;

            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;

            grapplingTarget = hit.point;
            isGrappling = true;

            rb.AddForce((grapplingTarget - (Vector2)transform.position) * 20f, ForceMode2D.Impulse);

            lr.enabled = true;
            lr.positionCount = 2;
        }
        else                                                                            // �������� ����
        {
            grappleObjectInRange = false;
            isGrappling = false;
            StartCoroutine(SimulateFreeHook(direction)); 
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        rb.gravityScale = currentGravityScale;
        lr.enabled = false;
    }

    private IEnumerator SimulateFreeHook(Vector2 direction)
    {
        lr.enabled = true;
        lr.positionCount = 2;  

        float duration = 0.5f;          // ���� ���󰡴� �ð�
        float elapsed = 0f;             // ���

        while (elapsed < duration)      // 0.2�� ���� ���� �ڿ������� ���󰡰� �ϱ� ���� �κ�
        {
            elapsed += Time.deltaTime;
            float smoothGrapple = elapsed / duration;

            Vector2 dynamicStart = transform.position;
            Vector2 dynamicEnd = dynamicStart + direction * grapplingRange;

            Vector2 current = Vector2.Lerp(dynamicStart, dynamicEnd, smoothGrapple);
            lr.SetPosition(0, dynamicStart);
            lr.SetPosition(1, current);
            yield return null;
        }

        lr.enabled = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grapplingRange);
    }
    #endregion

    */




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("JumpPlate"))
        {
            float jumpForce = maxJumpForce * jumpPlateForceMultiplier;                  // �ִ��������� ���ǰ����� ����
            rb.AddForce(Vector2.up * jumpForce);                                        // ���ο� jumpForce������ ����
            isGrounded = false;                                                         // ���� �� ������ ������
            isJumping = true;
        }
        if (other.gameObject.CompareTag("JumpPlateStrong"))
        {
            float jumpForce = maxJumpForce * jumpPlateStrongForceMultiplier;
            rb.AddForce(Vector2.up * jumpForce);
            isGrounded = false;
            isJumping = true;
        }
        if (other.gameObject.CompareTag("AirJump"))
        {
            canAirJump = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("AirJump"))
        {
            canAirJump = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;                              // �ٴڿ� ��������� isGrounded
            isJumping = false;
            hasDashed = false;

            // ���� ����
            rb.velocity = new Vector2(rb.velocity.x * 0.2f, rb.velocity.y);
        }
    }
}