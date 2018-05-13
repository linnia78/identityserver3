import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  // Reference : https://github.com/manfredsteyer/angular-oauth2-oidc
  // Url of the Identity Provider
  issuer: 'https://localhost:44388/identity',

  // URL of the SPA to redirect the user to after login
  redirectUri: window.location.origin + '/',

  // The SPA's id. The SPA is registerd with this id at the auth-server
  clientId: 'angular',

  oidc: true,
  requestAccessToken : true,
 
  // set the scope for the permissions the client should request
  // The first three are defined by OIDC. The 4th is a usecase-specific one
  scope: 'openid profile roles api vendor',

  // URL of the SPA to redirect the user after silent refresh
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html'
}