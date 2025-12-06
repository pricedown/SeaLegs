using UnityEngine;

namespace SeaLegs
{
    public class ShipController : MonoBehaviour
    { 
        public Rigidbody shipRb;
        public Vector3 velocity = new Vector3(1f, 0f, 0f);
    
        void Update()
        {
            shipRb.AddForce(velocity * Time.deltaTime, ForceMode.Acceleration);
        }
    }
}
