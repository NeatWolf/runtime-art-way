﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitCalculator {

	public List<Vector2> Calculate(List<Vector2> cloud, float step){
		cloud = EqualDistanceUtil.Prepare(cloud, step);
		var result = FindCircuit(cloud, step*1.1f);
		return result;
	}

    private List<Vector2> FindCircuit(List<Vector2> cloud, float radius) {
		var result = new List<Vector2>();

		var points = cloud.Select(x => new Point(x)).ToList();
		points[0].Enabled = false;
		result.Add(points[0].Position);
		points[1].Enabled = false;
		result.Add(points[1].Position);

		Vector2 previous = result[0];
		Vector2 current = result[1];
		Vector2 next = Vector2.zero;
		while (FindNext(points, previous, current, radius, out next)){
			result.Add(next);
			previous = current;
			current = next;
		}

		return result;
    }

	private bool FindNext(List<Point> points, Vector2 previous, Vector2 current, float radius, out Vector2 result){
		var prevDirection = current - previous;

		var candidates = from p in points
					where p.Position != previous && p.Position != current
					where (p.Position - current).magnitude < radius
					let direction = p.Position - current
					let angle = Vector2.SignedAngle(prevDirection, direction)
					orderby angle ascending
					select new {point = p, angle = angle};

		Debug.LogFormat("From previous: {0}", prevDirection);
		Debug.LogFormat("\nPrevious: {0}, Current: {1}", previous, current);
		candidates.ToList().ForEach(x => Debug.LogFormat("Position: {0}, angle: {1}, Enabled: {2}", x.point.Position, x.angle, x.point.Enabled));

		var next = candidates.First().point;
		if (next.Enabled) {
			result = next.Position;
			next.Enabled = false;
			return true;
		}
		else {
			result = Vector2.zero;
			return false;
		}
	}

	private class Point {
		public Vector2 Position { get; set; }
		public bool Enabled { get; set; }

		public Point(Vector2 position){
			Position = position;
			Enabled = true;
		}

	}
}
