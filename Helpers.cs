namespace webapi;

internal static class Helpers
{
    public static void Patch<TSource, TTarget>(TSource source, TTarget target)
    {
        var sourceProps = typeof(TSource).GetProperties();
        foreach (var sourceProp in sourceProps)
        {
            var value = sourceProp.GetValue(source);
            if (value is not null)
            {
                var targetProp = typeof(TTarget).GetProperty(sourceProp.Name);
                targetProp?.SetValue(target, value);
            }
        }
    }
}