using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public float speed = 50f;
    private int keyCount = 0;

    [SerializeField] public GameObject uiHolder;
    private UI uiScript;

    private void Start()
    {
        uiScript = FindObjectOfType<UI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = Vector3.zero;
        rotation += Vector3.right;

        gameObject.transform.Rotate(rotation * speed * Time.deltaTime);

    }

    // player collects key
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("key collected!");
            uiScript.increaseKeyCount();
            keyCount++;
            Destroy(gameObject);
        }
    }

    public int getKeyCount()
    {
        return keyCount;
    }
}
