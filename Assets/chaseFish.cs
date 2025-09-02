using UnityEngine;
using UnityEngine.AI;

public class CatChaseFish : MonoBehaviour
{
    public float chaseDistance = 10f;   // �ִ� �ν� �Ÿ�
    public string fishTag = "Fish";     // ���� �±�
    public float updateInterval = 0.5f; // ��ǥ ���� �ֱ�

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

            // chaseDistance �̳��� ���� ����
            if (dist <= chaseDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
            }
            // chaseDistance���� �ָ� �ƿ� �� ��
            else
            {
                agent.isStopped = true;
            }
        }
        else
        {
            agent.isStopped = true; // ���� ������ ����
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

            // chaseDistance �ȿ� �ִ� ������ ���
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
            Destroy(collision.gameObject); // ���� ����
            currentTarget = FindNearestFish(); // �ٷ� ���� Ÿ�� ����
            agent.isStopped = currentTarget == null; // ������ ����
        }
    }
}

