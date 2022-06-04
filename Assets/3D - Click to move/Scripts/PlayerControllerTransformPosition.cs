using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTransformPosition : MonoBehaviour
{
    private Camera mainCamera;

    private CharacterController characterController;
    private Rigidbody rb;
    private int groundLayer;

    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 20f;

    [SerializeField]
    private float rotationSpeed = 10f;

    private Coroutine moveCoroutine;

    private Vector3 targetPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground");

        Physics.gravity = new Vector3(0, -100f, 0);
    }


    private void OnEnable()
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
    }

    private void Move(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider != null && hit.collider.gameObject.layer.CompareTo(groundLayer) == 0)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(PlayerMoveTowards(hit.point));

            targetPosition = hit.point;
        }
    }

    private IEnumerator PlayerMoveTowards(Vector3 target)
    {

        var offset = transform.position.y - target.y;
        target.y += offset;

        var cameraOffset = mainCamera.transform.position - transform.position;

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {

            var direction = target - transform.position;

            var lookRotation = Quaternion.LookRotation(direction.normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            Vector3 destination = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.deltaTime);

            // NOTE: MOVES THE PLAYER CHANGING TRANSFORM.POSITION DIRECTLY - IGNORES COLLISIONS           
            // transform.position = destination;


            // NOTE: MOVES THE PLAYER THROUGH CHARACTER CONTROLLER - CALC COLLISIONS 
            // var movement = direction.normalized * playerSpeed * Time.deltaTime;
            // characterController.Move(movement);

            // NOTE: MOVES THE PLAYER THROUGH RIGIDBODY;
            rb.velocity = direction.normalized * playerSpeed;

            mainCamera.transform.position = destination + cameraOffset;

            yield return null;

        }

        // STOP RB VELOCITY IMMEDIATELY WHEN REACH THE TARGET POSITION;
        rb.velocity = Vector3.zero;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1);
    }

}
