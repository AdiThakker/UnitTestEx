﻿// Copyright (c) Avanade. Licensed under the MIT License. See https://github.com/Avanade/UnitTestEx

using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace UnitTestEx.Mocking
{
    /// <summary>
    /// Provides the <see cref="System.Net.Http.HttpClient"/> (more specifically <see cref="HttpMessageHandler"/>) mocking.
    /// </summary>
    public class MockHttpClient
    {
        private readonly List<MockHttpClientRequest> _requests = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpClient"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="MockHttpClientFactory"/>.</param>
        /// <param name="name">The logical name of the client.</param>
        /// <param name="baseAddress">The base Uniform Resource Identifier (URI) of the Internet resource used when sending requests; defaults to '<c>https://unittest</c>' where not specified.</param>
        internal MockHttpClient(MockHttpClientFactory factory, string name, Uri? baseAddress)
        {
            Factory = factory;
            HttpClient = new HttpClient(new MockHttpClientHandler(factory, MessageHandler.Object)) { BaseAddress = baseAddress ?? new Uri("https://unittest") };
            Factory.HttpClientFactory.Setup(x => x.CreateClient(It.Is<string>(x => x == name))).Returns(() => HttpClient);
        }

        /// <summary>
        /// Gets the <see cref="MockHttpClientFactory"/>.
        /// </summary>
        internal MockHttpClientFactory Factory { get; }

        /// <summary>
        /// Gets the <see cref="Mock"/> <see cref="HttpMessageHandler"/>.
        /// </summary>
        public Mock<HttpMessageHandler> MessageHandler { get; } = new Mock<HttpMessageHandler>();

        /// <summary>
        /// Verifies that all verifiable <see cref="Mock"/> expectations have been met; being all requests have been invoked.
        /// </summary>
        /// <remarks>This is a wrapper for '<c>MessageHandler.Verify()</c>' which can be invoked directly to leverage additional capabilities (overloads). Additionally, the <see cref="MockHttpClientRequest.Verify"/> is invoked for each 
        /// underlying <see cref="Request(HttpMethod, string)"/> to perform the corresponding <see cref="MockHttpClientRequest.Times(Times)"/> verification.<para>Note: no verify will occur where using sequences; this appears to be a
        /// limitation of MOQ.</para></remarks>
        public void Verify()
        {
            MessageHandler.Verify();

            foreach (var r in _requests)
            {
                r.Verify();
            }
        }

        /// <summary>
        /// Gets the mocked <see cref="HttpClient"/>.
        /// </summary>
        internal HttpClient HttpClient { get; set; }

        /// <summary>
        /// Creates a new <see cref="MockHttpClientRequest"/> for the <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="method">The <see cref="HttpMethod"/>.</param>
        /// <param name="requestUri">The string that represents the request <see cref="Uri"/>.</param>
        /// <returns>The <see cref="MockHttpClientRequest"/>.</returns>
        public MockHttpClientRequest Request(HttpMethod method, string requestUri)
        {
            var r = new MockHttpClientRequest(this, method, requestUri);
            _requests.Add(r);
            return r;
        }
    }
}