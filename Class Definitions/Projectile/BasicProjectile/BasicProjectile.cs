class BasicProjectile : Projectile {
	public void move(float delta) {
		this.position.x += delta * this.speed * this.direction.x;
		this.position.y += delta * this.speed * this.direction.y;
	}
}