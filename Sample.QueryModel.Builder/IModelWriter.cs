namespace Sample.QueryModel.Builder
{
    public interface IModelWriter<T>
    {
        void Save(T model);
    }
}