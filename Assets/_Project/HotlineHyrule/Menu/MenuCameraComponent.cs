using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraComponent : MonoBehaviour
{
    [SerializeField] GameObject waypointObject;

    private int currentIndex;
    private float speed;

    private Vector3[] waypoints;

    private float z;

    void Awake()
    {
        currentIndex = 0;
        speed = 1f;

        float z = transform.position.z;

        transform.position = new Vector3(0, 0, -10);

        waypoints = new Vector3[waypointObject.transform.childCount];
		
        for (int i = 0; i < waypoints.Length; ++i)
            waypoints[i] = waypointObject.transform.GetChild(i).position;

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
