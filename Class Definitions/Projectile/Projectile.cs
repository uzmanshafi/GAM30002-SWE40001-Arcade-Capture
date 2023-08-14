public abstract class Projectile {
	private Vector2 position;
	private Vector2 direction;
	private float speed;

	public Projectile(Vector2 position, Vector2 direction, float speed) {
		this.position = position;
		this.direction = direction;
		this.speed = speed;
	}

	public abstract void move();
}