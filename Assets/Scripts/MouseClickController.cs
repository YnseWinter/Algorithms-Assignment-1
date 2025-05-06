using UnityEngine;
using UnityEngine.Events;

public class MouseClickController : MonoBehaviour
{
    public Vector3 clickPosition;

    public UnityEvent<Vector3> OnClick;
    
    void Update() { 
        // Get the mouse click position in world space 
        if (Input.GetMouseButtonDown(0)) { 
            Ray mouseRay = Camera.main.ScreenPointToRay( Input.mousePosition ); 
            if (Physics.Raycast( mouseRay, out RaycastHit hitInfo )) { 
                Vector3 clickWorldPosition = hitInfo.point; 
                Debug.Log(clickWorldPosition);

                // Store the click position here
                clickPosition = clickWorldPosition;

                // Trigger an unity event to notify other scripts about the click here
                OnClick.Invoke(clickPosition);
            } 
        }

        // Add visual debugging here
        Debug.DrawLine(transform.position, clickPosition, Color.yellow);
        DebugExtension.DebugWireSphere(clickPosition, Color.blue);
    } 

}
