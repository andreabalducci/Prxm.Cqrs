namespace Sample.QueryModel.Storage.Readers
{
    public interface IModelWriter<T>
    {
        void Save(T model);
    }
}