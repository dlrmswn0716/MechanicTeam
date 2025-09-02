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
    void Update()
    {
        timer += Time.deltaTime;
        if ( timer >= updateInterval)
        {
            currentTarget = FindNearestFish();
            timer = 0f;
        }

        if( currentTarget != null)
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
            if (dist < minDist && dist <= chaseDistance)
            {
                minDist = dist;
                nearest = fish;
            }
        }
        return nearest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(fishTag))
        {
            Destroy(collision.gameObject);
            currentTarget = null; // 새 타겟은 다음 Update에서 자동 탐색
        }
    }
}

