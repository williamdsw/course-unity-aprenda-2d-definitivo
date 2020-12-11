using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxRevisao : MonoBehaviour
{
    // Variaveis de controle
    public float scale;
    public float velocity;

    // Objetos
    public Transform backgroundTransform;

    // Propriedades da camera
    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    // ------------------- FUNCOES UNITY ------------------- //

    private void Start () 
    {
        // Inicializa
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    private void LateUpdate () 
    {
        // Calcula efeito parallax em X
        float parallaxEffectX = (previousCameraPosition.x - cameraTransform.position.x) * scale;
        float targetBackgroundX = (backgroundTransform.position.x + parallaxEffectX);

        // Define nova posicao
        Vector3 backgroundPosition = new Vector3 (targetBackgroundX, backgroundTransform.position.y, backgroundTransform.position.z);

        // Faz a animacao de uma posicao ate outra
		// "Time.deltaTime" = Quanto tempo se passou de um frame para o outro
        backgroundTransform.position = Vector3.Lerp (backgroundTransform.position, backgroundPosition, velocity * Time.deltaTime);

        // Armazena posicao da camera
        previousCameraPosition = cameraTransform.position;
    }
}