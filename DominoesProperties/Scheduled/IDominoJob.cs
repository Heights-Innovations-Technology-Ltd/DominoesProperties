using System.Threading.Tasks;

namespace DominoesProperties.Scheduled
{
    public interface IDominoJob
    {
        void PerformPairInvestment();
        Task<bool> CheckUnclosedPairing();
    }
}

