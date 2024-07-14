using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueHub.Source.dto
{
    public static class MethodCallDtoExtensions
    {
        public static MethodCalldto<string[]> ConvertToMethodCallDtoStringArray(this MethodCalldto<dynamic> dynamicDto)
        {
            MethodCalldto<string[]> stringArrayDto = new MethodCalldto<string[]>();

            if (dynamicDto.Args is List<string> stringList)
            {
                stringArrayDto.Args = stringList.ToArray();
            }
            else if (dynamicDto.Args is string str)
            {
                stringArrayDto.Args = new string[] { str };
            }
            else
            {
                // Handle other types or throw an exception
                throw new InvalidOperationException("Unsupported data type in MethodCallDto<dynamic>.Data");
            }

            return stringArrayDto;
        }

        public static MethodCalldto<object[]> ConvertToMethodCallDtoObjectArray(this MethodCalldto<dynamic> dynamicDto)
        {
            MethodCalldto<object[]> objectArrayDto = new MethodCalldto<object[]>();

            if (dynamicDto.Args is IEnumerable<object> objectList)
            {
                objectArrayDto.Args = new List<object>(objectList).ToArray();
            }
            else if (dynamicDto.Args != null)
            {
                objectArrayDto.Args = new object[] { dynamicDto.Args };
            }
            else
            {
                // Handle null case
                objectArrayDto.Args = Array.Empty<object>();
            }

            return objectArrayDto;
        }
    }
}
