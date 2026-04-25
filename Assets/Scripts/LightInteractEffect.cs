// using UnityEngine;

// public class LightInteractEffect : MonoBehaviour
// {
//     private SpriteRenderer sr;

//     [SerializeField] private Color normalColor = Color.white;
//     [SerializeField] private Color highlightColor = Color.yellow;

//     [SerializeField] private float scaleMultiplier = 1.2f;

//     [SerializeField] private ParticleSystem glowParticles;

//     private Vector3 originalScale;
//     private bool isLit = false;

//     void Start()
//     {
//         sr = GetComponent<SpriteRenderer>();
//         originalScale = transform.localScale;

//         // 🔥 garante que a partícula começa desligada
//         if (glowParticles != null)
//         {
//             glowParticles.Stop();
//             glowParticles.Clear();
//         }
//     }

//     public void OnLightEnter()
//     {
//         if (isLit) return; // evita repetir várias vezes

//         isLit = true;

//         sr.color = highlightColor;
//         transform.localScale = originalScale * scaleMultiplier;

//         if (glowParticles != null)
//         {
//             glowParticles.Clear(); // limpa antes de tocar
//             glowParticles.Play();
//         }
//     }

//     public void OnLightExit()
//     {
//         if (!isLit) return;

//         isLit = false;

//         sr.color = normalColor;
//         transform.localScale = originalScale;

//         if (glowParticles != null)
//         {
//             glowParticles.Stop();
//             glowParticles.Clear();
//         }
//     }
// }



using UnityEngine;

public class LightInteractEffect : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [SerializeField] private float scaleMultiplier = 1.2f;

    [SerializeField] private GameObject glowObject; // 🔥 NOVO

    [SerializeField] private Transform glowTransform;

    private Vector3 originalScale;
    private bool isLit = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (glowObject != null)
            glowObject.SetActive(false); // começa desligado
    }

    void Update()
{
    if (isLit && glowTransform != null)
    {
        float pulse = Mathf.Sin(Time.time * 4f) * 0.1f;
        glowTransform.localScale = Vector3.one * (1.5f + pulse);
    }
}

    public void OnLightEnter()
    {
        if (isLit) return;

        isLit = true;

        sr.color = highlightColor;
        transform.localScale = originalScale * scaleMultiplier;

        if (glowObject != null)
            glowObject.SetActive(true);
    }

    public void OnLightExit()
    {
        if (!isLit) return;

        isLit = false;

        sr.color = normalColor;
        transform.localScale = originalScale;

        if (glowObject != null)
            glowObject.SetActive(false);
    }

    public void SetLit(bool value)
{
    if (value)
        OnLightEnter();
    else
        OnLightExit();
}


}