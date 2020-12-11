using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Permite adicionar isso nos menus acessiveis
[CreateAssetMenu (fileName = "Nova Arma", menuName = "Arma")]
public class ItemModelo : ScriptableObject
{
	public int idArma;
}