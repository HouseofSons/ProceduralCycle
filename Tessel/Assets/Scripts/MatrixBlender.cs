using UnityEngine;
using System.Collections;

public class MatrixBlender : MonoBehaviour
{
	public static Matrix4x4 ortho;
	public static Matrix4x4 perspective;

	public bool finishedBending = true;

	void Start() {
		float fov = 60f, near = this.gameObject.GetComponent<Camera> ().nearClipPlane, far = this.gameObject.GetComponent<Camera> ().farClipPlane, orthographicSize = this.gameObject.GetComponent<Camera> ().orthographicSize;

		ortho =
			Matrix4x4.Ortho(-orthographicSize * (float) Screen.width / (float) Screen.height,
			orthographicSize * (float) Screen.width / (float) Screen.height,
			-orthographicSize, orthographicSize, near, far);
		perspective =
			Matrix4x4.Perspective(fov,
			(float) Screen.width / (float) Screen.height,
			near,
			far);
	}

	public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
	{
		Matrix4x4 ret = new Matrix4x4();
		for (int i = 0; i < 16; i++)
			ret[i] = Mathf.Lerp(from[i], to[i], time);
		return ret;
	}

	public IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration, float extender)
	{
		finishedBending = false;
		float startTime = Time.time;
		while (Time.time - startTime < duration * extender)
		{
			this.gameObject.GetComponent<Camera> ().projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / (duration * extender));
			extender-=Time.deltaTime*20;
			yield return null;
		}
		this.gameObject.GetComponent<Camera> ().projectionMatrix = dest;
		finishedBending = true;
	}

	public IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
	{
		finishedBending = false;
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			this.gameObject.GetComponent<Camera> ().projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / (duration));
			yield return null;
		}
		this.gameObject.GetComponent<Camera> ().projectionMatrix = dest;
		finishedBending = true;
	}
}