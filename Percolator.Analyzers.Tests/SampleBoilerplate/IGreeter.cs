using System.Threading.Tasks;

namespace SampleCode;

public interface IGreeter
{
    Task Greet();
    Task Farewell();
}