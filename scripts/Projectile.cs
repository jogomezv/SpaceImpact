using Godot;
using System;

public partial class Projectile : Area2D
{
	private float _speed = 600;
	public Vector2 Direction { get; set; } = new Vector2(1, 0);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.X < 20 || Position.X > 1300)
			QueueFree();
		// Position += new Vector2(_speed * (float)delta, 0);
		// Position += Direction * _speed * (float)delta;
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * _speed * (float)delta;
	}

    private void OnCrossEndMap(Area2D _body)
	{
		if (_body == this)
			QueueFree();
	}

	private void OnAreaEntered(Area2D body)
	{
		// if (body is Enemy)
			QueueFree();
	}
}
