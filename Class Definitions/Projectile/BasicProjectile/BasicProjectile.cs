class BasicProjectile : Projectile {
	public void move() {
		this.position.x += this.speed * this.direction.x;
		this.position.y += this.speed * this.direction.y;
	}
}