import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription ,  Observable } from 'rxjs';

import { HttpClient, HttpHeaders } from '@angular/common/http';


import { OAuthService } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc';
import { authConfig } from './auth.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';
  apiResponse: any;
  claims: any;
  constructor(
    private oauthService: OAuthService,
    private httpClient: HttpClient) {
      this.configureWithNewConfigApi();
  }

  private configureWithNewConfigApi() {
    this.oauthService.configure(authConfig);
    this.oauthService.setStorage(localStorage);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
    this.oauthService.setupAutomaticSilentRefresh();
  }

  ngOnInit() {
    this.oauthService.tryLogin({
        onTokenReceived: (info) => {
            console.debug('state', info.state);
        }
    });
  }

  ngOnDestroy(): void {
  }

  login() {
    this.oauthService.initImplicitFlow();
  }

  showClaims() {
    this.claims = this.oauthService.getIdentityClaims();
  }

  manualSilentRefresh() {
    this
      .oauthService
      .silentRefresh()
      .then(info => console.debug('refresh ok', info))
      .catch(err => console.error('refresh error', err));
  }

  getApi() {
    this.httpClient
      // .get("https://localhost:44383/api/WebApiResrouce", {
      //   headers: new HttpHeaders(
      //     {
      //       'Authorization': 'Bearer ' + this.oauthService.getAccessToken()
      //     }
      //   )
      // })
      .get("https://localhost:44383/api/WebApiResrouce")
      .subscribe(data => this.apiResponse = data, error => this.apiResponse = {});
      
  }

  logout() {
    this.oauthService.logOut();
  }
}
