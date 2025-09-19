using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    Transform player;
    Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale; 
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player) return;

        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        float sign = player.position.x < transform.position.x ? -1f : 1f;
        transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * sign, baseScale.y, baseScale.z);
    }
}
