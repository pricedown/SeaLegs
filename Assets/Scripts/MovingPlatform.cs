using UnityEngine;

namespace SeaLegs
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform _staticClone;
        public Transform StaticClone => _staticClone;
    }
}
