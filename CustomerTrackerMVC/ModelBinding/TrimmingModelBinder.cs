using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CustomerTrackerMVC.ModelBinding
{
    public class TrimmingModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            string? value = result.FirstValue?.Trim();
            if (string.IsNullOrEmpty(value) && bindingContext.ModelMetadata.ConvertEmptyStringToNull)
            {
                value = null;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value, value);
            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }
    }

    public class TrimmingModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(string) ? new TrimmingModelBinder() : null;
        }
    }
}