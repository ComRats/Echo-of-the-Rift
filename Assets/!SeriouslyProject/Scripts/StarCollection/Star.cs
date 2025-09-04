using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private List<Star> stars;
    [SerializeField] private List<LineRenderer> lines;
    [SerializeField] private Material lineMaterial;
    private void Start()
    {
        // Создаём LineRenderer для каждой пары объектов
        for (int i = 0; i < stars.Count; i++)
        {
            for (int j = i + 1; j < stars.Count; j++)
            {
                GameObject lineObj = new GameObject("Line_" + i + "_" + j);
                lineObj.transform.parent = transform;
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();

                lr.material = lineMaterial;
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.positionCount = 2;

                lines.Add(lr);
            }
        }
    }

    private void Update()
    {
        int index = 0;
        for (int i = 0; i < stars.Count; i++)
        {
            for (int j = i + 1; j < stars.Count; j++)
            {
                if (stars[i] != null && stars[j] != null)
                {
                    lines[index].enabled = true;
                    lines[index].SetPosition(0, stars[i].transform.position);
                    lines[index].SetPosition(1, stars[j].transform.position);
                }
                else
                {
                    lines[index].enabled = false;
                }
                index++;
            }
        }
    }
}
