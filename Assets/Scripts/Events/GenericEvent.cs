using UnityEngine;
using UnityEngine.Events;

public class GenericEvent : MonoBehaviour
{
    [SerializeField]
    private UnityEvent callbacks;

    [SerializeField]
    private string executionTag;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(executionTag))
            callbacks.Invoke();
    }
}
