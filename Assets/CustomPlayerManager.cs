using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class CustomPlayerSpawner : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    private int playerCount = 0;
    
    // Exemplo: tecla Enter para adicionar jogador
    void Update()
    {
            // Pressione Enter para adicionar os dois jogadores (uma vez só)
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                Debug.Log("botao apertado");
                // PLAYER 1 - Teclado
                PlayerInputManager.instance.playerPrefab = player1Prefab;
                PlayerInputManager.instance.JoinPlayer(
                    playerIndex: 0,
                    controlScheme: "Player1",
                    pairWithDevice: Keyboard.current
                );

                // PLAYER 2 - Controle
                if (Gamepad.current != null)
                {
                    PlayerInputManager.instance.playerPrefab = player2Prefab;
                    PlayerInputManager.instance.JoinPlayer(
                        playerIndex: 1,
                        controlScheme: "Player2",
                        pairWithDevice: Gamepad.current
                    );
                }
                else
                {
                    Debug.LogWarning("Nenhum controle conectado!");
                }

                playerCount = 2; // Evita criar de novo

            }
    }
}

