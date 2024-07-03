using Godot;
using System;

public partial class Level : Node2D
{
	private PackedScene _projectile = GD.Load<PackedScene>("res://scenes/Projectile.tscn");
	private PackedScene _enemyProjectile = GD.Load<PackedScene>("res://scenes/EnemyProjectile.tscn");
	private PackedScene _enemy = GD.Load<PackedScene>("res://scenes/Enemy.tscn");
	private PackedScene _betaEnemy = GD.Load<PackedScene>("res://scenes/BetaEnemy.tscn");
	private Random _randomGenerator = new Random();
	private double _timeKeeper = 0;
	private int _cameraShakeAmplitud = 80;

	public override void _Ready()
	{
		GetNode<Timer>("RespawnEnemyTimmer").Start();

		var player = GetNode<Player>("Player");

		player.TreeExited += () => GetNode<Timer>("RestartLevelTimer").Start();
		player.PlayerDie += () => GetNode<Camera>("Camera").ShakeCamera();
	}

	public override void _Process(double delta)
	{
		_timeKeeper += delta;
		
	}

	private void OnPlayerShoot(Vector2 position)
	{
		var projectile = _projectile.Instantiate<Projectile>();
		projectile.Rotate((float)Math.PI);
		projectile.Position = position;
		projectile.CollisionLayer = 0b1000;
		projectile.CollisionMask = 0b110;

		// var endMapRight = GetNode<Area2D>("EndMapRight");
		// endMapRight.Connect(Area2D.SignalName.AreaEntered, new Callable(projectile, Projectile.MethodName.OnCrossEndMap));

		AddChild(projectile);
	}

	private void OnEnemyShoot(Vector2 position, Vector2 direction)
	{
		var projectile = _enemyProjectile.Instantiate<Projectile>();

		projectile.Position = position;
		projectile.Direction = direction;
		projectile.CollisionLayer = 0b10000;
		projectile.CollisionMask = 0b11;

		// var endMapRight = GetNodeOrNull<Area2D>("EndMapLeft");
		var player = GetNodeOrNull<Player>("Player");
		// endMapRight.Connect(Area2D.SignalName.AreaEntered, new Callable(projectile, Projectile.MethodName.OnCrossEndMap));

		if (player is not null)
			projectile.Connect(Projectile.SignalName.BodyEntered, new Callable(player, Player.MethodName.OnProjectileHit));

		AddChild(projectile);
	}

	private void AddEnemy()
	{
		// var position = new Vector2(1200, _randomGenerator.Next(70, 500));
		var position = new Vector2(1000, _randomGenerator.Next(70, 500));
		var enemy = _randomGenerator.Next(0, 2) == 1 ? _enemy.Instantiate<Enemy>() : _betaEnemy.Instantiate<Enemy>();
		var endMapLeft = GetNode<Area2D>("EndMapLeft");
		var player = GetNodeOrNull<Player>("Player");

		enemy.GlobalPosition = position;
		// endMapLeft.Connect(Area2D.SignalName.AreaEntered, new Callable(enemy, Enemy.MethodName.OnCrossEndMap));
		enemy.Connect(Enemy.SignalName.EnemyShoot, new Callable(this, MethodName.OnEnemyShoot));

		if (player is not null)
			enemy.Connect(Projectile.SignalName.BodyEntered, new Callable(player, Player.MethodName.OnProjectileHit));

		AddChild(enemy);
	}

	private void OnRespawnEnemyTimmerTimeout()
	{
		AddEnemy();
	}
	private void OnEestartLevelTimerTimeout()
	{
		GetTree().ReloadCurrentScene();
	}
}


