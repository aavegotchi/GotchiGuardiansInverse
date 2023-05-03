using UnityEngine;
using UnityEngine.EventSystems;

public class MouseRaycaster : MonoBehaviour
{
    private LineRenderer line;

    void Start()
    {
        // Create a new line object and set its properties
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
    }

    void Update()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Skip drawing the line
            return;
        }

        // Cast a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a variable to store information about the object the ray hits
        RaycastHit hit;

        // Check if the ray hits a collider
        if (Physics.Raycast(ray, out hit))
        {
            // Update the line's positions to draw a line from the camera to the point where the ray hits the collider
            line.SetPosition(0, ray.origin);
            line.SetPosition(1, hit.point);
        }
        else
        {
            // Update the line's positions to draw a line from the camera to the point where the ray would have hit a collider if there was one within a distance of 1000 units from the camera
            line.SetPosition(0, ray.origin);
            line.SetPosition(1, ray.origin + ray.direction * 1000f);
        }
    }
}
