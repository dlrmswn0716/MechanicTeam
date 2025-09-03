using UnityEngine;
using UnityEngine.AI;

public class chaseFish : MonoBehaviour
{
    public float chaseDistance = 10f; // 최대 인식 거리
    public string fishTag = "Fish"; // 생선 태그
    public float updateInterval = 1f; // 목표 갱신 주기

    public float offsetY = 0.5f; // 타겟의 Y축 오프셋
    private GameObject currentTarget;
    private float timer = 0f;
    public float moveSpeed = 5f; // 이동 속도


    public LayerMask groundLayer;         // 바닥 레이어 지정
    public LayerMask obstacleLayer;     // 장애물 레이어 지정 
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            currentTarget = FindNearestFish();
            timer = 0f;
        }

        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (dist <= chaseDistance)
            {
                // 타겟 위치 (Y축 오프셋 적용)
                Vector3 targetPos = currentTarget.transform.position + new Vector3(0, offsetY, 0); ;

                // 이동 (속도 = moveSpeed)
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                // 바라보는 방향도 자연스럽게 회전
                Vector3 direction = (targetPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
                }

            }

        }
        // Raycast로 바닥 법선 구하기
        StickToGround(); // 항상 바닥에 붙도록 처리
    }

    GameObject FindNearestFish()
    {
        GameObject[] fishes = GameObject.FindGameObjectsWithTag(fishTag);
        GameObject nearest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject fish in fishes)
        {
            float dist = Vector3.Distance(currentPos, fish.transform.position);
            if (dist < minDist && dist <= chaseDistance && HasClearLineOfSight(fish))
            {
                minDist = dist;
                nearest = fish;
            }
        }
        return nearest;
    }

    bool HasClearLineOfSight(GameObject target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f; // 고양이 눈높이 정도
        Vector3 dir = (target.transform.position - origin).normalized;
        float dist = Vector3.Distance(origin, target.transform.position);

        // 장애물 체크 (Fish는 무시)
        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, obstacleLayer))
        {
            // 맞은 게 Fish면 시야 확보, 아니면 막힘
            if (!hit.collider.CompareTag(fishTag))
                return false;
        }
        return true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(fishTag))
        {
            Destroy(collision.gameObject);
            currentTarget = null; // 새 타겟은 다음 Update에서 자동 탐색
        }
    }

    void StickToGround()
    {

        Ray ray = new Ray(transform.position + Vector3.up * 3f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer))
        {
            // 위치 보정
            Vector3 pos = transform.position;
            pos.y = hit.point.y + offsetY + 0.5f;
            transform.position = pos;


            // 경사에 맞춰 회전 (앞 방향 유지)
            Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRot, 5f * Time.deltaTime);
        }
    }

}

