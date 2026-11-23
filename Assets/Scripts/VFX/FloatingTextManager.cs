using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public GameObject Prefab;
    public int InitialSize = 20;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < InitialSize; i++)
        {
            if (Prefab != null)
            {
                var obj = Instantiate(Prefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : (Prefab != null ? Instantiate(Prefab, transform) : null);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    public enum TextColorType
    {
        DamageHP,      // US-053: Đỏ = Mất HP
        DamageShield,  // US-053: Xanh lơ = Mất Shield
        HealHP         // US-053: Xanh lá = Hồi HP
    }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// US-052 & US-053: Hiển thị Floating Text đổi màu theo loại tương tác
    /// </summary>
    public void ShowText(string content, Vector3 worldPos, TextColorType colorType)
    {
        Color targetColor = Color.white;
        switch (colorType)
        {
            case TextColorType.DamageHP:
                targetColor = Color.red;
                break;
            case TextColorType.DamageShield:
                targetColor = new Color(0.2f, 0.8f, 1f); // Xanh lơ
                break;
            case TextColorType.HealHP:
                targetColor = Color.green;
                break;
        }

        Debug.Log($"[FloatingText] Hiển thị '{content}' tại {worldPos} với màu: {colorType}");
    }
}
