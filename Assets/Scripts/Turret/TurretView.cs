using UnityEngine;

namespace Turret
{
    public class TurretView : MonoBehaviour
    {
        [SerializeField] private Transform m_ProjectileOrigin;

        [SerializeField] private Transform m_Tower;
        public Transform ProjectileOrigin => m_ProjectileOrigin;
        
        private TurretData m_Data;

        public TurretData Data => m_Data;
        public void AttachData(TurretData turretData)
        {
            m_Data = turretData;
            transform.position = m_Data.Node.Position;
        }

        public void TowerLookAt(Vector3 point)
        {
            point.y = m_Tower.position.y;
            m_Tower.LookAt(point);
        }
    }
}