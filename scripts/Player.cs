using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody2D
{
	private float _speed = 600;
	private bool _canShoot = true;
	private bool _shoot = false;
	private bool _canMove = true;
	private int _direction = 0;

	private AnimatedSprite2D _animatedSprite;
	private AnimatedSprite2D _engineAnimatedSprite;
	private AnimatedSprite2D _engineAnimatedSprite2;

	[Signal]
	public delegate void PlayerShootEventHandler(Vector2 position);
	[Signal]
	public delegate void PlayerDieEventHandler();

	public override void _Ready()
	{
	  _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	  _engineAnimatedSprite = GetNode<AnimatedSprite2D>("EngineAnimatedSprite2D");
	}

	public override void _Process(double delta)
	{
		if(_canMove)
		{
			switch(_direction)
			{
				case 1:
					_animatedSprite.Play("move");
					_animatedSprite.FlipH = false;
					_engineAnimatedSprite.Position = new Vector2(-1, 5);
					break;
				case -1:
					_animatedSprite.Play("move");
					_animatedSprite.FlipH = true;
					_engineAnimatedSprite.Position = new Vector2(1, 5);
					break;
				default:
					_animatedSprite.Play("default");
					_animatedSprite.FlipH = false;
					_engineAnimatedSprite.Position = new Vector2(0, 5);
					break;
			}
		}
		
		if (_canShoot && _shoot)
		{
			_canShoot = false;
			var position = GetNode<Marker2D>("ShootingPoint");
			EmitSignal(SignalName.PlayerShoot, position.GlobalPosition);
			var timer = GetNode<Timer>("ShootingTimer");
			timer.Start();
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = _direction switch
		{
				1 => Vector2.Up * _speed,
				-1 => Vector2.Down * _speed,
				_ => Vector2.Zero,
		};

		if (_canMove)
			MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionPressed("UP"))
		{
			_direction = 1;
		}
		else if(Input.IsActionPressed("DOWN"))
		{
			_direction = -1;
		}
		else
		{
			_direction = 0;
		}

		_shoot = Input.IsActionPressed("SHOOT");
		
	}

  private void OnShootingTimerTimeout()
	{
		_canShoot = true;
	}

	private void OnProjectileHit(Node2D body)
	{
		if (body == this)
		{
			_speed = 0;
			_canMove = false;
			_engineAnimatedSprite.QueueFree();
			_animatedSprite.Play("explode");

			var collition = GetNode<CollisionShape2D>("CollisionShape2D");
			collition.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
			EmitSignal(SignalName.PlayerDie);
		}
	}
	private void OnAnimatedSprite2DAnimationFinished()
	{
		if (_animatedSprite.Animation == "explode")
			QueueFree();
	}
}




