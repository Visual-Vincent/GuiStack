/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;

namespace GuiStack.Authentication
{
    /// <summary>
    /// Defines properties and methods for a service authenticating against an API or SDK.
    /// </summary>
    public abstract class Authenticator<TCredentials, TAuthenticatedClient>
        where TCredentials : class
        where TAuthenticatedClient : class
    {
        /// <summary>
        /// Authenticates against the API or SDK, using the credentials returned by <see cref="GetCredentials"/>, and returns the authenticated client.
        /// </summary>
        /// <param name="credentials">The credentials used to authenticated with the API or SDK.</param>
        public TAuthenticatedClient Authenticate()
        {
           return  Authenticate(GetCredentials());
        }

        /// <summary>
        /// Authenticates against the API or SDK and returns the authenticated client.
        /// </summary>
        /// <param name="credentials">The credentials used to authenticate with the API or SDK.</param>
        public abstract TAuthenticatedClient Authenticate(TCredentials credentials);

        /// <summary>
        /// Gets the authentication credentials. Inheriting classes want to use this to automatically fetch credentials from e.g. environment variables, user session, etc.
        /// </summary>
        public abstract TCredentials GetCredentials();
    }
}
