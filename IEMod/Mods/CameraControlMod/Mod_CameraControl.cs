using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.CameraControlMod {
	[ModifiesType("CameraControl")]
	public class mod_CameraControl : global::CameraControl
	{
		[ModifiesMember("DoUpdate")]
		public void DoUpdateNew()
		{
			
			if (Application.isPlaying)
			{
				Camera main = Camera.main;
				if (this.m_forceReset)
				{
					this.m_testLeft = true;
					this.m_testRight = true;
					this.m_testTop = true;
					this.m_testBottom = true;
					this.m_forceReset = false;
				}
				else
				{
					this.m_testLeft = false;
					this.m_testRight = false;
					this.m_testTop = false;
					this.m_testBottom = false;
				}
				if (this.PlayerControlEnabled && GameState.ApplicationIsFocused)
				{
					if (this.PlayerScrollEnabled)
					{
						float axisRaw = Input.GetAxisRaw("Mouse ScrollWheel");
						if (axisRaw != 0f)
						{
							this.OrthoSettings.SetZoomLevelDelta(axisRaw);
							this.ResetAtEdges();
						}
					}
					if (GameInput.GetDoublePressed(KeyCode.Mouse0, true) && !UINoClick.MouseOverUI)
					{
						Vector3 worldMousePosition = GameInput.WorldMousePosition;
						if (GameCursor.CharacterUnderCursor != null)
						{
							worldMousePosition = GameCursor.CharacterUnderCursor.transform.position;
							this.ResetAtEdges();
						}
						if (Instance != null)
						{
							Instance.FocusOnPoint(worldMousePosition, 0.4f);
							this.ResetAtEdges();
						}
					}
					if (GameInput.GetControlDown(MappedControl.ZOOM_IN))
					{
						this.OrthoSettings.SetZoomLevelDelta(this.m_zoomRes);
						this.ResetAtEdges();
					}
					if (GameInput.GetControlDown(MappedControl.ZOOM_OUT))
					{
						this.OrthoSettings.SetZoomLevelDelta(-this.m_zoomRes);
						this.ResetAtEdges();
					}
					if (GameInput.GetControlDown(MappedControl.RESET_ZOOM))
					{
						this.OrthoSettings.SetZoomLevel(PlayerPrefs.GetFloat("DefaultZoom", 1f), false); // changed this line
						this.ResetAtEdges();
					}
					if (GameInput.GetControlDown(MappedControl.PAN_CAMERA))
					{
						this.m_mouseDrag_lastMousePos = GameInput.MousePosition;
						float x = main.pixelWidth * 0.5f;
						float y = main.pixelHeight * 0.5f;
						Vector3 vector2 = this.ProjectScreenCoordsToGroundPlane(main, new Vector3(x + 1f, y, main.nearClipPlane));
						Vector3 vector3 = this.ProjectScreenCoordsToGroundPlane(main, new Vector3(x, y, main.nearClipPlane));
						this.CameraPanDeltaX = vector3 - vector2;
						vector2 = this.ProjectScreenCoordsToGroundPlane(main, new Vector3(x, y + 1f, main.nearClipPlane));
						vector3 = this.ProjectScreenCoordsToGroundPlane(main, new Vector3(x, y, main.nearClipPlane));
						this.CameraPanDeltaY = vector3 - vector2;
					}
					else if (GameInput.GetControl(MappedControl.PAN_CAMERA))
					{
						Vector3 vector4 = GameInput.MousePosition - this.m_mouseDrag_lastMousePos;
						this.m_mouseDrag_lastMousePos = GameInput.MousePosition;
						if (vector4.x < 0f)
						{
							this.m_atLeft = false;
						}
						else if (vector4.x > 0f)
						{
							this.m_atRight = false;
						}
						if (vector4.y < 0f)
						{
							this.m_atBottom = false;
						}
						else if (vector4.y > 0f)
						{
							this.m_atTop = false;
						}
						if (this.m_atRight && (vector4.x < 0f))
						{
							vector4.x = 0f;
						}
						else if (this.m_atLeft && (vector4.x > 0f))
						{
							vector4.x = 0f;
						}
						if (this.m_atTop && (vector4.y < 0f))
						{
							vector4.y = 0f;
						}
						else if (this.m_atBottom && (vector4.y > 0f))
						{
							vector4.y = 0f;
						}
						if (vector4.x < 0f)
						{
							this.m_testRight = true;
						}
						else if (vector4.x > 0f)
						{
							this.m_testLeft = true;
						}
						if (vector4.y < 0f)
						{
							this.m_testTop = true;
						}
						else if (vector4.y > 0f)
						{
							this.m_testBottom = true;
						}
						this.position_offset += (Vector3) ((-main.transform.right * this.CameraPanDeltaX.magnitude) * vector4.x);
						this.position_offset += (Vector3) ((-main.transform.up * Vector3.Dot(-main.transform.up, this.CameraPanDeltaY)) * vector4.y);
					}
					else
					{
						bool option = GameState.Option.GetOption(GameOption.BoolOption.SCREEN_EDGE_SCROLLING);
						bool flag2 = (GameInput.MousePosition.x > (0f + this.m_mouseScrollBufferOuter)) && (GameInput.MousePosition.x < (Screen.width - this.m_mouseScrollBufferOuter));
						bool flag3 = (GameInput.MousePosition.y > (0f + this.m_mouseScrollBufferOuter)) && (GameInput.MousePosition.y < (Screen.height - this.m_mouseScrollBufferOuter));
						if (GameInput.GetControl(MappedControl.PAN_CAMERA_LEFT) || ((flag3 && option) && ((GameInput.MousePosition.x < this.m_mouseScrollBuffer) && (GameInput.MousePosition.x > this.m_mouseScrollBufferOuter))))
						{
							this.m_atRight = false;
							if (!this.m_atLeft)
							{
								this.position_offset -= (Vector3) (Camera.main.transform.right * this.CameraMoveDelta);
								this.m_testLeft = true;
							}
						}
						else if (GameInput.GetControl(MappedControl.PAN_CAMERA_RIGHT) || ((flag3 && option) && ((GameInput.MousePosition.x > (Screen.width - this.m_mouseScrollBuffer)) && (GameInput.MousePosition.x < (Screen.width - this.m_mouseScrollBufferOuter)))))
						{
							this.m_atLeft = false;
							if (!this.m_atRight)
							{
								this.position_offset += (Vector3) (Camera.main.transform.right * this.CameraMoveDelta);
								this.m_testRight = true;
							}
						}
						if (GameInput.GetControl(MappedControl.PAN_CAMERA_DOWN) || ((flag2 && option) && ((GameInput.MousePosition.y < this.m_mouseScrollBuffer) && (GameInput.MousePosition.y > this.m_mouseScrollBufferOuter))))
						{
							this.m_atTop = false;
							if (!this.m_atBottom)
							{
								this.position_offset -= (Vector3) (Camera.main.transform.up * this.CameraMoveDelta);
								this.m_testBottom = true;
							}
						}
						else if (GameInput.GetControl(MappedControl.PAN_CAMERA_UP) || ((flag2 && option) && ((GameInput.MousePosition.y > (Screen.height - this.m_mouseScrollBuffer)) && (GameInput.MousePosition.y < (Screen.height - this.m_mouseScrollBufferOuter)))))
						{
							this.m_atBottom = false;
							if (!this.m_atTop)
							{
								this.position_offset += (Vector3) (Camera.main.transform.up * this.CameraMoveDelta);
								this.m_testTop = true;
							}
						}
					}
				}
				if (this.InterpolatingToTarget)
				{
					if (GameState.s_playerCharacter != null)
					{
						this.MoveTime -= global::CameraControl.GetDeltaTime();
					}
					Vector3 moveToPointDest = Vector3.zero;
					if (this.MoveTime <= 0f)
					{
						this.MoveTime = 0f;
						this.InterpolatingToTarget = false;
						moveToPointDest = this.MoveToPointDest;
					}
					else
					{
						float t = this.MoveTime / this.MoveTotalTime;
						moveToPointDest.x = Mathf.SmoothStep(this.MoveToPointDest.x, this.MoveToPointSrc.x, t);
						moveToPointDest.y = Mathf.SmoothStep(this.MoveToPointDest.y, this.MoveToPointSrc.y, t);
						moveToPointDest.z = Mathf.SmoothStep(this.MoveToPointDest.z, this.MoveToPointSrc.z, t);
					}
					this.lastPosition = moveToPointDest;
					base.transform.position = moveToPointDest;
					this.position_offset = Vector3.zero;
					this.ResetAtEdges();
				}
				Vector3 zero = Vector3.zero;
				if ((this.m_screenShakeTimer > 0f) && (GameState.s_playerCharacter != null))
				{
					this.m_screenShakeTimer -= Time.unscaledDeltaTime;
					Vector3 vector7 = (Vector3) ((UnityEngine.Random.onUnitSphere * this.m_screenShakeStrength) * (this.m_screenShakeTimer / this.m_screenShakeTotalTime));
					zero += (Vector3) (vector7.x * -main.transform.right);
					zero += (Vector3) (vector7.y * -main.transform.up);
				}
				Vector3 vector8 = Vector3.zero;
				base.transform.position = (this.lastPosition + this.position_offset) + zero;
				if (!this.m_blockoutMode)
				{
					Vector3 lhs = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
					Vector3 vector10 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
					Vector3 vector11 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, Camera.main.nearClipPlane)) - lhs;
					Vector3 vector12 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, Camera.main.nearClipPlane)) - lhs;
					float magnitude = vector11.magnitude;
					float num6 = vector12.magnitude;
					lhs -= this.m_worldBoundsOrigin;
					vector10 -= this.m_worldBoundsOrigin;
					Vector3 worldBoundsX = this.m_worldBoundsX;
					Vector3 worldBoundsY = this.m_worldBoundsY;
					worldBoundsX.Normalize();
					worldBoundsY.Normalize();
					float num7 = Vector3.Dot(lhs, worldBoundsX);
					float num8 = Vector3.Dot(lhs, worldBoundsY);
					float num9 = Vector3.Dot(vector10, worldBoundsX);
					float num10 = Vector3.Dot(vector10, worldBoundsY);
					float num11 = this.m_worldBoundsX.magnitude;
					float num12 = this.m_worldBoundsY.magnitude;
					float num13 = this.BufferLeft * magnitude;
					float num14 = this.BufferRight * magnitude;
					float num15 = this.BufferTop * num6;
					float num16 = this.BufferBottom * num6;
					if (magnitude > num11)
					{
						float num17 = (magnitude - num11) / 2f;
						num13 += num17;
						num14 += num17;
					}
					if (num6 > num12)
					{
						float num18 = (num6 - num12) / 2f;
						num15 += num18;
						num16 += num18;
					}
					if (this.m_testLeft && (num7 < -num13))
					{
						vector8 += (Vector3) ((-num7 - num13) * worldBoundsX);
						this.m_atLeft = true;
						this.m_atRight = false;
					}
					else if (this.m_testRight && (num9 > (num11 + num14)))
					{
						vector8 -= (Vector3) ((num9 - (num11 + num14)) * worldBoundsX);
						this.m_atRight = true;
						this.m_atLeft = false;
					}
					if (this.m_testBottom && (num8 < -num16))
					{
						vector8 += (Vector3) ((-num8 - num16) * worldBoundsY);
						this.m_atBottom = true;
						this.m_atTop = false;
					}
					else if (this.m_testTop && (num10 > (num12 + num15)))
					{
						vector8 -= (Vector3) ((num10 - (num12 + num15)) * worldBoundsY);
						this.m_atTop = true;
						this.m_atBottom = false;
					}
					base.transform.position = (this.lastPosition + this.position_offset) + vector8;
					this.lastPosition = base.transform.position;
					Transform transform = base.transform;
					transform.position += zero;
					this.position_offset = Vector3.zero;
				}
				if (this.Audio != null)
				{
					RaycastHit hit;
					Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
					int layerMask = ((int) 1) << LayerMask.NameToLayer("Walkable");
					if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask))
					{
						this.Audio.position = hit.point;
						this.m_lastAudioY = this.Audio.position.y;
					}
					else
					{
						Plane cameraPlane = new Plane(Vector3.up, new Vector3(0f, this.m_lastAudioY, 0f));
						this.Audio.position = this.GetPlaneRayIntersectionPosition(cameraPlane, ray);
					}
				}
			}
		}
	}
}
