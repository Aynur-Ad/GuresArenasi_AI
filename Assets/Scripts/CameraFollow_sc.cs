using UnityEngine;

public class CameraFollow_sc : MonoBehaviour
{
    public Transform target;    // playerý koy, takip edecek
    public Vector3 offset;        
    public float followSpeed = 3f;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        Vector3 hedefPozisyon = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, hedefPozisyon, followSpeed * Time.deltaTime);

        // kamera playera dönsün
        transform.LookAt(target);
    }
}

