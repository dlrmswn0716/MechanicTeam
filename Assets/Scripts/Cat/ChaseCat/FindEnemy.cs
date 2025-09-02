using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemy : MonoBehaviour
{
    // 원뿔 거리
    [SerializeField] private float detectionDistance = 10f;
    //플레이어 태그를 찾음
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rayInterval = 1f;
    //원뿔 범위
    [SerializeField] private float forwardAngle = 15f;
    [SerializeField] private bool showArea = true;

    private List<Transform> detectedPlayers = new List<Transform>();
    private Coroutine detectionCoroutine;

    //코루틴 순환
    void Start()
    {
        detectionCoroutine = StartCoroutine(DetectionLoop());
    }

    IEnumerator DetectionLoop()
    {
        while (true)
        {
            DetectAllObjectsInTriangle();
            yield return new WaitForSeconds(rayInterval);
        }
    }

    //구 범위 -> 삼각형 범위 -> 플레이어 여부 확인
    void DetectAllObjectsInTriangle()
    {
        detectedPlayers.Clear();

        // 1단계: 범위 내 모든 콜라이더 찾기 (구 범위의 스캔)
        Collider[] allColliders = Physics.OverlapSphere(transform.position, detectionDistance);

        foreach (var collider in allColliders)
        {
            // 2단계: 플레이어 태그 체크
            if (!collider.CompareTag(playerTag)) continue;

            // 3단계: 삼각형 범위 내에 있는지 체크
            if (IsInForwardRange(collider.transform.position))
            {
                // 4단계: 처음 충돌한 레이캐스트만 검사
                if (HasClearLineOfSight(collider.transform.position))
                {
                    detectedPlayers.Add(collider.transform);
                    OnPlayerDetected(collider.transform);
                }
            }
        }

        if (detectedPlayers.Count > 0)
            Debug.Log($"삼각형 범위 전체 검사 - 감지된 플레이어: {detectedPlayers[0].name}");
        else
            Debug.Log("플레이어를 찾지 못함.");
    }

    bool IsInForwardRange(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 변경된 부분: coneAngle * 0.5f → forwardAngle (또는 더 작은 각도)
        return angleToTarget <= forwardAngle && distanceToTarget <= detectionDistance;
    }

    bool HasClearLineOfSight(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget))
        {
            // 타겟이 맞는지 확인 (장애물이 아닌)
            return hit.collider.CompareTag(playerTag);
        }

        return true; // 아무것도 막지 않음
    }

    private void OnPlayerDetected(Transform player)
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"플레이어 발견: {player.name}, 거리: {distance:F2}m");
    }

    //TO-DO : 삼각형 시각화 코드 (완성 시 삭제)
    private void OnDrawGizmos()
    {
        if (!showArea) return;

        // 삼각형 범위 시각화
        float halfAngle = forwardAngle * 0.5f;
        Vector3 leftBoundary = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward * detectionDistance;
        Vector3 rightBoundary = Quaternion.AngleAxis(halfAngle, Vector3.up) * transform.forward * detectionDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position + leftBoundary, transform.position + rightBoundary);

        // 감지된 플레이어들 표시
        Gizmos.color = Color.red;
        foreach (var player in detectedPlayers)
        {
            if (player != null)
            {
                Gizmos.DrawLine(transform.position, player.position);
                Gizmos.DrawWireSphere(player.position, 0.5f);
            }
        }
    }
}
