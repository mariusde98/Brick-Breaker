using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class defines the UI for visualizing balls outside the player's current view.
// Arrows appear on the edge of the screen and point towards the current position
// of any ball that the player is currently not seeing in front of them.
public class BallIndicatorPanel : MonoBehaviour
{

    // Prefab for the arrow asset to show in the UI
    public GameObject arrowPrefab;

    private int arrowIndex = 0;
    private List<GameObject> arrowPool = new List<GameObject>();
    private Camera mainCamera;
    private RectTransform parentRect;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        parentRect = GetComponentInParent<RectTransform>();
    }

    void LateUpdate()
    {
        if (arrowPrefab == null)
        {
            return;
        }

        ResetPool();

        // Collect all balls in the scene
        List<BallController> balls = BallsHolderSingleton.Instance.balls;
        foreach (BallController ballObject in balls)
        {
            Vector3 worldPosition = ballObject.transform.position;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(ballObject.transform.position);

            if (screenPosition.z > 0 &&
                screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                // Ball is currently visible on screen - do nothing
            }
            else
            {
                if (screenPosition.z < 0)
                {
                    // Ball is behind us; invert the vector to make the arrow appear on the bottom
                    screenPosition *= -1;
                }

                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

                // Origin is currently in screen center - translate it to the bottom-left edge (easier calculations)
                screenPosition -= screenCenter;

                // Calculate the correct direction towards the ball in a circular motion
                float circleRadius = Mathf.Min(parentRect.rect.width, parentRect.rect.height) * 0.25f;
                screenPosition = screenPosition.normalized * circleRadius;
                float angle = Mathf.Atan2(screenPosition.y, screenPosition.x);

                // Attach a new arrow
                GameObject arrow = GetArrow();
                arrow.transform.localPosition = screenPosition;
                arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            }
        }

        CleanPool();
    }

    // Object pooling inspired by: https://www.youtube.com/watch?v=gAQpR1GN0Os
    private GameObject GetArrow()
    {
        GameObject arrow;

        if (arrowIndex < arrowPool.Count)
        {
            // Reuse existing arrow
            arrow = arrowPool[arrowIndex];
        }
        else
        {
            // Create a new one
            arrow = Instantiate(arrowPrefab);
            arrow.transform.SetParent(this.transform, false);
            arrowPool.Add(arrow);
        }

        arrowIndex++;
        return arrow;
    }

    private void ResetPool()
    {
        arrowIndex = 0;
    }

    private void CleanPool()
    {
        while (arrowPool.Count > arrowIndex)
        {
            GameObject obj = arrowPool[arrowPool.Count - 1];
            arrowPool.Remove(obj);
            Destroy(obj);
        }
    }
}
