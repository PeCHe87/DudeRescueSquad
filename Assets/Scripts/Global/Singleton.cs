using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private readonly static object m_Lock = new object();
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));
                }
                return m_Instance;
            }
        }
    }
}