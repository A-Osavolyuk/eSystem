using MudExtensions;

namespace eAccount.Blazor.Server.UI.Extensions;

public static class MudStepperExtendedExtensions
{
    public static async Task StepNextAsync(this MudStepperExtended stepper)
    {
        var index = stepper.GetActiveIndex();
        await stepper.CompleteStep(index);
    }
}