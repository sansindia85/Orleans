﻿using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{    
    public interface IHello : IGrainWithGuidKey
    {
        Task<string> SayHelloAsync(string greeting);
    }
}
