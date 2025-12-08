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
        [SerializeField]
        private float _jumpBufferWindow = 0.1f;
        [SerializeField]
        private float _jumpPower = 5.0f;
        [SerializeField]
        private float _movementDrag = 4.0f;
        [SerializeField]
        private float _airDrag = 0.0f;

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
        
        MovingPlatform currentPlatform;
        private bool isOnPlatform;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();

            rigidbody.freezeRotation = true;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            UpdateCamera();
            UpdateInputs();
        }

        private bool PerformGroundCheck(out RaycastHit hit)
        {
            Vector3 groundCheckPosition = new Vector3(0, -_groundRayLength, 0) + transform.position;

            return Physics.SphereCast(transform.position, _groundCheckRadius, Vector3.down, out hit, _groundRayLength, _groundLayer);
        }

        private void EnterPlatform(MovingPlatform platform)
        {
            // Set current platform
            currentPlatform = platform;
            isOnPlatform = true;
            
            // Local position on the real boat
            Vector3 localPos = platform.transform.InverseTransformPoint(transform.position);
            
            // Equivalent position on static boat
            Vector3 staticWorldPos = platform.StaticClone.TransformPoint(localPos);
            
            // Set position of rb to static world pos
            rigidbody.position = staticWorldPos;
            
            // Subtract boat velocity to enter boat's reference frame
            Vector3 platformVelocity = platform.Rb.GetPointVelocity(transform.position);
            rigidbody.linearVelocity -= platformVelocity;
        }
        
        private void ExitPlatform(MovingPlatform platform)
        {
            // Pos on the static boat
            Vector3 localPos = platform.StaticClone.InverseTransformPoint(rigidbody.position);
            // Equivalent position on the real boat
            Vector3 realWorldPos = currentPlatform.transform.TransformPoint(localPos);
            // Set position of rb to real boat pos
            rigidbody.position = realWorldPos;
            
            // Convert back to world reference frame
            Vector3 platformVelocity = currentPlatform.Rb.GetPointVelocity(realWorldPos);
            rigidbody.linearVelocity += platformVelocity;
            
            // unset current platform
            currentPlatform = null;
            isOnPlatform = false;
        }

        private void SyncVisual()
        {
            if (isOnPlatform && currentPlatform != null)
            {
                // Where is Rb relative to static boat?
                // Where should visual appear on real boat?
            }
            else
            {
                _playerMesh.position = rigidbody.position;
            }
            
            _playerMesh.rotation = orientation;
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
            bool grounded = PerformGroundCheck(out RaycastHit hit); // update ground check first before jump
            if (grounded)
            {
                var platform = hit.collider.GetComponent<MovingPlatform>();
                if (platform != null && !isOnPlatform)
                    EnterPlatform(platform);
                else if (platform == null && isOnPlatform)
                    ExitPlatform(platform);
            } else if (isOnPlatform)
            {
                // ExitPlatform(platform);
            }
            
            if (jumpQueued && grounded) Jump();

            isGrounded = grounded; // we set this after so that we dont have any weird bugs with grounding for a frame. order matters
            
            SyncVisual();
        }

        private void ApplyMovementForces()
        {
            if (isGrounded) HandleGroundMovement();
            else HandleAirMovement();

            // fake gravity
            rigidbody.AddForce(Vector3.down * _gravity, ForceMode.Acceleration);
        }

        private void HandleGroundMovement()
        {
            // calculate movement direction
            Vector3 baseDirection = new Vector3(input.x, 0, input.z).normalized;
            Vector3 moveDirection = orientation * baseDirection;

            // apply force in direction of input (with respect to orientation)
            rigidbody.linearDamping = _movementDrag;
            rigidbody.AddForce(moveDirection * _baseMovementSpeed, ForceMode.Force);
        }

        private void HandleAirMovement()
        {
            rigidbody.linearDamping = _airDrag;
        }

        private void Jump()
        {
            jumpQueued = false;

            // jump timestamp must be within window
            if (Time.time - jumpPressedTimestamp > _jumpBufferWindow) return;

            // cancel out y velocity, then apply impulse
            rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);
            rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse); // apply jump force
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 groundCheckPos = new Vector3(0, -_groundRayLength, 0) + transform.position;
            Gizmos.color = Color.blue;

            if(isGrounded) Gizmos.color = Color.green;
            else Gizmos.color = Color.red;
            
            Gizmos.DrawSphere(groundCheckPos, _groundCheckRadius);
        }
    }
}
