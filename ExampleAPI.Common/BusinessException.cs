
namespace ExampleAPI.Common
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {

        }
        public override string ToString()
        {
            return Message;
        }
    }
}
