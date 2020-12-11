using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollision : MonoBehaviour 
{
	public LayerMask destruirLayer;

	// ------------------- FUNCOES UNITY ------------------- //

	private void Update () 
	{
		// Detecta colisao entre a propria posicao e uma distancia a '0.1f'
		RaycastHit2D hit = Physics2D.Raycast (this.transform.position, Vector2.right, 0.1f, destruirLayer);

		if (hit)
		{
			Destroy (this.gameObject);
		}
	}
}