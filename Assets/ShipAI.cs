using UnityEngine;

public class ShipAI : MonoBehaviour
{ 
    public Rigidbody shipRb;
    public float speed = 1f;
    public Vector3 direction = new Vector3(1f, 0f, 0f);
    
    void Update()
    {
        shipRb.AddForce(direction * (speed * Time.deltaTime), ForceMode.Acceleration);
    }
}
