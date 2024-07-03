using Godot;
using System;

public partial class Camera : Godot.Camera2D
{
	private Random _randomGenerator = new();
	private double _timeKeeper = 0;
	private int _cameraShakeAmplitud = 80;
	private bool _shakeCamera = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (_shakeCamera)
		{
			_timeKeeper += delta;
			var sinValue = (float)Math.Sin(_timeKeeper * 100) * _cameraShakeAmplitud;
			var cosValue = (float)Math.Cos(_timeKeeper * 100) * _cameraShakeAmplitud;
			_cameraShakeAmplitud -= 2;
			var direction = new Vector2(sinValue + (float)_randomGenerator.NextDouble(), cosValue + (float)_randomGenerator.NextDouble());

			if(_cameraShakeAmplitud < 0)
			{
				_cameraShakeAmplitud = 80;
				_shakeCamera = false;
			}

			Position += direction;
		}
	}

	public void ShakeCamera()
	{
		_shakeCamera = true;
	}
}
