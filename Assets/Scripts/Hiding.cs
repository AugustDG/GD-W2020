using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class SpriteRendererEvent : UnityEvent<SpriteRenderer> { }

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

public class Hiding : MonoBehaviour
{
    public SpriteRendererEvent hide = new SpriteRendererEvent();
    public BoolEvent show = new BoolEvent();

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            hide.Invoke(_spriteRenderer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            show.Invoke(false);
        }
    }
}