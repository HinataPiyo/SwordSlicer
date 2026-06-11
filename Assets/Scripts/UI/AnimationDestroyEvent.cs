using UnityEngine;

public class AnimationDestroyEvent : MonoBehaviour
{
    [SerializeField] Transform target;
    public void DestroyObject()
    {
        Destroy(target.gameObject);
    }
}