﻿
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Borg.Framework.Identity
{
    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client> {
            new Client {
                ClientId = "oauthClient",
                ClientName = "Example Client Credentials Client Application",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {
                    new Secret("superSecretPassword".Sha256())},
                AllowedScopes = new List<string> {"customAPI.read"}
            },
            new Client {

                ClientId = "openIdConnectClient",
                ClientName = "Example Implicit Client Application",
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                RequireConsent = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role"
                },
                RedirectUris = new List<string> {"http://localhost:6001/signin-oidc"},
                PostLogoutRedirectUris = new List<string> { "http://localhost:6001/" }
            },
                            new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = true,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                RedirectUris = new List<string> {"http://localhost:6001/signin-oidc"},
                PostLogoutRedirectUris = new List<string> { "http://localhost:6001/" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                                    IdentityServerConstants.StandardScopes.Email,
                                           IdentityServerConstants.StandardScopes.Phone,
                          "role",
                    "customAPI"
                    },
                    AllowOfflineAccess = true
                }
        };
        }
    }

    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource {
                Name = "role",
                UserClaims = new List<string> {"role", ClaimTypes.Role }
            }
        };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {
            new ApiResource {
                Name = "customAPI",
                DisplayName = "Custom API",
                Description = "Custom API Access",
                UserClaims = new List<string> {"role"},
                ApiSecrets = new List<Secret> {new Secret("scopeSecret".Sha256())},
                Scopes = new List<Scope> {
                    new Scope("customAPI.read"),
                    new Scope("customAPI.write")
                }
            }
        };
        }
    }

    internal class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser> {
            new TestUser {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "mits",
                Password = "bits",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                    new Claim(JwtClaimTypes.Role, "admin"),
                    new Claim(JwtClaimTypes.NickName, "mitsbits")
                }
            }
        };
        }
    }
}



