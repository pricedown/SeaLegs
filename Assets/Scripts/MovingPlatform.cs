using UnityEngine;
using UnityEngine.Serialization;

namespace SeaLegs
{
    public class MovingPlatform : MonoBehaviour
    {
        [FormerlySerializedAs("_staticCounterpart")] [SerializeField] private Transform _staticClone;
        public Transform StaticClone => _staticClone;
        private Rigidbody _rb;
        public Rigidbody Rb => _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
    }
}