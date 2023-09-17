namespace Trainworks.BuildersV2
{
    /// <summary>
    /// Interface for RewardDataBuilders.
    /// </summary>
    public interface IRewardDataBuilder
    {
        RewardData BuildAndRegister();
        RewardData Build(bool register = true);
    }
}