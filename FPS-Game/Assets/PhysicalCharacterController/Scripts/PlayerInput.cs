using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public float speed = 5;
	public float jumpHeight = 15;
	public PhysicalCC physicalCC;

	public Transform bodyRender;
	IEnumerator sitCort;
	public bool isSitting;

	void Update()
	{
		if (physicalCC.isGround)
		{
			physicalCC.moveInput = Vector3.ClampMagnitude(transform.forward
							* Input.GetAxis("Vertical")
							+ transform.right
							* Input.GetAxis("Horizontal"), 1f) * speed;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				physicalCC.inertiaVelocity.y = 0f;
				physicalCC.inertiaVelocity.y += jumpHeight;
			}

			if (Input.GetKeyDown(KeyCode.C) && sitCort == null)
			{
				sitCort = sitDown();
				StartCoroutine(sitCort);
			}
		}
	}

	IEnumerator sitDown()
	{
		if (isSitting && Physics.Raycast(transform.position, Vector3.up, physicalCC.cc.height * 1.5f))
		{
			sitCort = null;
			yield break;
		}
		isSitting = !isSitting;

		float t = 0;
		float startSize = physicalCC.cc.height;
		float finalSize = isSitting ? physicalCC.cc.height / 2 : physicalCC.cc.height * 2;

		Vector3 startBodySize = bodyRender.localScale;
		Vector3 finalBodySize = isSitting ? bodyRender.localScale - Vector3.up * bodyRender.localScale.y / 2f : bodyRender.localScale + Vector3.up * bodyRender.localScale.y;

		

		speed = isSitting ? speed / 2 : speed * 2;
		jumpHeight = isSitting ? jumpHeight * 3 : jumpHeight / 3;
		
		while (t < 0.2f)
		{
			t += Time.deltaTime;
			physicalCC.cc.height = Mathf.Lerp(startSize, finalSize, t / 0.2f);
			bodyRender.localScale = Vector3.Lerp(startBodySize, finalBodySize, t / 0.2f);
			yield return null;
		}

		sitCort = null;
		yield break;
	}
}
