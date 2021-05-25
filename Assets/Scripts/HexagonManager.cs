using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    public GameObject circle;
    private IEnumerable<KeyValuePair<Transform, float>> closest3Tranforms;

    public GameObject Circle { get => circle; set => circle = value; }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CalcClosest3Points(out closest3Tranforms);
            CreatePointObject(closest3Tranforms);
        }
    }

    private void CreatePointObject(IEnumerable<KeyValuePair<Transform, float>> closest3points)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var kvp in closest3points)
        {
            positions.Add(kvp.Key.position);
        }
        Vector3 center = GetCentroid(positions) - Vector3.forward;

        if (Circle.GetComponent<SpriteRenderer>().enabled == false)
        {
            Circle.GetComponent<SpriteRenderer>().enabled = true;
        }

        Circle.transform.position = center;
    }

    private void Rotate3Transform()
    {
        foreach (var item in closest3Tranforms)
        {
            item.Key.parent = Circle.transform;
        }

        Circle.transform.Rotate(0, 0, 15);

        foreach (var item in closest3Tranforms)
        {
            item.Key.parent = transform;
        }
    }

    private void CalcClosest3Points(out IEnumerable<KeyValuePair<Transform, float>> closest3points)
    {
        Dictionary<Transform, float> childDistance = new Dictionary<Transform, float>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i).transform;
            Vector2 childScrPos = Camera.main.WorldToScreenPoint(child.position);

            float distance = Vector2.Distance(childScrPos, Input.mousePosition);

            childDistance.Add(child, distance);
        }

        closest3points = childDistance.OrderBy(kvp => kvp.Value).Take(3);
    }

    public static Vector3 GetCentroid(List<Vector3> poly)
    {
        float accumulatedArea = 0.0f;
        float centerX = 0.0f;
        float centerY = 0.0f;

        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
            accumulatedArea += temp;
            centerX += (poly[i].x + poly[j].x) * temp;
            centerY += (poly[i].y + poly[j].y) * temp;
        }

        if (Math.Abs(accumulatedArea) < 1E-7f)
            return new Vector3();  // Avoid division by zero

        accumulatedArea *= 3f;
        return new Vector3(centerX / accumulatedArea, centerY / accumulatedArea);
    }
}