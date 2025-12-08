using UnityEngine;

namespace SeaLegs
{
    public class PlayerController : MonoBehaviour
    {
        // reparented to whatever you land on, not what is directly below you (raycast directly down)
        // whatever an object is parented to, it will act as though its part of that object (inherit all its movement and orientation)
        [Header("Movement Parameters")]
        [SerializeField]
        private float _baseMovementSpeed = 1.0f;

        [Header("Boat Parameters")]
        [SerializeField]
        private float _rayLength;

        [Header("Camera Movement")]
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private Transform _cameraHolder;
        [SerializeField]
        private Camera _playerCamera;
        [SerializeField, Tooltip("The mesh that will represent the player, placed on the real ship")]
        private Transform _playerMesh;

        [SerializeField]
        private float _sensitivity = 0.2f;
        [SerializeField]
        private float _yRotConstraint = 85.0f;

        Quaternion orientation;
        Vector2 rotation = Vector2.zero;
        Vector3 input = Vector3.zero;

        private void Update()
        {
            UpdateCamera();
            UpdateInputs();
        }

        private void UpdateInputs()
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");
        }

        /// <summary>
        /// Updates the rotation of the camera
        /// </summary>
        private void UpdateCamera()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime;

            rotation.y += mouseX;
            rotation.x -= mouseY;
            rotation.x = Mathf.Clamp(rotation.x, -90.0f, 90.0f);

            orientation = Quaternion.Euler(0, rotation.y, 0);
            _cameraHolder.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        }

        private void FixedUpdate()
        {
            ApplyMovementForces();
        }

        private void ApplyMovementForces()
        {

        }
    }
}
