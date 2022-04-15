using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_101
{
    public class Test2
    {
        public static async Task Main()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(3000);
            });
        }

        public static async Task Main2()
        {
            await Task.Run(() => Task.Delay(3000));
        }
    }
}
