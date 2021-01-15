using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiloHost.Filters
{
    public class LoggingFilter : IIncomingGrainCallFilter
    {
        private readonly GrainInformation _grainInformation;
        private readonly ILogger<LoggingFilter> _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public LoggingFilter(GrainInformation grainInformation, 
            ILogger<LoggingFilter> logger,
            JsonSerializerSettings jsonSerializerSettings)
        {
            _grainInformation = grainInformation;
            _logger = logger;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                if (ShouldLog(context.InterfaceMethod.Name))
                {
                    var arguments = JsonConvert.SerializeObject(context.Arguments, _jsonSerializerSettings);
                    _logger.LogInformation($"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name}: arguments: {arguments} request");
                }


                //Trick the Grain Method which we are intercepting
                await context.Invoke();

                if (ShouldLog(context.InterfaceMethod.Name))
                {
                    var result = JsonConvert.SerializeObject(context.Result, _jsonSerializerSettings);
                    _logger.LogInformation($"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name}: result : {result} request");
                }
            }
            catch (Exception ex)
            {
                var arguments = JsonConvert.SerializeObject(context.Arguments, _jsonSerializerSettings);
                var result = JsonConvert.SerializeObject(context.Result, _jsonSerializerSettings);
                _logger.LogError($"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name}: threw an exception: {nameof(ex)} request", ex);

                throw;
            }                            
        }

        private bool ShouldLog(string methodName)
        {
            return _grainInformation.Methods.Contains(methodName);
        }
    }
}
