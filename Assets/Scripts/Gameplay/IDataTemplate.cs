public class IDataTemplate
{
    // Return false to mark an invalidation of the data, signifying that the data should be cleared.
    public virtual IDataTemplate DrawDataInspectors(GameplayData data)
    {
        return this;
    }
}