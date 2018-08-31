using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PlexusEffect : MonoBehaviour
{
	[SerializeField] private float _distance = 1f;
	[SerializeField] private int _maxConnections = 4;
	[SerializeField] private int _maxRenderers = 100;
	[Space, SerializeField] private LineRenderer _lineRendererPrefab;
	
	private ParticleSystem _particleSystem;
	private ParticleSystem.Particle[] _particles;
	private ParticleSystem.MainModule _mainModule;
	private LineRenderer[] _lineRenderers;
	private Gradient[] _gradients;
	private Transform _transform;

	private void Start()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		_mainModule = _particleSystem.main;
		_particles = new ParticleSystem.Particle[_mainModule.maxParticles];
		_transform = transform;
		
		_lineRenderers = new LineRenderer[_maxRenderers];
		_gradients = new Gradient[_maxRenderers];
		for (int i = 0; i < _maxRenderers; i++)
		{
			_lineRenderers[i] = Instantiate(_lineRendererPrefab, _transform, false);
			InitGradient(i);
		}
	}

	private void LateUpdate()
	{
		if (_maxConnections <= 0 || _maxRenderers <= 0)
		{
			return;
		}

		UpdateBuffersIfNeeded();
		UpdateLines();
	}

	private void UpdateLines()
	{
		var lrIndex = 0;
		var distanceSqr = _distance * _distance;
		var particleCount = _particleSystem.GetParticles(_particles);
		for (int i = 0; i < particleCount; i++)
		{
			if (lrIndex >= _maxRenderers)
			{
				return;
			}
			
			var connections = 0;
			var p1 = _particles[i];
			var p1Pos = p1.position;
			for (int j = i + 1; j < particleCount; j++)
			{
				var p2 = _particles[j];
				var p2Pos = p2.position;
				var distance = Vector3.SqrMagnitude(p1Pos - p2Pos);
				if (distance <= distanceSqr)
				{
					var p1Col = p1.GetCurrentColor(_particleSystem);
					var p2Col = p2.GetCurrentColor(_particleSystem);
					lrIndex = MakeLine(p1Pos, p1Col, p2Pos, p2Col, lrIndex);
					connections++;
				}

				if (lrIndex >= _maxRenderers || connections > _maxConnections)
				{
					break;
				}
			}
		}

		for (int i = lrIndex; i < _maxRenderers; i++)
		{
			_lineRenderers[i].enabled = false;
		}
	}

	private int MakeLine(Vector3 pos1, Color32 col1, Vector3 pos2, Color32 col2, int rendererIndex)
	{
		var lr = _lineRenderers[rendererIndex];
		
		lr.enabled = true;
		lr.SetPosition(0, pos1);
		lr.SetPosition(1, pos2);
		
		var gradient = _gradients[rendererIndex];
		var keys = gradient.colorKeys;
		var k1 = keys[0];
		var k2 = keys[1];
		k1.color = col1;
		k2.color = col2;
		keys[0] = k1;
		keys[1] = k2;
		gradient.colorKeys = keys;
		
		lr.colorGradient = gradient;
		
		return rendererIndex + 1;
	}

	private void UpdateBuffersIfNeeded()
	{
		var maxParticles = _mainModule.maxParticles;
		if (maxParticles > _particles.Length)
		{
			_particles = new ParticleSystem.Particle[maxParticles];
		}

		var renderersCount = _lineRenderers.Length;
		if (_maxRenderers > renderersCount)
		{
			var buffer = new LineRenderer[_maxRenderers];
			Array.Copy(_lineRenderers, buffer, renderersCount);
			_lineRenderers = buffer;

			for (int i = renderersCount; i < _maxRenderers; i++)
			{
				_lineRenderers[i] = Instantiate(_lineRendererPrefab, _transform, false);
				InitGradient(i);
			}
		}
	}

	private void InitGradient(int index)
	{
		var gradient = new Gradient();
		var k1 = new GradientColorKey(Color.white, 0f);
		var k2 = new GradientColorKey(Color.white, 1f);
		gradient.colorKeys = new[] {k1, k2};
		_gradients[index] = gradient;
	}
}
