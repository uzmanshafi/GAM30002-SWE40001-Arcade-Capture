public abstract class Projectile {
	private Vector2 position;
	private Vector2 direction;
	private float speed;
	private float damage;

	public Projectile(Vector2 position, Vector2 direction, float speed, float damage) {
		this.position = position;
		this.direction = direction;
		this.speed = speed;
		this.damage = damage;
	}

	public abstract void move(float delta);
}