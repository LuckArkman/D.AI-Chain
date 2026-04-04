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

    public static GuardianThreshold Create(decimal autonomousLimitEth, decimal multiSigLimitEth)
    {
        if (autonomousLimitEth < 0m)
            throw new ArgumentException("Autonomous limit cannot be negative.", nameof(autonomousLimitEth));

        if (multiSigLimitEth < autonomousLimitEth)
            throw new ArgumentException("MultiSig limit must be greater or equal to autonomous limit.", nameof(multiSigLimitEth));

        return new GuardianThreshold(autonomousLimitEth, multiSigLimitEth);
    }
}
