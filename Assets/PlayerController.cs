using UnityEngine;

namespace SeaLegs
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        // reparented to whatever you land on, not what is directly below you (raycast directly down)
        // whatever an object is parented to, it will act as though its part of that object (inherit all its movement and orientation)
        [Header("Movement Parameters")]
        [SerializeField]
        private float _baseMovementSpeed = 1.0f;
        [SerializeField]
        private float _gravity = -9.81f;

        [Header("Boat Parameters")]
        [SerializeField]
        private float _groundRayLength;
        [SerializeField]
        private float _groundCheckRadius;
        [SerializeField]
        private LayerMask _groundLayer;

        [Header("Camera Movement")]
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

        Rigidbody rigidbody;
        bool jumpQueued;
        bool isGrounded;
        float jumpPressedTimestamp;

        Quaternion orientation;
        Vector2 rotation = Vector2.zero;
        Vector3 input = Vector3.zero;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            rigidbody.freezeRotation = true;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false; // using custom gravity for more customization
        }

        private void Update()
        {
            UpdateCamera();
            UpdateInputs();
        }

        private void UpdateGroundCheck()
        {
            Vector3 groundCheckPosition = new Vector3(0, -_groundRayLength, 0) + transform.position;

            isGrounded = Physics.CheckSphere(groundCheckPosition, _groundCheckRadius, _groundLayer);
        }

        private void UpdateInputs()
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.z = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpQueued = true;
                jumpPressedTimestamp = Time.time; // record time stamp in which player pressed space
            }
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

            // when we jump, we jump relative to the boat
            UpdateGroundCheck(); // update ground check first before jump
            if (jumpQueued) Jump();
        }

        private void ApplyMovementForces()
        {

        }

        private void Jump()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Vector3 groundCheckPos = new Vector3(0, -_groundRayLength, 0) + transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundCheckPos, _groundCheckRadius);
        }
    }
}
