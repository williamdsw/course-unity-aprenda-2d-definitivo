using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DadosJogador 
{
	public int idIdioma;
	public int numeroOuro;
	public int idPersonagem;
	public int idArma;
	public int idFlechaEquipada;
	public int[] quantidadeFlechas;
	public int[] quantidadePocoes;
	public List<string> itensInventario;
	public List<int> aprimoramentosArmas;
}