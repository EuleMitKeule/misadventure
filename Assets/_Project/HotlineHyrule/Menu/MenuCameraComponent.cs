using UnityEngine;

namespace HotlineHyrule.Menu
{
    public class MenuCameraComponent : MonoBehaviour
    {
        [SerializeField] GameObject waypointObject;

        int currentIndex;
        float speed;

        Vector3[] waypoints;

        float z;

        void Awake()
        {
            currentIndex = 0;
            speed = 1f;

            waypoints = new Vector3[waypointObject.transform.childCount];

            for (var i = 0; i < waypoints.Length; ++i)
                waypoints[i] = waypointObject.transform.GetChild(i).position;

            transform.position = new Vector3(waypoints[0].x, waypoints[0].y, -10);
        }

        void Update()
        {
            Vector2 target = waypoints[currentIndex];
            Vector2 position = transform.position;

            Vector2 direction = target - position;
            direction.Normalize();

            Vector2 moved = direction * speed * Time.deltaTime;

            transform.position += new Vector3(moved.x, moved.y, 0);

            Vector2 newPosition = transform.position;

            float distance = Vector2.Distance(newPosition, target);

            if (distance < 0.01f)
                if (currentIndex < waypoints.Length - 1)
                    ++currentIndex;
                else
                    currentIndex = 0;
        }
    }
}
