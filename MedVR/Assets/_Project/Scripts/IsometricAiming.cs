using UnityEngine;


public class IsometricAiming : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings
    [SerializeField] private Camera playerCamera;

    [SerializeField] private LayerMask groundMask;
    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine; // Arraste o componente LineRenderer aqui
    [SerializeField] private Transform firePoint;    // Objeto vazio na ponta da arma
    [SerializeField] private float laserMaxLength = 50f;

    #endregion

    #region Private Fields

    private Camera mainCamera;

    #endregion

    #endregion

    #region Methods

    #region Unity Callbacks

    private void Start()
    {
        mainCamera = playerCamera;

        // Configuração básica do LineRenderer via código caso esqueça no Inspector
        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.startWidth = 0.05f;
            laserLine.endWidth = 0.05f;
        }
    }

    private void Update()
    {
        UpdateLaser();
    }

    #endregion


    private void UpdateLaser()
    {
        if (laserLine == null || firePoint == null) return;

        // Define o início do laser sempre na ponta da arma
        laserLine.SetPosition(0, firePoint.position);

        // Lógica de colisão do laser (para não atravessar paredes)
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, laserMaxLength, groundMask))
        {
            // Se bater em algo, o laser para no ponto de impacto
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            // Se não bater em nada, o laser vai até o comprimento máximo
            laserLine.SetPosition(1, firePoint.position + firePoint.forward * laserMaxLength);
        }
    }


    #endregion
}