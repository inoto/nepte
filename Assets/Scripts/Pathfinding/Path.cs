using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {

	public readonly Vector2[] points;
	public int currentIndex;

	public Path(Vector2[] waypoints, Vector3 startPos, float turnDst, float stoppingDst) {
		points = waypoints;

		Vector2 previousPoint = V3ToV2 (startPos);
		for (int i = 0; i < points.Length; i++) {
			Vector2 currentPoint = V3ToV2 (points [i]);
			Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
		}

		float dstFromEndPoint = 0;
		for (int i = points.Length - 1; i > 0; i--) {
			dstFromEndPoint += Vector3.Distance (points [i], points [i - 1]);
		}
	}

	Vector2 V3ToV2(Vector3 v3) {
		return new Vector2 (v3.x, v3.y);
	}

	public void DrawWithGizmos() {

        Color newColor = Color.green;
        newColor.a = 0.5f;
        Gizmos.color = newColor;
		for (int i = 0; i < points.Length; i++)
		{
			if (i == points.Length)
			{
                Gizmos.DrawLine(points[i], points[points.Length]);
			}
			else
			{
				Gizmos.DrawLine(points[i], points[i+1]);
                Gizmos.DrawCube(points[i], Vector3.one * (Grid.nodeDiameter - 0.01f));
			}
		}

	}

}
