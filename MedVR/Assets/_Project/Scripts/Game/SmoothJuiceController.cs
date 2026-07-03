using UnityEngine;

public class SmoothJuiceController : MonoBehaviour
{
    [Header("Configurações")]
    public float bobbingStrength = 0.05f;
    public float bobbingSpeed = 12f;
    public float transitionSpeed = 5f; // Velocidade do Fade in/out do juice

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;

    private float currentWeight = 0f; // 0 = sem juice, 1 = juice total
    private float targetWeight = 0f;
    private float timer = 0f;
    private Vector3 initialLocalPos;
    public Transform cameraTarget;

    void Start()
    {
        initialLocalPos = cameraTarget.localPosition;
    }

    // Chame isso quando o personagem começar a andar
    public void EnableJuice() => targetWeight = 1f;

    // Chame isso quando o personagem parar
    public void DisableJuice() => targetWeight = 0f;

    void Update()
    {
        // Interpolação suave do peso do juice para evitar transições abruptas
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * transitionSpeed);

        if (currentWeight > 0.01f)
        {
            ApplyBobbing();
        }
        else
        {
            // Retorna suavemente para a posição original ao parar
            cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, initialLocalPos, Time.deltaTime * transitionSpeed);
            timer = 0; // Reseta o timer para o próximo passo
        }
    }

    void ApplyBobbing()
    {
        timer += Time.deltaTime * bobbingSpeed;
        
        // O peso (currentWeight) multiplica o efeito final
        float bobX = Mathf.Sin(timer) * bobbingStrength * currentWeight;
        float bobY = Mathf.Abs(Mathf.Cos(timer)) * bobbingStrength * currentWeight;

        cameraTarget.localPosition = new Vector3(initialLocalPos.x + bobX, initialLocalPos.y + bobY, initialLocalPos.z);

        // Lógica de som com base no timer (ajuste o threshold conforme necessário)
      
    }

    public void PlayFootstep()
    {
        if (audioSource != null && footstepSounds.Length > 0)
        {
            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
        }
    }
}