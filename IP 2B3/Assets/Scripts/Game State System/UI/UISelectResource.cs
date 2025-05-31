using System;
using System.Collections;
using System.Collections.Generic;
using B3.ResourcesSystem;
using B3.UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace B3.GameStateSystem
{
    public static class UISelectResource
    {
		public static IEnumerator SelectResourceType(Action<ResourceType> returnCallback)
		{
			ResourceType? selectedResource = null;
			
			var instructionNotif = NotificationManager.Instance
                .AddNotification("Select a type of resource:", float.PositiveInfinity, false);
			
			List<NotificationManager.NotificationHandle> resourceNotifications = new();
			for (int i = 0; i < 5; i++) {
				var resType = (ResourceType) i;
				resourceNotifications.Add(NotificationManager.Instance
					.AddNotification(resType.GetString(), float.PositiveInfinity, false)
				);
                resourceNotifications[i].AddOnClickListener(() =>
                {
	                selectedResource = resType;
                });
            }

			while (selectedResource == null)
			{
				yield return new WaitForFixedUpdate();
			}
			
			instructionNotif.Destroy();
			for (int i = 0; i < 5; i++)
			{
				resourceNotifications[i].Destroy();
			}
			
			returnCallback(selectedResource.Value);
		}
        
    }
}