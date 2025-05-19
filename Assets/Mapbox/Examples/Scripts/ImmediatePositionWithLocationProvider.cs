namespace Mapbox.Examples
{
	using Mapbox.Unity.Location;
    using Mapbox.Unity.Map;
    using UnityEngine;
	using System.Collections;

	public class ImmediatePositionWithLocationProvider : MonoBehaviour
	{

		bool _isInitialized;

		ILocationProvider _locationProvider;
		ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
		}

		Vector3 _targetPosition;

		void Start()
		{
			LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
			StartCoroutine(DelayedPlayerPositionInitialization(0.5f));
        }

		/*void LateUpdate()
		{
			if (_isInitialized)
			{
				var map = LocationProviderFactory.Instance.mapManager;
				transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
            }
		}*/

		IEnumerator DelayedPlayerPositionInitialization(float time)
		{
			yield return new WaitForSeconds(time);
			var map = LocationProviderFactory.Instance.mapManager;
			transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
		}
	}
}