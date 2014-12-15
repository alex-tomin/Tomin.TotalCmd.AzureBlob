using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomin.TotalCmd.AzureBlob.Helpers
{
	public class RetryPolicyCancellationWrapper : IRetryPolicy
	{
		private IRetryPolicy originalRetryPolicy;

		public RetryPolicyCancellationWrapper(IRetryPolicy originalRetryPolicy)
		{
			this.originalRetryPolicy = originalRetryPolicy;
		}

		public IRetryPolicy CreateInstance()
		{
			return new RetryPolicyCancellationWrapper(originalRetryPolicy);
		}

		public bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, Microsoft.WindowsAzure.Storage.OperationContext operationContext)
		{
			retryInterval = TimeSpan.FromSeconds(1);
			if (lastException is OperationCanceledException || lastException.InnerException is OperationCanceledException)
			{
				return false;
			}
			return originalRetryPolicy.ShouldRetry(currentRetryCount, statusCode, lastException, out retryInterval, operationContext);
		}
	}
}
