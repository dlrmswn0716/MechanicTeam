using UnityEngine;

public class MomCatLegMove : MonoBehaviour
{
    public float legsRotationUpLimit = 223.0f;
    public float legsRotationDownLimit = 144.0f;
    public float legsSpeed = 240.0f;
    public bool isActive = false;

    private bool isAdd = true;

    // Update is called once per frame
    void Update()
    {
        UpdateLegMovement();
    }

    void UpdateLegMovement()
    {
        if (isActive == false)
            return;

        float rotationZ = transform.eulerAngles.z;
        float addValue = legsSpeed * Time.deltaTime;
        rotationZ = isAdd ? rotationZ + addValue : rotationZ - addValue;

        if (rotationZ > legsRotationUpLimit)
        {
            isAdd = false;
            rotationZ = legsRotationUpLimit - addValue;
        }

        if (rotationZ < legsRotationDownLimit)
        {
            isAdd = true;
            rotationZ = legsRotationDownLimit + addValue;
        }

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, rotationZ);
    }
}
