using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderRedirector : MonoBehaviour
{
    [SerializeField] 
    protected GameObject Target;
    
    public static T GetFinalTarget<T>(GameObject collidedObj) where T : MonoBehaviour
    {
        if (collidedObj == null)
            return default(T);

        var redirector = collidedObj.GetComponent<ColliderRedirector>();

        if (redirector != null)
        {
            return redirector.Target.GetComponent<T>();
        }

        return collidedObj.GetComponent<T>();
    }

    public static GameObject GetFinalTarget(GameObject collidedObj)
    {
        if (collidedObj == null)
            return null;

        var redirector = collidedObj.GetComponent<ColliderRedirector>();

        if (redirector != null)
        {
            return redirector.Target;
        }

        return collidedObj;
    }

    private void OnMouseEnter()
    {
        Target?.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseExit()
    {
        Target?.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseUp()
    {
        Target?.SendMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseDown()
    {
        Target?.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseOver()
    {
        Target?.SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseDrag()
    {
        Target?.SendMessage("OnMouseDrag", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseUpAsButton()
    {
        Target?.SendMessage("OnMouseUpAsButton", SendMessageOptions.DontRequireReceiver);
    }
}
