using UnityEngine;
using Grid = Fields.Grid;

namespace Enemy
{
    public class EnemyView : MonoBehaviour
    {
        private EnemyData m_Data;
        private IMovementAgent m_MovementAgent;

        public EnemyData Data => m_Data;

        [SerializeField] private Animator m_Animator;
        private static readonly int DieAnimationIndex = Animator.StringToHash("Die");

        public IMovementAgent MovementAgent => m_MovementAgent;

        public void AttachData(EnemyData data)
        {
            m_Data = data;
        }

        public void CreateMovementAgent(Grid grid)
        {
            if (m_Data.Asset.IsFlyingEnemy)
            {
                m_MovementAgent = new FlyingMovementAgent(m_Data.Asset.Speed, transform, grid, m_Data);
            }
            if (!m_Data.Asset.IsFlyingEnemy)
            {
                m_MovementAgent = new GridMovementAgent(m_Data.Asset.Speed, transform, grid, m_Data);
            }
        }

        public void Die()
        {
            m_Animator.SetTrigger(DieAnimationIndex);
        }
    }
}