using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpellsController : MonoBehaviour 
{
	[SerializeField]
	private ParticleSystem _previewParticlePrefab;
	[SerializeField]
	private ParticleSystem _blackHoleParticlePrefab;

	private ParticleSystem _previewInstance;
	private Camera _mainCamera;
	private RaycastHit hit;
	private bool _isCasting;

	private bool IsCasting 
	{
		get
		{
			return _isCasting;
		}

		set
		{
			if (_isCasting == value) {
				return;
			}

			if (!value) {
				Destroy(_previewInstance.gameObject);
			} else {
				_previewInstance = Instantiate(_previewParticlePrefab);
			}

			_isCasting = value;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))  {
			IsCasting = !IsCasting;
		}
		
		SnapParticlePreview();
	}

    private void SnapParticlePreview()
    {
		if (!IsCasting) {
			return;
		}

		var mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(mouseRay, out hit, 100f)) {
			_previewInstance.transform.position = hit.point;
		}
    }

	private void Awake()
	{
		_mainCamera = Camera.main;
	}
}
