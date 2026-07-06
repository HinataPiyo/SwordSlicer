using UnityEngine;

public class AnimationDestroyEvent : MonoBehaviour
{
    [SerializeField] Transform target;
    public event System.Action OnDestroyEvent;
    public void DestroyObject()
    {
        if(OnDestroyEvent != null)
        {
            OnDestroyEvent.Invoke();
            return;
        }

        Destroy(target.gameObject);
    }
}