using MudExtensions;

namespace eSecurity.Client.Extensions;

public static class MudStepperExtendedExtensions
{
    extension(MudStepperExtended stepper)
    {
        public async Task StepNextAsync()
        {
            var index = stepper.GetActiveIndex();
            await stepper.CompleteStep(index);
        }
    }
}