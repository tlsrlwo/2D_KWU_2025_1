using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("플레이어 이동값")]
    public float            maxJumpForce;                                   // 최대 점프 힘
    public float            minJumpForce;
    private float           maxClickTime = 2f;                              // 최대 클릭 시간
    private float           horizontalJumpForceMultiplier = 0.5f;           // 수평 힘 (수직 힘 * 수평 힘0.5)
    private float           clickStartTime;                                 // 클릭 시간(Time.time)
    
    [Header("점프플레이트")]
    public float            jumpPlateForceMultiplier;                // 점프 플레이트 점프 힘 배수
    public float            jumpPlateStrongForceMultiplier;          // 점프 플레이트(강한거) 힘 배수

    [Header("대쉬")]
    public float            dashForce;
    public float            currentGravityScale;
    public float            dashingTime;                                    // gravityScale 을 0 으로 설정하면 영원히 0이기 때문에 시간을 정해줌
        
    [Header("점프상호작용")]                                                 // 추가 점프 상호작용 오브젝트
    public float            horizontalJumpInteractForce;
    public float            verticalJumpInteractForce;


    private bool            isCharging;
    private bool            isGrounded;
    private bool            isJumping;
    private bool            hasDashed;
    private bool            canAirJump;

    [Header("그래플링")]
    [SerializeField] private float grappleInputBufferTime = 0.2f;                            // 그래플 실행 후 유예 시간 ; 0.2초 (60프레임 기준 12프레임)
    [SerializeField] private float lastGrappleInputTime = -1f;                               // Time.time 이용 위함. 초기값 없음 설정(-1)
    
    [Header("컴포넌트")]
    [SerializeField] private Rigidbody2D     rb;                                             // 플레이어 rigidbody
    [SerializeField] private TrailRenderer   tr;                                             // 대쉬           
    [SerializeField] private GrapplingSystem grapplingSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<TrailRenderer>();
        grapplingSystem = GetComponent<GrapplingSystem>();       
        currentGravityScale = rb.gravityScale;

        // 데이터 파싱을 위한 부분
        // 플레이어
        if (JumpConfigLoad.configDic.TryGetValue("PlayerMin", out var minConfig))
            minJumpForce = minConfig.ForceValue;
        if (JumpConfigLoad.configDic.TryGetValue("PlayerMax", out var maxConfig))
            maxJumpForce = maxConfig.ForceValue;  

        // 점프 발판
        if (JumpConfigLoad.configDic.TryGetValue("JumpPlate", out var plateConfig))
            jumpPlateForceMultiplier = plateConfig.Multiplier;

        if(JumpConfigLoad.configDic.TryGetValue("JumpPlateStrong", out var strongPlateConfig))
            jumpPlateStrongForceMultiplier = strongPlateConfig.Multiplier;

        // 공중 점프 상호작용
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
            lastGrappleInputTime = -1f;                                     // 다시 인풋 시간 초기화
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            grapplingSystem.StopGrapple();
        }

        // 대쉬
        if (!isGrounded && isJumping && !hasDashed && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Dash());
        }

        grapplingSystem.UpdateGrappleLine();        
    }


    // 이동
    #region Move
    private void Move()
    {
        // 땅에서 (기본 점프)
        if (Input.GetMouseButtonDown(0) && isGrounded)                                      // 땅에서 점프준비. 점프포스 충전 시작
        {
            clickStartTime = Time.time;
            isCharging = true;
        }
        // 공중에서 점프 중 마우스 클릭 (점프 추가 오브젝트)
        else if(Input.GetMouseButtonDown(0) && !isGrounded && canAirJump)
        {          
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(horizontalJumpInteractForce, verticalJumpInteractForce));

            isJumping = true;
            canAirJump = false;
        }


        if (Input.GetMouseButtonUp(0) && isCharging)                                        
        {
            float clickDuration = Mathf.Min(Time.time - clickStartTime, maxClickTime);      // Math.Min(a,b) a,b 중 더 작은 값
            float normalizedPower = clickDuration / maxClickTime;                           // jumpForce 에 사용
            
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, normalizedPower);

            float horizontalJumpForce = jumpForce * horizontalJumpForceMultiplier;
            float verticalJumpForce = jumpForce;

            rb.AddForce(new Vector2(horizontalJumpForce, verticalJumpForce));

            isCharging = false;
            isGrounded = false;
            isJumping = true;
        }
    }

    // 대쉬    
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
    // 그래플링     
    #region Grappling
    private void TryGrapple()
    {
        Vector2 direction = new Vector2(1,1).normalized;                                // 기본 그래플링 훅 발사 위치 (대각선 위)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grapplingRange, grappableLayer);

        if(hit.collider != null)                                                        // 목적지가 정해짐
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
        else                                                                            // 목적지가 없음
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

        float duration = 0.5f;          // 줄이 날라가는 시간
        float elapsed = 0f;             // 경과

        while (elapsed < duration)      // 0.2초 동안 줄이 자연스럽게 날라가게 하기 위한 부분
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
            float jumpForce = maxJumpForce * jumpPlateForceMultiplier;                  // 최대점프값에 발판값으로 증가
            rb.AddForce(Vector2.up * jumpForce);                                        // 새로운 jumpForce변수값 적용
            isGrounded = false;                                                         // 점프 후 땅에서 떨어짐
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
            isGrounded = true;                              // 바닥에 닿아있으면 isGrounded
            isJumping = false;
            hasDashed = false;

            // 감속 적용
            rb.velocity = new Vector2(rb.velocity.x * 0.2f, rb.velocity.y);
        }
    }
}