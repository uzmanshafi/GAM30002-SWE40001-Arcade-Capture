//https://www.baeldung.com/cs/circle-line-segment-collision-detection

float DistanceBetweenPoints(Vector2 A, Vector2 B) {
	Vector2 line = end - start;
	return line.magnitude;
}

float TriangleArea(Vector2 A, Vector2 B, Vector2 C)
{
	Vector2 AB = B - A;
	Vector2 AC = C - A;

	float cross_product = (AB.x * AC.y) - (AB.y * AC.x);
	return Math.abs(cross_product) / 2;
}

bool CircleLineIntersection(Vector2 start, Vector2 end, Vector2 centre, float radius)
{
	float triangle_area = TriangleArea(start, end, centre);

	float minimum_distance = 2 * triangle_area / DistanceBetweenPoints(start, end);

	return minimum_distance <= radius;
}

//Returns true if collision
bool SegmentCircleCollision(Vector2 start, Vector2 end, Vector2 centre, float radius)
{
	float minimum_distance = float.PositiveInfinity;
	float maximum_distance = Math.max(DistanceBetweenPoints(centre, start), DistanceBetweenPoints(centre, end));

	return true;
}