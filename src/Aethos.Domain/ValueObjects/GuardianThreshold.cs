namespace Aethos.Domain.ValueObjects;

public readonly record struct GuardianThreshold
{
    public decimal AutonomousLimit { get; }
    public decimal MultiSigLimit { get; }

    private GuardianThreshold(decimal autonomousLimit, decimal multiSigLimit)
    {
        AutonomousLimit = autonomousLimit;
        MultiSigLimit = multiSigLimit;
    }

    public static GuardianThreshold Create(decimal autonomousLimit, decimal multiSigLimit)
    {
        if (autonomousLimit < 0 || multiSigLimit < 0)
            throw new ArgumentException("Limits cannot be negative.");

        if (multiSigLimit < autonomousLimit)
            throw new ArgumentException("MultiSig limit must be higher than Autonomous limit.");

        return new GuardianThreshold(autonomousLimit, multiSigLimit);
    }
}
