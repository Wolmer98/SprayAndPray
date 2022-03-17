using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    private static FloatingTextManager m_instance;
    public static FloatingTextManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<FloatingTextManager>();

            return m_instance;
        }
    }

    [SerializeField] private FloatingText m_floatingTextPrefab;

    public Camera MyCamera;

    private void Start()
    {
        MyCamera = Camera.main;
    }

    public void SpawnFloatingText(string message, Vector3 position, Transform parent)
    {
        var spawnedFloatingText = Instantiate(m_floatingTextPrefab, position, Quaternion.identity);
        if (parent != null)
            spawnedFloatingText.transform.SetParent(parent);
        spawnedFloatingText.Setup(message);
    }
}
