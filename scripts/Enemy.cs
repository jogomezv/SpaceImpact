using Godot;
using System;
using System.IO;

public partial class Enemy : Area2D
{

	protected float _speed = 300;
	private AnimatedSprite2D _animatedSprite;
	[Signal]
	public delegate void EnemyShootEventHandler(Vector2 position, Vector2 direction);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		OnShootingTimerTimeout();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// var sinValue = (float)Math.Sin(Position.X/100) / 2;
		// var direction = new Vector2(-1, sinValue).Normalized();

		// Position += direction * _speed * (float)delta;

		// var follower = GetNode<PathFollow2D>("PathFollow2D");
		// follower.Progress += _speed * (float)delta;

		if (Position.X < -20)
			QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		var sinValue = (float)Math.Sin(Position.X / 100) / 2;
		var direction = new Vector2(-1, sinValue).Normalized();

		Position += direction * _speed * (float)delta;
	}

	private void OnCrossEndMap(Area2D body)
	{
		if (body == this)
			QueueFree();
	}

	private void OnAreaEntered(Area2D body)
	{
		if (body is Projectile)
		{
			_speed = 0;
			_animatedSprite.Play("explode");

			var collition = GetNode<CollisionShape2D>("CollisionShape2D");
			collition.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
		}
	}

	private void onAnimatedAprite2DAnimationFinished()
	{
		if (_animatedSprite.Animation == "explode")
			QueueFree();
	}

	private void OnShootingTimerTimeout()
	{
		var position = GetNode<Marker2D>("ShootingPoint");
		EmitSignal(SignalName.EnemyShoot, position.GlobalPosition, Vector2.Left);
	}
}
