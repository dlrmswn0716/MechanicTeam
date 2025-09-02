using UnityEngine;
using UnityEngine.AI;

public class CatChaseFish : MonoBehaviour
{
    public float chaseDistance = 10f;   // 최대 인식 거리
    public string fishTag = "Fish";     // 생선 태그
    public float updateInterval = 0.5f; // 목표 갱신 주기

    private NavMeshAgent agent;
    private float timer = 0f;
    private GameObject currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

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

            // chaseDistance 이내일 때만 추적
            if (dist <= chaseDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
            }
            // chaseDistance보다 멀면 아예 안 감
            else
            {
                agent.isStopped = true;
            }
        }
        else
        {
            agent.isStopped = true; // 생선 없으면 멈춤
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

            // chaseDistance 안에 있는 생선만 고려
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
            Destroy(collision.gameObject); // 생선 삭제
            currentTarget = FindNearestFish(); // 바로 다음 타겟 갱신
            agent.isStopped = currentTarget == null; // 없으면 멈춤
        }
    }
}

