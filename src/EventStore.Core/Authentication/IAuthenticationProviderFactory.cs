﻿using EventStore.Core.Bus;
using EventStore.Core.Services;
using EventStore.Core.Services.Transport.Http;

namespace EventStore.Core.Authentication {
	public interface IAuthenticationProviderFactory {
		IAuthenticationProvider BuildAuthenticationProvider(bool logFailedAuthenticationAttempts);

		void RegisterHttpControllers(IHttpService externalHttpService, HttpSendService httpSendService,
			IPublisher mainQueue, IPublisher networkSendQueue);
	}
}
