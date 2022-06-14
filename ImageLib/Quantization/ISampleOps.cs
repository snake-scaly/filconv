namespace ImageLib.Quantization
{
    public interface ISampleOps<T>
    {
        double Metric(T x, T y);
        T Sum(T x, T y);
        T Average(T sum, int count);
        bool CloseEnough(T x, T y);
    }
}
