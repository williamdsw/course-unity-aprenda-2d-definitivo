using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollisionRevisao : MonoBehaviour 
{
	public LayerMask layerDestroy;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Update () 
	{
		// Detecta colisao entre a propria posicao e uma distancia a '0.1f'
		RaycastHit2D hit = Physics2D.Raycast (this.transform.position, Vector2.right, 0.1f, layerDestroy);

		if (hit)
		{
			Destroy (this.gameObject);
		}
	}
}