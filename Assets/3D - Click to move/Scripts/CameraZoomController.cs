using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseInputAction;


    [SerializeField]
    private float maxHeight = 35f;

    [SerializeField]
    private float minHeight = 15f;

    [SerializeField]
    private float zoomSpeed = 10f;

    [SerializeField]
    private float zoomSteper = 12f;

    private void OnEnable()
    {
        mouseInputAction.Enable();
        mouseInputAction.performed += Zoom;
    }

    private void OnDisable()
    {
        mouseInputAction.performed -= Zoom;
        mouseInputAction.Disable();
    }

    private void Zoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();

        var currentPosition = transform.position;

        var targetHeight = scrollValue > 0 ? currentPosition.y - zoomSteper : currentPosition.y + zoomSteper;


        var direction = new Vector3(currentPosition.x, targetHeight, currentPosition.z);

        transform.position = Vector3.MoveTowards(currentPosition, direction, zoomSpeed * Time.deltaTime);

    }




}
