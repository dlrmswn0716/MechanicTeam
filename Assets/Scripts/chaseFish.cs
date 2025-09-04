using UnityEngine;
using UnityEngine.AI;

public class chaseFish : MonoBehaviour
{
    public float chaseDistance = 10f; // �ִ� �ν� �Ÿ�
    public string fishTag = "Fish"; // ���� �±�
    public float updateInterval = 1f; // ��ǥ ���� �ֱ�

    public float offsetY = 0.5f; // Ÿ���� Y�� ������
    private GameObject currentTarget;
    private float timer = 0f;
    public float moveSpeed = 5f; // �̵� �ӵ�


    public LayerMask groundLayer;         // �ٴ� ���̾� ����
    public LayerMask obstacleLayer;     // ��ֹ� ���̾� ���� 
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
                // Ÿ�� ��ġ (Y�� ������ ����)
                Vector3 targetPos = currentTarget.transform.position + new Vector3(0, offsetY, 0); ;

                // �̵� (�ӵ� = moveSpeed)
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                // �ٶ󺸴� ���⵵ �ڿ������� ȸ��
                Vector3 direction = (targetPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
                }

            }

        }
        // Raycast�� �ٴ� ���� ���ϱ�
        StickToGround(); // �׻� �ٴڿ� �ٵ��� ó��
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
        Vector3 origin = transform.position + Vector3.up * 0.05f;  // ����� �� ��ġ
        Vector3 toTarget = target.transform.position - origin;
        float dist = toTarget.magnitude;
        if (dist <= 0.001f) return true; // ���� ��ġ��� �ٷ� true

        Vector3 dir = toTarget.normalized;

        // ��ֹ� ���̾ üũ
        if (Physics.Raycast(origin, dir, dist, obstacleLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(origin, dir * dist, Color.red, 0.1f);
            return false; // Fish���� ���� �濡 ��ֹ� ����
        }

        Debug.DrawRay(origin, dir * dist, Color.green, 0.1f);
        return true; // ���� ��ֹ� ����
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(fishTag))
        {
            collision.gameObject.GetComponent<InteractObj>().HideUI();
            Destroy(collision.gameObject);
            currentTarget = null; // �� Ÿ���� ���� Update���� �ڵ� Ž��
        }
    }

    void StickToGround()
    {

        Ray ray = new Ray(transform.position + Vector3.up * 3f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer))
        {
            // ��ġ ����
            Vector3 pos = transform.position;
            pos.y = hit.point.y + offsetY + 0.5f;
            transform.position = pos;


            // ��翡 ���� ȸ�� (�� ���� ����)
            Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRot, 5f * Time.deltaTime);
        }
    }

}

