using MudExtensions;

namespace eAccount.Extensions;

public static class MudStepperExtendedExtensions
{
    public static async Task StepNextAsync(this MudStepperExtended stepper)
    {
        var index = stepper.GetActiveIndex();
        await stepper.CompleteStep(index);
    }
}