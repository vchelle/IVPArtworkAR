﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelAnchorController : MonoBehaviour 
{
	[Tooltip("Transform of the controlled model.")]
	public Transform modelTransform;

	[Tooltip("Transform of the anchor object.")]
	public Transform anchorTransform;

	[Tooltip("Toggle to show if the model is active or not.")]
	public Toggle modelActiveToggle;

	[Tooltip("Toggle to show if the model is anchored or not.")]
	public Toggle anchorActiveToggle;

	[Tooltip("Whether the virtual model should rotate at the AR-camera or not.")]
	public bool modelLookingAtCamera = true;

	[Tooltip("UI-Text to show information messages.")]
	public Text infoText;


	// reference to the MultiARManager
	private MultiARManager arManager;
	// attached anchorId, or empty string
	private string anchorId;
	// internal action or not
	private bool bIntAction = false;


	void Start () 
	{
		// get reference to MultiARManager
		arManager = MultiARManager.Instance;

//		// select the model toggle at start
//		if(modelActiveToggle)
//		{
//			modelActiveToggle.isOn = true;
//		}
	}
	
	void Update () 
	{
//		// don't consider taps over the UI
//		if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//			return;

		// check for tap
		if (arManager && arManager.IsInitialized() && arManager.IsInputAvailable(true))
		{
			MultiARInterop.InputAction action = arManager.GetInputAction();

			if(action == MultiARInterop.InputAction.Click || action == MultiARInterop.InputAction.Grip)
			{
				if(modelTransform && modelTransform.gameObject.activeSelf)
				{
					// raycast world
					//Vector2 screenPos = Input.GetTouch(0).position;
					MultiARInterop.TrackableHit hit;

					if(arManager.RaycastToWorld(true, out hit))
					{
						// set model's new position
						SetModelWorldPos(hit.point);
					}
				}
			}
		}

		// check if anchorId is still valid
		if(arManager && anchorId != string.Empty && !arManager.IsValidAnchorId(anchorId))
		{
			anchorId = string.Empty;
		}

		// update the model-active and anchor-active transforms
		UpdateModelToggle(modelActiveToggle, modelTransform);
		UpdateModelToggle(anchorActiveToggle, anchorTransform);

		// update the info-text
		if(infoText)
		{
			string sMsg = (modelTransform && modelTransform.gameObject.activeSelf ? 
				"Model at " + modelTransform.transform.position + ", " : "No model, ");
			sMsg += (anchorTransform && anchorTransform.gameObject.activeSelf ? 
				"Anchor at " + anchorTransform.transform.position : "No anchor");
			sMsg += !string.IsNullOrEmpty(anchorId) ? "\nAnchorId: " + anchorId : string.Empty;

			infoText.text = sMsg;
		}
	}

	// positions the controlled model in the world
	private bool SetModelWorldPos(Vector3 vNewPos)
	{
		Camera arCamera = arManager.GetMainCamera();

		if(modelTransform && modelTransform.gameObject.activeSelf && arCamera)
		{
			// set position and look at the camera
			modelTransform.position = vNewPos;

			if (modelLookingAtCamera) 
			{
				modelTransform.LookAt(arCamera.transform);

				// avoid rotation around x
				Vector3 objRotation = modelTransform.rotation.eulerAngles;
				modelTransform.rotation = Quaternion.Euler(0f, objRotation.y, objRotation.z);
			}

			return true;
		}

		return false;
	}

	// update the toggle, depending on the model activity
	private void UpdateModelToggle(Toggle toggle, Transform model)
	{
		if(toggle)
		{
			if(model != null && toggle.isOn != model.gameObject.activeSelf)
			{
				bIntAction = true;
				toggle.isOn = model.gameObject.activeInHierarchy;
				bIntAction = false;
			}
		}
	}

	// invoked by the model toggle
	public void ModelToggleSelected(bool bOn)
	{
		if(bIntAction || !modelTransform || !arManager)
			return;

		if(bOn)
		{
			// activate model if needed
			if(!modelTransform.gameObject.activeSelf)
			{
				modelTransform.gameObject.SetActive(true);
			}

			if(anchorTransform && anchorTransform.gameObject.activeSelf)
			{
				// set model at anchor's position
				SetModelWorldPos(anchorTransform.position);
			}
			else
			{
				// raycast from center of the screen
				//Vector2 screenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
				MultiARInterop.TrackableHit hit;
				if(arManager.RaycastToWorld(false, out hit))
				{
					// set model position
					SetModelWorldPos(hit.point);
				}
			}

			// attach to existing anchor, if needed
			if(arManager.IsValidAnchorId(anchorId))
			{
				arManager.AttachObjectToAnchor(modelTransform.gameObject, anchorId, false, false);
			}
		}
		else
		{
			// detach from the anchor, if needed
			if(arManager.IsValidAnchorId(anchorId))
			{
				anchorId = arManager.DetachObjectFromAnchor(modelTransform.gameObject, anchorId, false, false);
			}

			// deactivate the model if needed
			if(modelTransform.gameObject.activeSelf)
			{
				modelTransform.gameObject.SetActive(false);
			}
		}
	}

	// invoked by the anchor
	public void AnchorToggleSelected(bool bOn)
	{
		if(bIntAction || !anchorTransform || !arManager)
			return;
		
		if(bOn)
		{
			// activate the anchor transform, if needed
			if(!anchorTransform.gameObject.activeSelf)
			{
				anchorTransform.gameObject.SetActive(true);
			}

			// create the anchor if needed
			if(!arManager.IsValidAnchorId(anchorId))
			{
				if(modelTransform && modelTransform.gameObject.activeSelf)
				{
					// create the world anchor at model's position
					anchorId = arManager.AnchorGameObjectToWorld(anchorTransform.gameObject, modelTransform.position, Quaternion.identity);
				}
				else
				{
					// raycast center of the screen
					//Vector2 screenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
					MultiARInterop.TrackableHit hit;
					if(arManager.RaycastToWorld(false, out hit))
					{
						// create the world anchor at hit's position
						anchorId = arManager.AnchorGameObjectToWorld(anchorTransform.gameObject, hit);
					}
				}

				// attach the model to the same anchor
				if(arManager.IsValidAnchorId(anchorId) && modelTransform && modelTransform.gameObject.activeSelf)
				{
					arManager.AttachObjectToAnchor(modelTransform.gameObject, anchorId, false, false);
				}
			}
		}
		else
		{
			// remove the anchor as needed
			if(arManager.IsValidAnchorId(anchorId))
			{
				// create the world anchor
				if(arManager.RemoveGameObjectAnchor(anchorId, true))
				{
					anchorId = string.Empty;
				}
			}

			// deactivate the anchor transform, if needed
			if(anchorTransform.gameObject.activeSelf)
			{
				anchorTransform.gameObject.SetActive(false);
			}
		}
	}

}