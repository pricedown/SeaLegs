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
        private Transform _cameraHolder;
        [SerializeField]
        private Camera _playerCamera;

        [SerializeField]
        private float _sensitivity = 0.2f;
        [SerializeField]
        private float _yRotConstraint = 85.0f;

        Vector2 rotation = Vector2.zero;

        private void Update()
        {
            UpdateCamera();
        }

        /// <summary>
        /// Updates the rotation of the camera
        /// </summary>
        private void UpdateCamera()
        {
            rotation.x += Input.GetAxis("Mouse X") * _sensitivity;
            rotation.y += Input.GetAxis("Mouse Y") * _sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -_yRotConstraint, _yRotConstraint);

            Quaternion angleX = Quaternion.AngleAxis(rotation.x, Vector3.up);
            Quaternion angleY = Quaternion.AngleAxis(rotation.y, Vector3.left);

            _cameraHolder.localRotation = angleX * angleY;
        }

        private void FixedUpdate()
        {
            
        }
    }
}
