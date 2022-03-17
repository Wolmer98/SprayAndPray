using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text m_text;
    [SerializeField] private float m_lifetime = 2.0f;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = FloatingTextManager.Instance.MyCamera;
        Destroy(gameObject, m_lifetime);
    }

    public void Setup(string text)
    {
        m_text.text = text;
    }
}
