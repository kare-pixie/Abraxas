using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TargetScanner : MonoBehaviour
{

    public PlayerController player;           // 플레이어의 위치를 받아올 변수
    public float detectionRange = 10f; // 몬스터가 플레이어를 인식할 범위
    public float moveSpeed = 3.5f;     // 몬스터 이동 속도

    private NavMeshAgent agent;        // 몬스터의 NavMeshAgent
    private bool isChasing = false;    // 몬스터가 플레이어를 추적 중인지 확인하는 변수

    [Header("Attack Settings")] 
    public float attackRange = 1f;          // 공격 가능 범위
    public float minAttackInterval = 1.5f;    // 최소 공격 간격
    public float maxAttackInterval = 3f;    // 최대 공격 간격
    public int damage = 10;                 // 공격 데미지

    private bool isAttacking = false;       // 공격 상태

    private Animator animator;

    void Start()
    {
        TryGetComponent(out animator);
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();
        agent.speed = moveSpeed;      // 몬스터의 이동 속도 설정
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= detectionRange)
        {
            // 플레이어가 감지 범위 내에 있을 때
            ChasePlayer();

            if (distance <= attackRange && !isAttacking)
            {
                // 공격 범위 내에 있고 공격 중이 아닐 때
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // 플레이어가 감지 범위 밖에 있을 때
            StopChasingPlayer();
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        float sec = 0.4f;

        while (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            // 플레이어를 향해 회전
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // 공격 애니메이션 재생 (애니메이터가 있는 경우)
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsTag("BlockMovement")) //
                break;

            if (animator != null)
            {
                animator.SetTrigger("Punch");
                sec = 0.4f;
                //switch (Random.Range(0, 2))
                //{
                //    case 0:
                //        animator.SetTrigger("Punch");
                //        sec = 0.4f;
                //        break;
                //    case 1:
                //        animator.SetTrigger("Swiping");
                //        sec = 1.5f;
                //        break;
                //}
            }

            // 데미지 적용 (예: 플레이어 스크립트에 데미지를 입히는 메서드 호출)
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
            {
                status.TakeDamage(damage, sec);
            }

            // 로그 출력 (디버깅 용도)
            Debug.Log("몬스터가 플레이어를 공격했습니다!");

            // 랜덤 간격 대기
            float attackInterval = Random.Range(minAttackInterval, maxAttackInterval);
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
    }
    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void ChasePlayer()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("Die")) return;

        isChasing = true;
        agent.isStopped = false;       // NavMeshAgent가 이동할 수 있도록 설정
        agent.SetDestination(player.transform.position); // 플레이어의 위치로 이동
    }

    private void StopChasingPlayer()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.isStopped = true;    // 추적 중지
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Gizmos의 색상 설정 (예: 빨간색)
        Gizmos.color = Color.red;

        // 몬스터의 감지 범위를 구체로 그리기
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

#endif

}
