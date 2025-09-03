using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FindEnemy : MonoBehaviour
{
    public Vector3 endPos;
    // 원뿔 거리
    [SerializeField] private float detectionDistance = 10f;
    //플레이어 태그를 찾음
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rayInterval = 1f;
    [SerializeField] private float delayInterval = 3f;
    //원뿔 범위
    [SerializeField] private float forwardAngle = 15f;
    [SerializeField] private bool showArea = false;

    public float duration = 0.25f;
    private Vector3 startPos;

    private List<Transform> detectedPlayers = new List<Transform>();
    private Coroutine detectionCoroutine;
    private GameObject Spot;

    //코루틴 순환
    void Start()
    {
        startPos = gameObject.transform.position;
        endPos = startPos - new Vector3(0, -0.576f, 0);
        detectionCoroutine = StartCoroutine(DetectionLoop());
        Spot = transform.GetChild(transform.childCount - 1).gameObject;
    }
    //3초 대기 후 1초동안 수색
    IEnumerator DetectionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayInterval);
            StartCoroutine(ChaseCatAnim(startPos,endPos));
            DetectAllObjectsInTriangle();
            showArea = true;
            Spot.SetActive(true);
            yield return new WaitForSeconds(rayInterval);
            StartCoroutine(ChaseCatAnim(endPos, startPos));
            showArea = false;
            Spot.SetActive(false);
        }
    }

    IEnumerator ChaseCatAnim(Vector3 startPos,Vector3 endPos)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 0~1 비율

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null; // 한 프레임 대기
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
                    //TO-DO감지 플레이어 로그 삭제시 삭제
                    detectedPlayers.Add(collider.transform);
                    Debug.Log($"플레이어 발견: {collider.transform.name}");
                }
            }
        }
    }

    //삼각형 범위 체크
    bool IsInForwardRange(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        return angleToTarget <= forwardAngle && distanceToTarget <= detectionDistance;
    }

    //막고 있는 오브젝트 체크
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
