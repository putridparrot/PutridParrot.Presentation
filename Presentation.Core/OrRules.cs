namespace Presentation.Core
{
    public class OrRules : Rules
    {
        public override bool PostInvoke<T>(T viewModel, string propertyName)
        {
            foreach (var kv in _rules)
            {
                if (kv.Key == propertyName)
                {
                    return base.PostInvoke(viewModel, propertyName);
                }
            }
            return false;
        }
    }
}